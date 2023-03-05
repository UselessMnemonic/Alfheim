using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

using Hermodr.Messages;
using Hermodr.Parsers;

namespace Hermodr;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class HermodrPlugin : BaseUnityPlugin
{
    private Harmony _harmony;
    private TcpListener _serverSocket;
    private MessageParser _parser;

    /// <summary>
    /// Called when this plugin should load.
    /// </summary>
    private void Awake()
    {
        var configDirectory = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID);
        var configFile = Path.Combine(configDirectory, "default.cfg");

        var config = new ConfigFile(configFile, true);
        var portConfig = config.Bind("Default", "port", 2458, "Messaging port");
        var hostConfig = config.Bind("Default", "hostname", "localhost", "Hostname on which to bind");
        var protocolConfig = config.Bind("Default", "protocol", MessageParser.Type.Json, "Message format protocol, either 'zpacket' or 'json'");

        switch (protocolConfig.Value)
        {
            case MessageParser.Type.Json:
                _parser = JsonMessageParser.Instance;
                break;
            default:
                Logger.LogError($"Unknown protocol {protocolConfig.Value}");
                return;
        }

        _harmony = Harmony.CreateAndPatchAll(typeof(HermodrPlugin).Assembly, PluginInfo.PLUGIN_GUID);
        StartServerAsync(hostConfig.Value, portConfig.Value, protocolConfig.Value);
    }

    /// <summary>
    ///     Called when this plugin should unload.
    /// </summary>
    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
        try
        {
            _serverSocket?.Stop();
        }
        catch (SocketException e)
        {
            Logger.LogError($"Issue closing RPC port: {e.Message}");
        }

        Logger.LogInfo("The Hermodr is closed!");
    }

    /// <summary>
    /// Main server listening loop
    /// </summary>
    private async void StartServerAsync(string hostname, int port, string protocol)
    {
        try
        {
            var hostAddresses = await Dns.GetHostAddressesAsync(hostname);
            var serverAddress = hostAddresses.First(x => x.AddressFamily is AddressFamily.InterNetwork);
            var serverEndpoint = new IPEndPoint(serverAddress, port);
            _serverSocket = new TcpListener(serverEndpoint);
            _serverSocket.Start();
            Logger.LogInfo($"RPC Server started on {serverEndpoint}");
        }
        catch (Exception e)
        {
            Logger.LogError($"RPC Server could not start: {e.Message}");
            return;
        }

        for (;;)
        {
            try
            {
                var socket = await _serverSocket.AcceptTcpClientAsync();
                HandleClientAsync(socket);
            }
            catch (SocketException e)
            {
                Logger.LogError($"Error while accepting a client: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Communicates with a client when they connect
    /// </summary>
    /// <param name="client"></param>
    private async void HandleClientAsync(TcpClient client)
    {
        if (client.Client.RemoteEndPoint is IPEndPoint ep)
        {
            Logger.LogInfo($"Client {ep.Address}:{ep.Port} connected");
        }
        else
        {
            Logger.LogInfo("Client connected");
        }
        try
        {
            var stream = client.GetStream();
            var requests = _parser.DeserializeStream(stream);
            await foreach (var request in requests)
            {
                Logger.LogInfo($"Client sent request {request}");
                try
                {
                    var response = HandleRequest(request);
                    await _parser.Serialize(stream, response);
                }
                catch (Exception e)
                {
                    var error = new CommandError(request.Op, request.Sequence)
                    {
                        Message = e.Message
                    };
                    await _parser.SerializeResponse(stream, error);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"Client error: {e.Message}");
        }

        client.Close();
        Logger.LogInfo("Client disconnected!");
    }

    private CommandResponse HandleRequest(Message request)
    {
        if (request is StatusRequest statusRequest)
        {
            var players = ZNet.instance.GetPlayerList();
            return new StatusResponse(request.Sequence)
            {
                WorldName = ZNet.instance.GetWorldName(),
                Players = players.Select(x => x.m_name).ToArray()
            };
        }
        if (request is BroadcastRequest broadcastRequest)
        {
            Chat.instance.SendText(Talker.Type.Normal, broadcastRequest.Message);
            return new BroadcastResponse(request.Sequence);
        }
        return new CommandError(request.Op, request.Sequence)
        {
            Message = "Unknown opcode."
        };
    }
}
