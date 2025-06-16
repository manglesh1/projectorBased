using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Helpers
{
	public class UpdateHelper
	{
		private static void StartLauncher(string filePath, string arguments)
		{
			Process process = new Process();
			process.StartInfo.FileName = filePath;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);
			process.Start();
		}

		public static bool CheckForUpdates()
		{
			TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
			if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && timeOfDay >= TimeSpan.FromHours(3.0) && timeOfDay <= TimeSpan.FromHours(8.0))
			{
				RunLauncher();
			}
			return true;
		}

		public static void RunLauncher()
		{
			if (SceneManager.sceneCount > 0)
			{
				StartLauncher(DataPathHelpers.GetLauncherPath(), null);
				Application.Quit();
			}
		}
	}
}
