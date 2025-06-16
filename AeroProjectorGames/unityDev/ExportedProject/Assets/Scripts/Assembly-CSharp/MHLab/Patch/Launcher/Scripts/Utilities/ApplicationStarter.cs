using System.Diagnostics;
using System.IO;

namespace MHLab.Patch.Launcher.Scripts.Utilities
{
	public static class ApplicationStarter
	{
		public static void StartApplication(string filePath, string arguments)
		{
			PrepareStartApplicationWindows(filePath, arguments).Start();
		}

		private static Process PrepareStartApplicationWindows(string filePath, string arguments)
		{
			return new Process
			{
				StartInfo = 
				{
					FileName = filePath,
					Arguments = arguments,
					UseShellExecute = false,
					Verb = "runas",
					WorkingDirectory = Path.GetDirectoryName(filePath)
				}
			};
		}

		private static Process PrepareApplicationMac(string filePath, string arguments)
		{
			Process process = new Process();
			process.StartInfo.FileName = "open";
			process.StartInfo.Arguments = "-a '" + filePath + "' -n --args '" + arguments + "'";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
			return process;
		}

		private static Process PrepareApplicationLinux(string filePath, string arguments)
		{
			return new Process
			{
				StartInfo = 
				{
					FileName = filePath,
					Arguments = arguments,
					UseShellExecute = false,
					WorkingDirectory = Path.GetDirectoryName(filePath)
				}
			};
		}
	}
}
