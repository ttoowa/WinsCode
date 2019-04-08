using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinsCode.Plugins {
    public class TestPlugin : Plugin
    {
        public override string Name => "Test plugin";

        public override PluginVersion Version => new PluginVersion(1, 0, 0, 1);

        public override void finalize(Instance instance) {
            instance.writeLine(string.Format("Finalizing {0} v{1}...", Name, Version));
        }

        public override bool initialize(Instance instance) {
            instance.writeLine(string.Format("Initializing {0} v{1}...", Name, Version));

            return true;
        }
    }
}