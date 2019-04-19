using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKit;

namespace WinsCode {
	public class RecentCmdManager {

		public const int MaxStack = 50;
		public int Pointer {
			get; private set;
		}
		public bool IsPointingCurrent => Pointer == -1;
		private List<string> recentCmdList;

		public RecentCmdManager() {
			recentCmdList = new List<string>();
		}
		public void AddPointer(int value) {
			if (recentCmdList.Count > 0) {
				Pointer = Mathf.Clamp(Pointer + value, 0, recentCmdList.Count - 1);
			} else {
				Pointer = -1;
			}
		}
		public void SetPointToCurrent() {
			Pointer = -1;
		}

		public void AddCommand(string command) {
			if (recentCmdList.Count > 0 && recentCmdList[0] == command)
				return;

			recentCmdList.Insert(0, command);
			ClearOld();
		}
		public string GetCommand() {
			return recentCmdList.Count > 0 ? recentCmdList[Pointer] : null;
		}

		private void ClearOld() {
			if (recentCmdList.Count > MaxStack) {
				recentCmdList.RemoveAt(recentCmdList.Count - 1);
			}
		}
	}
}
