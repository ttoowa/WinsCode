using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
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
		public bool IsOpened => Visibility == Visibility.Visible;
		//FX
		private float fxAlpha;
		private SoundPlayer soundPlayer;

		public TerminalWindow() {
			InitializeComponent();
			Init();
		}
		private void Init() {
			root = new Root(this);
			outputBuilder = new StringBuilder();
			soundPlayer = new SoundPlayer(Properties.Resources.Keyboard_EnterClick);
			cmdProc = new CMDProcess();
			cmdProc.Start();

			UpdateWorkingDir();

			ClearUI();
			//Test();
			UpdateResolution();
			RegisterEvent();
			cmdProc.UpdateWorkingDirectory();

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
				InputContext.MouseDown += OnMouseDown_InputContext;
				Closing += OnClosing;

				root.loopEngine.AddLoopAction(OnTick);
			}
		}



		private void OnTick() {
			const float FadeDelta = 0.003f;

			if(fxAlpha > 0f) {
				fxAlpha = Mathf.Clamp01(fxAlpha - FadeDelta);

				Color color = new Color();
				color.R = color.G = color.B = 255;
				color.A = (byte)(fxAlpha * BMath.Float2Byte);
				FXCover.Background = new SolidColorBrush(color);
			}
		}
		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (!root.exitSwitch) {
				e.Cancel = true;
				SetOpened(false);
			} else {
				root.trayIconManager.trayIcon.Hide();
			}
		}
		private void OnMouseDown_InputContext(object sender, MouseButtonEventArgs e) {
			InputTextView.Focus();
		}
		private void OnKeyDown_InputTextView(object sender, KeyEventArgs e) {
			switch (e.Key) {
				case Key.Return:
					string input = InputTextView.Text;

					HandleCommand(input);
					InputTextView.Text = "";
					FireFX();

					e.Handled = true;
					break;
				case Key.Tab:
					e.Handled = true;
					break;
			}
		}
		private void OnOutputReceived_CMDProc(string text, bool isError) {
			WriteLine(text);
		}

		private void FireFX() {
			const float Alpha = 0.1f;
			fxAlpha = Alpha;

			soundPlayer.Play();
		}

		public void SetOpened(bool open) {
			Visibility = open ? Visibility.Visible : Visibility.Collapsed;

			if(open) {
				SetForeground();
				InputTextView.Focus();
			}
		}
		private void SetForeground() {
			Topmost = false;
			Topmost = true;
		}
		private void UpdateResolution() {
			System.Windows.Forms.Screen screen = ScreenInfo.MainScreen;
			Left = screen.Bounds.Left;
			Top = screen.Bounds.Top;
			Width = screen.Bounds.Width;
		}

		private void HandleCommand(string command) {
			string[] cmds = command.Split(' ');
			if (cmds.Length == 0)
				return;

			switch (cmds[0].ToLower()) {
				case "exit":
					SetOpened(false);
					Clear();
					break;
				default:
					cmdProc.Execute(command);
					break;
			}
		}

		public void Clear() {
			OutputTextView.Dispatcher.BeginInvoke(new Action(() => {
				outputBuilder.Clear();

				ApplyOutputTextView();
			}));
		}
		public void Write(string text) {
			OutputTextView.Dispatcher.BeginInvoke(new Action(() => {
				outputBuilder.Append(text);

				ApplyOutputTextView();
			}));
		}
		public void WriteLine(string text) {
			OutputTextView.Dispatcher.BeginInvoke(new Action(() => {
				outputBuilder.AppendLine(text);

				ApplyOutputTextView();
			}));
		}
		private void ApplyOutputTextView() {
			OutputTextView.Text = outputBuilder.ToString();
			OutputScrollView.ScrollToEnd();
			UpdateWorkingDir();
		}

		public void UpdateWorkingDir() {
			WorkingDirTextView.Dispatcher.BeginInvoke(new Action(() => {
				WorkingDirTextView.Text = cmdProc.workingDirectory;
			}));
		}
	}
}
