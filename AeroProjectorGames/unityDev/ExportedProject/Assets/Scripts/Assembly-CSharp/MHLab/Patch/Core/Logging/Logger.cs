using System;
using System.IO;

namespace MHLab.Patch.Core.Logging
{
	public static class Logger
	{
		private static readonly object _lock;

		private static readonly string logFilePath;

		static Logger()
		{
			_lock = new object();
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			logFilePath = Path.Combine(text, $"Log_{DateTime.Now:yyyy-MM-dd}.txt");
		}

		public static void LogInfo(string message)
		{
			WriteLog("INFO", message);
		}

		public static void LogWarning(string message)
		{
			WriteLog("WARNING", message);
		}

		public static void LogError(string message)
		{
			WriteLog("ERROR", message);
		}

		private static void WriteLog(string logType, string message)
		{
			string text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logType}] {message}";
			lock (_lock)
			{
				File.AppendAllText(logFilePath, text + Environment.NewLine);
				Console.WriteLine(text);
			}
		}
	}
}
