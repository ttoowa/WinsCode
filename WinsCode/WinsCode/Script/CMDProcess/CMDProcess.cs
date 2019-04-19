using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GKit;
using GKit.Security;

namespace WinsCode {
	public class CMDProcess : IDisposable {
		private Root Root => Root.Instance;
		private TerminalWindow TerminalWindow => Root.terminalWindow;

		public delegate void OnOutputReceivedDelegate(string text, bool isError);
		private const string WinsCodeCMDToken = "__WINSCODE_CMD";

		public event OnOutputReceivedDelegate OnOutputReceived;
		public bool IsRunning {
			get; private set;
		}
		public string workingDirectory;
		public Process proc;
        private string fileName;

		private SelfCmdType selfCmdType = SelfCmdType.None;
		private int ignoreLineCount;


		public CMDProcess() : this("cmd") {

		}
		public CMDProcess(string fileName) {
            this.fileName = fileName;

            Init();
        }
        public void Init()
        {
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
			proc.EnableRaisingEvents = true;
            InitWorkingDir();
            RegisterEvent();
            Start();

            void RegisterEvent()
            {
                proc.Exited += OnExited_Proc;
                proc.OutputDataReceived += OnOutputReceived_Proc;
                proc.ErrorDataReceived += OnErroeDataReceived_Proc;
            }
        }

        public void Dispose() {
			try {
				Stop();

			} finally {
				proc.Dispose();
			}
		}

        private void OnExited_Proc(object sender, EventArgs e)
        {
            //캐치 안됨
			IsRunning = false;
			TerminalWindow.Clear();
			Init();
        }
        private void OnOutputReceived_Proc(object sender, DataReceivedEventArgs e) {
			if(!HandleReceivedSelfCmd(e.Data)) {
				OnOutputReceived?.Invoke(e.Data, false);
			}
		}
		private void OnErroeDataReceived_Proc(object sender, DataReceivedEventArgs e) {
			OnOutputReceived?.Invoke(e.Data, true);
		}
		private bool HandleReceivedSelfCmd(string text) {
			if (string.IsNullOrEmpty(text)) {
				return true;
			} else if (ignoreLineCount > 0) {
				--ignoreLineCount;
				return true;
			} else if (text.Contains(WinsCodeCMDToken)) {
				//입력 브로드캐스팅 무시
				if (text.Contains("echo"))
					return true;

				string[] words = text.Split(' ');
				if (words.Length > 2) {
					int cmdTypeNum;
					if (int.TryParse(words[1], out cmdTypeNum)) {
						selfCmdType = (SelfCmdType)cmdTypeNum;
						ignoreLineCount = 1;
						return true;
					}
				}
			} else if (selfCmdType != SelfCmdType.None) {
				OnReceived_SelfCMD(selfCmdType, text);

				selfCmdType = SelfCmdType.None;
				return true;
			}
			return false;
		}

		public bool Start() {
			if (IsRunning)
				return false;
			IsRunning = true;

			proc.Start();
			proc.BeginOutputReadLine();
			proc.BeginErrorReadLine();

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
		public void Execute(string command, bool updateWorkingDir = true) {
			proc.StandardInput.WriteLine(command);
			proc.StandardInput.Flush();

			if(updateWorkingDir) {
				ExecuteUpdateWorkingDirectory();
			}
		}

		public string GetChildProcessName() {
			StringBuilder builder = new StringBuilder();
			Process[] childProcs = ProcessUtility.GetChildProcesses(proc);
			if(childProcs.Length >= 2) {
				FindChildProc(childProcs[1]);
				return builder.ToString();
			} else {
				return null;
			}

			void FindChildProc(Process proc) {
				Process[] subChildProcs = ProcessUtility.GetChildProcesses(proc);
				if(subChildProcs.Length >= 1) {
					Process childProc = subChildProcs[0];
					builder.Append(' ');
					builder.Append(childProc.ProcessName);
					builder.Append('>');

					FindChildProc(childProc);
				}
			}
		}

		//SelfCmd
		public void ExecuteUpdateWorkingDirectory() {
			ExecuteSelfCMD(SelfCmdType.GetWorkingDirectory, "cd");
		}
		public void ExecuteSelfCMD(SelfCmdType type, string command) {
			Execute($"echo {WinsCodeCMDToken} {((int)type).ToString()} {Environment.NewLine}{command}", type != SelfCmdType.GetWorkingDirectory);
		}
		private void OnReceived_SelfCMD(SelfCmdType type, string result) {
			switch (type) {
				case SelfCmdType.GetWorkingDirectory:
					workingDirectory = result;
					TerminalWindow.UpdateWorkingDir();
					break;
			}
		}

	}
}
