using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Hermodr.Extensions;
using Hermodr.Gateway;
using Hermodr.Gateway.Packets;
s
namespace Hermodr;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class HermodrPlugin : BaseUnityPlugin
{
    private Harmony _harmony;
    private GatewayServer _server;
    private bool _isActive;

    /// <summary>
    /// Called when this plugin should load.
    /// </summary>
    private void Awake()
    {
        HermodrPlugin.LoadTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var configDirectory = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID);
        var configFile = Path.Combine(configDirectory, "default.cfg");

        var config = new ConfigFile(configFile, true);
        var portConfig = config.Bind("Default", "port", 2458, "Messaging port");
        var hostConfig = config.Bind("Default", "hostname", "localhost", "Hostname on which to bind");

        _harmony = Harmony.CreateAndPatchAll(typeof(HermodrPlugin).Assembly, PluginInfo.PLUGIN_GUID);
        ServerMainAsync(hostConfig.Value, portConfig.Value);
    }

    /// <summary>
    /// Called when this plugin should unload.
    /// </summary>
    private void OnDestroy()
    {
        try
        {
            _server?.Stop();
        }
        catch (SocketException e)
        {
            Logger.LogWarning($"Error while closing gateway server: {e.Message}");
        }
        _harmony?.UnpatchSelf();
        Logger.LogInfo("The Hermodr is closed!");
    }

    /// <summary>
    /// Main server listening loop
    /// </summary>
    private async void ServerMainAsync(string hostname, int port)
    {
        // wait for ZNet to be live
        while (ZNet.instance == null)
        {
            await Task.Yield();
        }
        IPEndPoint serverEndpoint;
        try
        {
            var hostAddresses = await Dns.GetHostAddressesAsync(hostname);
            var serverAddress = hostAddresses.First(x => x.AddressFamily is AddressFamily.InterNetwork);
            serverEndpoint = new IPEndPoint(serverAddress, port);
        }
        catch (Exception e)
        {
            Logger.LogError($"Endpoint could not be resolved: {e.Message}");
            return;
        }
        try
        {
            _server = new GatewayServer(serverEndpoint);
            _server.Start();
            Logger.LogInfo($"RPC Server started on {serverEndpoint}");
        }
        catch (Exception e)
        {
            Logger.LogError($"Gateway server could not be started: {e.Message}");
            return;
        }

        _isActive = true;
        while (_isActive)
        {
            try
            {
                var client = await _server.AcceptGatewayClientAsync();
                HandleClientAsync(client);
            }
            catch (SocketException e)
            {
                Logger.LogWarning($"Error while accepting a client: {e.Message}");
                _isActive = (e.SocketErrorCode == SocketError.Interrupted);
            }
        }
        _server.Stop();
    }

    /// <summary>
    /// Communicates with a client when they connect
    /// </summary>
    /// <param name="client"></param>
    private async void HandleClientAsync(GatewayClient client)
    {
        var remoteEp = client.Client.Client.RemoteEndPoint.Serialize().ToString();
        Logger.LogInfo($"Client connected: {remoteEp}");
        while (_isActive)
        {
            try {
                var request = await client.RecvAsync();
                byte[] buffer;
                int offset;
                switch (request.Op)
                {
                    case 1:
                        var players = ZNet.instance.GetPlayerList();
                        var nameSizes = players
                            .Select(x => Encoding.UTF8.GetByteCount(x.m_name))
                            .ToList();
                        buffer = new byte[4 + (4 * players.Count) + nameSizes.Sum()];
                        offset = 0;

                        DataEncodings.PutBytesBE(players.Count, buffer, offset);
                        offset += 4;

                        for (var i = 0; i < players.Count; i++)
                        {
                            var nameSize = nameSizes[i];
                            DataEncodings.PutBytesBE(nameSize, buffer, offset);
                            offset += 4;
                            
                            var actualSize = Encoding.UTF8.GetBytes(players[i].m_name, 0, name.Length, buffer, offset);
                            offset += actualSize;
                        }
                        break;
                    case 2:
                        var worldName = ZNet.instance.GetWorldName();
                        var worldNameSize = Encoding.UTF8.GetByteCount(worldName);
                        buffer = new byte[4 + worldNameSize];
                        offset = 0;
                        
                        DataEncodings.PutBytesBE(worldNameSize, buffer, offset);
                        offset += 4;
                        
                        Encoding.UTF8.GetBytes(worldName, 0, worldName.Length, buffer, offset);
                        break;
                    case 3:
                        ZNet.instance.GetNetStats(
                            out var localQuality,
                            out var remoteQuality,
                            out var ping,
                            out var outByteSec,
                            out var inByteSec
                        );
                        buffer = new byte[20];
                        DataEncodings.PutBytesBE(localQuality, buffer, 0);
                        DataEncodings.PutBytesBE(remoteQuality, buffer, 4);
                        DataEncodings.PutBytesBE(ping, buffer, 8);
                        DataEncodings.PutBytesBE(outByteSec, buffer, 12);
                        DataEncodings.PutBytesBE(inByteSec, buffer, 16);
                        break;
                    case 4:
                        buffer = new byte[8];
                        DataEncodings.PutBytesBE(HermodrPlugin.LoadTime, buffer, 0);
                        break;
                    default:
                        buffer = Array.Empty<byte>();
                        break;
                }
                var response = new BinaryPacket(request.Id, request.Op, buffer);
                await client.SendAsync(response);
                continue;
            error:
                response = new BinaryPacket(request.Id, -1);
                await client.SendAsync(response);
            }
            catch (Exception e)
            {
                Logger.LogInfo($"Error during request: {remoteEp}: {e.Message}");
                break;
            }
        }
        Logger.LogInfo($"Client disconnecting: {remoteEp}");
        client.Dispose();
    }

    public static long LoadTime { get; private set; }
}
