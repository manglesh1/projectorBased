using System.Diagnostics;
using MHLab.Patch.Core.IO;

namespace MHLab.Patch.Launcher.Scripts.Utilities
{
	public static class PrivilegesSetter
	{
		public static void EnsureExecutePrivileges(string filePath)
		{
			EnsurePrivilegesWindows(filePath);
		}

		private static void EnsurePrivilegesWindows(string filePath)
		{
			Process process = new Process();
			process.StartInfo.FileName = "ICACLS";
			process.StartInfo.Arguments = "\"" + filePath + "\" /grant \"Users\":M";
			process.Start();
		}

		private static void EnsurePrivilegesMac(string filePath)
		{
			string text = filePath + "/Contents/MacOS/" + PathsManager.GetFilename(filePath).Replace(".app", "");
			Process process = new Process();
			process.StartInfo.FileName = "chmod";
			process.StartInfo.Arguments = "+x \"" + text + "\"";
			process.Start();
			Process process2 = new Process();
			process2.StartInfo.FileName = "xattr";
			process2.StartInfo.Arguments = "-d com.apple.quarantine \"" + filePath + "\"";
			process2.Start();
		}

		private static void EnsurePrivilegesLinux(string filePath)
		{
			Process process = new Process();
			process.StartInfo.FileName = "chmod";
			process.StartInfo.Arguments = "+x \"" + filePath + "\"";
			process.Start();
		}
	}
}
