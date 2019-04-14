using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace WinsCode {
	public static class ProcessUtility {
		public static Process[] GetChildProcesses(this Process process) {
			List<Process> children = new List<Process>();
			ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

			foreach (ManagementObject mo in mos.Get()) {
				children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
			}

			return children.ToArray();
		}
	}
}
