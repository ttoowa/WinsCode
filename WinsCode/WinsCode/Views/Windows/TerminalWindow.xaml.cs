using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinsCode.Plugins;
using GKit;

namespace WinsCode {
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TerminalWindow : Window {

		private Root root;
		private GLoopEngine LoopEngine => root.loopEngine;
		private CMDProcess cmdProc;
		private StringBuilder outputBuilder;

		public TerminalWindow() {
			InitializeComponent();
			Init();
		}
		private void Init() {
			root = new Root(this);
			outputBuilder = new StringBuilder();
			cmdProc = new CMDProcess();
			cmdProc.Start();

			UpdateWorkingDir();

			ClearUI();
			Test();
			RegisterEvent();

			void ClearUI() {
				InputTextView.Text = "";
				InputTextView.Focus();
			}
			void Test() {
				Left = 0d;
				Top = 0d;
				Width = 1920d;

				OutputTextView.Text = "결과 출력 디스플레이\nExecute...\n존-새 밥오.";

				TestPlugin testPlugin = new TestPlugin();
				root.pluginManager.registerPlugin(testPlugin);
			}
			void RegisterEvent() {
				InputTextView.KeyDown += OnKeyDown_InputTextView;
				cmdProc.OnOutputReceived += OnOutputReceived_CMDProc;
			}
		}

		private void OnKeyDown_InputTextView(object sender, KeyEventArgs e) {
			if(e.Key == Key.Return) {
				string input = InputTextView.Text;

				

				HandleCommand(input);
				InputTextView.Text = "";

				e.Handled = true;
			}
		}
		private void OnOutputReceived_CMDProc(string text) {
			WriteLine(text);
		}
		private void HandleCommand(string command) {
			switch(command) {
				case "exit":
					Application.Current.Shutdown();
					break;
				default:
					cmdProc.Write(command);
					break;
			}
		}

		public void Write(string text) {
			LoopEngine.AddJob(() => {
				outputBuilder.Append(text);

				ApplyOutputTextView();
			});
		}
		public void WriteLine(string text) {
			LoopEngine.AddJob(() => {
				outputBuilder.AppendLine(text);

				ApplyOutputTextView();
			});
		}
		private void ApplyOutputTextView() {
			OutputTextView.Text = outputBuilder.ToString();
			OutputScrollView.ScrollToEnd();
			UpdateWorkingDir();
		}

		private void UpdateWorkingDir() {
			WorkingDirTextView.Text = cmdProc.WorkingDirectory;
		}
	}
}
