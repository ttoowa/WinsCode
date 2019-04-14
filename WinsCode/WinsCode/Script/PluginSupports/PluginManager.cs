
using System.Collections.Generic;

namespace WinsCode.Plugins {
    public class PluginManager {
        private TerminalWindow instance;
        private Dictionary<string, Plugin> pluginDict;

        public PluginManager(TerminalWindow instance) {
            this.instance = instance;
            pluginDict = new Dictionary<string, Plugin>();
        }

        public void initAllPlugin() {

        }

        public void registerPlugin(Plugin plugin) {
            plugin.initialize(instance);
            pluginDict.Add(plugin.Name, plugin);
        }

        public void unregisterPlugin(string pluginName) {
            Plugin plugin;

            if (pluginDict.TryGetValue(pluginName, out plugin)) {
                plugin.finalize(instance);
                pluginDict.Remove(pluginName);
            }
        }
    }
}