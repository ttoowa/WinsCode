using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinsCode.Plugins {
    public class TestPlugin : Plugin
    {
		private static Root Root => Root.Instance;
		private static TerminalWindow TerminalWindow => Root.terminalWindow;
        public override string Name => "Test plugin";

        public override PluginVersion Version => new PluginVersion(1, 0, 0, 1);

        public override void finalize(TerminalWindow instance) {
            //instance.WriteLine(string.Format("Finalizing {0} v{1}...", Name, Version));
        }

        public override bool initialize(TerminalWindow instance) {
			//instance.WriteLine(string.Format("Initializing {0} v{1}...", Name, Version));

			TerminalWindow.autoCompleteManager.RegisterAutoCompleter(new TestTest());

            return true;
        }
    }
}