using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using GKit;
using Application = System.Windows.Application;

namespace WinsCode {
	public class TrayIconManager {
		private Root Root => Root.Instance;
		private TerminalWindow TerminalWindow => Root.terminalWindow;

		public TrayIcon trayIcon;

		public TrayIconManager() {
			trayIcon = new TrayIcon();
			trayIcon.SetIcon("Resources/Images/Logo.ico");

			trayIcon.AddMenuItem("터미널 열기", OnClick_OpenTerminal);
			MenuItem startupItem = trayIcon.AddCheckableMenuItem("시작프로그램 등록", OnClick_SwitchStartProcess);
			trayIcon.AddMenuSeparator();
			trayIcon.AddMenuItem("종료", OnClick_Exit);

			trayIcon.OnDoubleClick += OnDoubleClick;

			trayIcon.Show();
			
			if(Startup.Get(AppInfo.AppFilter)) {
				startupItem.Checked = true;
			}
		}

		private void OnDoubleClick() {
			TerminalWindow.SetOpened(!TerminalWindow.IsOpened);
		}
		private void OnClick_OpenTerminal() {
			TerminalWindow.SetOpened(true);
		}
		private void OnClick_SwitchStartProcess(bool isChecked) {
			Startup.Set(AppInfo.AppFilter, isChecked);
		}
		private void OnClick_Exit() {
			Root.exitSwitch = true;
			Application.Current.Shutdown();
		}

	}
}
