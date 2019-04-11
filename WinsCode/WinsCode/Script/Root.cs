using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinsCode.Plugins;
using GKit;

namespace WinsCode {
	public class Root {
		public static Root Instance {
			get; private set;
		}

		public bool exitSwitch;

		public GLoopEngine loopEngine;
		public TerminalWindow terminalWindow;
		public PluginManager pluginManager;
		public TrayIconManager trayIconManager;

		public Root(TerminalWindow terminalWindow) {
			Instance = this;
			this.terminalWindow = terminalWindow;
			loopEngine = new GLoopEngine();
			pluginManager = new PluginManager(terminalWindow);
			trayIconManager = new TrayIconManager();

			loopEngine.StartLoop();
			loopEngine.AddLoopAction(OnTick);

		}

		private void OnTick() {
			CheckShowKey();
		}

		private void CheckShowKey() {
			if(KeyInput.GetKey(WinKey.LeftControl) && KeyInput.GetKeyDown(WinKey.BackQuote)) {
				terminalWindow.SetOpened(!terminalWindow.IsOpened);
			}
		}
	}
}
