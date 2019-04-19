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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinsCode.Plugins;
using WinsCode.SubSystem.AutoComplete;
using GKit;

namespace WinsCode {
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TerminalWindow : Window {

		public string WorkingDirectory { get { return CmdProc.workingDirectory; } }
		public string Input { get { return InputEditText.Text; } }

		private Root root;
		private GLoopEngine LoopEngine => root.loopEngine;
		public CMDProcess CmdProc {
			get; private set;
		}
		private StringBuilder outputBuilder;
		public AutoCompleteManager autoCompleteManager;
		private AutoCompleteSet autoCompleteSet;
		private RecentCmdManager recentCmdManager;
		public bool IsOpened => Visibility == Visibility.Visible;
		public IntPtr Handle {
			get; private set;
		}
		//FX
		private float fxAlpha;
		private SoundPlayer soundPlayer;

		public TerminalWindow() {
			InitializeComponent();
			Init();
			Loaded += OnLoaded;
		}


		private void Init() {
			root = new Root(this);
			outputBuilder = new StringBuilder();
			autoCompleteManager = new AutoCompleteManager(this);
			recentCmdManager = new RecentCmdManager();
			soundPlayer = new SoundPlayer(Properties.Resources.Keyboard_EnterClick);
			CmdProc = new CMDProcess();

			FindHandle();
			ClearInput();
			LoadPlugin();
			UpdateResolution();
			RegisterEvent();

			CmdProc.ExecuteUpdateWorkingDirectory();

			void FindHandle() {
				Window window = Window.GetWindow(this);
				var wih = new WindowInteropHelper(window);
				Handle = wih.Handle;
			}
			void ClearInput() {
				InputEditText.Text = "";
				InputEditText.Focus();
			}
			void LoadPlugin() {
				//Left = 0d;
				//Top = 0d;
				//Width = 1920d;

				//OutputTextView.Text = "결과 출력 디스플레이\nExecute...\n존-새 밥오.";

				TestPlugin testPlugin = new TestPlugin();
				root.pluginManager.registerPlugin(testPlugin);
			}
			void RegisterEvent() {
				InputEditText.KeyDown += OnKeyDown_InputEditText;
				InputEditText.PreviewKeyDown += OnPreviewKeyDown_InputEditText;
				CmdProc.OnOutputReceived += OnOutputReceived_CMDProc;
				InputContext.MouseDown += OnMouseDown_InputContext;
				Closing += OnClosing;

				root.loopEngine.AddLoopAction(OnTick);
			}
		}

		private void OnTick() {
			const float FadeDelta = 0.003f;

			if (fxAlpha > 0f) {
				fxAlpha = Mathf.Clamp01(fxAlpha - FadeDelta);

				Color color = new Color();
				color.R = color.G = color.B = 255;
				color.A = (byte)(fxAlpha * BMath.Float2Byte);
				FXCover.Background = new SolidColorBrush(color);
			}
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			SetOpened(false);
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
			InputEditText.Focus();
			e.Handled = false;
		}
		private void OnPreviewKeyDown_InputEditText(object sender, KeyEventArgs e) {
			if (e.Key != Key.Tab)
				autoCompleteSet = null;

			switch (e.Key) {
				case Key.Up:
					recentCmdManager.AddPointer(1);
					ApplyRecentInputCmd();

					e.Handled = true;
					break;
				case Key.Down:
					recentCmdManager.AddPointer(-1);
					ApplyRecentInputCmd();

					e.Handled = true;
					break;
			}
		}
		private void OnKeyDown_InputEditText(object sender, KeyEventArgs e) {
			if (e.Key != Key.Tab)
				autoCompleteSet = null;

			switch (e.Key) {
				case Key.Return:
					string input = InputEditText.Text;

					HandleCommand(input);
					InputEditText.Text = string.Empty;
					FireFX();

					e.Handled = true;
					break;
				case Key.Tab:
					if (autoCompleteSet == null) {
						autoCompleteSet = autoCompleteManager.GetAutoCompleteSet();
					}

					if (autoCompleteSet != null) {
						InputEditText.Text = autoCompleteSet.Context.Input + autoCompleteSet.Next;
						InputEditText.CaretIndex = InputEditText.Text.Length;

						if (autoCompleteSet.Length == 1)
							autoCompleteSet = null;
					}

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

			if (open) {
				SetForeground();
			}
		}
		private async void SetForeground() {
			Topmost = false;
			Topmost = true;
			WinAPI.ShowWindow(Handle, 5);

			InputEditText.Focus();
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

			recentCmdManager.AddCommand(command);
			recentCmdManager.SetPointToCurrent();
			switch (cmds[0].ToLower()) {
				//case "exit":
				//	SetOpened(false);
				//	Clear();
				//	break;
				default:
					CmdProc.Execute(command);
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
		private void ApplyRecentInputCmd() {
			if (!recentCmdManager.IsPointingCurrent) {
				InputEditText.Text = recentCmdManager.GetCommand();
			}
		}

		public void UpdateWorkingDir() {
			//string displayHead;

			//string childProcName = CmdProc.GetChildProcessName();
			//if (childProcName == null) {
			//	displayHead = CmdProc.workingDirectory;
			//} else {
			//	displayHead = childProcName;
			//}
			//displayHead += ">";
			//WorkingDirTextView.Dispatcher.BeginInvoke(new Action(() => {
			//	WorkingDirTextView.Text = displayHead;
			//}));
			WorkingDirTextView.Dispatcher.BeginInvoke(new Action(() => {
				WorkingDirTextView.Text = $"{CmdProc.workingDirectory}>";
			}));
		}
	}
}
