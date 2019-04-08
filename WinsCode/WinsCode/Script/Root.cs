using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinsCode.Plugins;
using GKit;

namespace WinsCode {
	public class Root {
		public static Root Instance {
			get; private set;
		}

		public GLoopEngine loopEngine;
		public TerminalWindow terminalWindow;
		public PluginManager pluginManager;


		public Root(TerminalWindow terminalWindow) {
			Instance = this;
			this.terminalWindow = terminalWindow;
			loopEngine = new GLoopEngine();
			pluginManager = new PluginManager(terminalWindow);

			loopEngine.StartLoop();
		}

	}
}
