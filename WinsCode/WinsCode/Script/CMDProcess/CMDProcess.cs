using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WinsCode {
	public class CMDProcess : IDisposable {
		private Root Root => Root.Instance;
		private TerminalWindow TerminalWindow => Root.terminalWindow;

		public delegate void OnOutputReceivedDelegate(string text);

		public event OnOutputReceivedDelegate OnOutputReceived;
		public bool IsRunning {
			get; private set;
		}
		public string WorkingDirectory {
			get {
				return proc.StartInfo.WorkingDirectory;
			}
			set {
				proc.StartInfo.WorkingDirectory = value;
			}
		}
		public Process proc;

		public CMDProcess() : this("cmd") {

		}
		public CMDProcess(string fileName) {
			proc = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo();

			startInfo.FileName = fileName;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardInput = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;

			proc.StartInfo = startInfo;
			InitWorkingDir();
			RegisterEvent();

			void RegisterEvent() {
				proc.OutputDataReceived += OnOutputReceived_Proc;
			}
		}

		public void Dispose() {
			try {
				Stop();

			} finally {
				proc.Dispose();
			}
		}

		private void OnOutputReceived_Proc(object sender, DataReceivedEventArgs e) {
			OnOutputReceived?.Invoke(e.Data);
		}

		public bool Start() {
			if (IsRunning)
				return false;
			IsRunning = true;

			proc.Start();
			proc.BeginOutputReadLine();

			return true;
		}
		public bool Stop() {
			if (!IsRunning)
				return false;
			IsRunning = false;

			proc.Close();

			return true;
		}

		public void InitWorkingDir() {
			proc.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		}

		public void Write(string command) {
			proc.StandardInput.WriteLine(command);
			proc.StandardInput.Flush();
		}

	}
}
