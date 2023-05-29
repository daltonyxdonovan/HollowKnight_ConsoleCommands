using BepInEx;
using UnityEngine;
using Unity;

namespace ConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                PlayerData.instance.AddHealth(1);
            }
        }
    }
}
