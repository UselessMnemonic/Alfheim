using System.IO;
using System.Net;
using System.Net.Sockets;
using BepInEx;
using HarmonyLib;

namespace Alfheim
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class AlfheimPlugin : BaseUnityPlugin
    {
        private TcpListener _serverSocket;
        private Harmony _harmony;

        /// <summary>
        /// Called when this plugin should load.
        /// </summary>
        private void Awake()
        {
            var configDirectory = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID);
            var configFile = Path.Combine(configDirectory, "default.cfg");
            var config = new BepInEx.Configuration.ConfigFile(configFile, true);
            var portConfig = config.Bind("Default", "port", 2458, "Port for RPC.");
            var port = portConfig.Value;
            Logger.LogInfo($"RPC port: ${port}");
            
            try
            {
                _serverSocket = TcpListener.Create(port);
                _serverSocket.Start();
                _harmony = Harmony.CreateAndPatchAll(typeof(AlfheimPlugin).Assembly, PluginInfo.PLUGIN_GUID);
                Logger.LogInfo("The Alfheim is open!");
            }
            catch (SocketException e)
            {
                Logger.LogError($"RPC port could not be opened: ${e.Message}");
            }
        }

        /// <summary>
        /// Called when this plugin should unload.
        /// </summary>
        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            _serverSocket?.Stop();
            Logger.LogInfo("The Alfheim is closed!");
        }
    }
}
