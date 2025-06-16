using System;
using MHLab.Patch.Core.IO;

namespace MHLab.Patch.Core
{
	[Serializable]
	public class Settings : ISettings
	{
		public string RootPath { get; set; }

		public string AppDataPath { get; set; }

		public string SoftwareVersion => "2.6.2";

		public string GameFolderName { get; set; } = "Game";

		public string EncryptionKeyphrase { get; set; } = "dwqqe2231ffe32";

		public string BuildsFolderName { get; set; } = "Builds";

		public string PatchesFolderName { get; set; } = "Patches";

		public string UpdaterFolderName { get; set; } = "Updater";

		public string LogsFolderName { get; set; } = "Logs";

		public string TempFolderName { get; set; } = "Temp";

		public string VersionFileName { get; set; } = "version.data";

		public string BuildsIndexFileName { get; set; } = "builds_index.json";

		public string BuildDefinitionFileName { get; set; } = "build_{0}.json";

		public string PatchesIndexFileName { get; set; } = "patches_index.json";

		public string PatchFileName { get; set; } = "{0}_{1}.zip";

		public string PatchDefinitionFileName { get; set; } = "{0}_{1}.json";

		public string UpdaterIndexFileName { get; set; } = "updater_index.json";

		public string UpdaterSafeModeIndexFileName { get; set; } = "updater_safemode_index.json";

		public string LogsFileName { get; set; } = "logs-{0}.txt";

		public string LaunchArgumentParameter { get; set; } = "--launchArgument";

		public string LaunchArgumentValue { get; set; } = "Qjshn1k!ncS_298";

		public bool DebugMode { get; set; }

		public virtual string GetLogsFilePath()
		{
			return PathsManager.Combine(RootPath, LogsFolderName, string.Format(LogsFileName, $"{DateTime.UtcNow.Year}{DateTime.UtcNow.Month}{DateTime.UtcNow.Day}"));
		}

		public virtual string GetLogsDirectoryPath()
		{
			return PathsManager.Combine(RootPath, LogsFolderName);
		}

		public virtual string ToDebugString()
		{
			return $"Debug Mode => {DebugMode}\n" + "RootPath => " + RootPath + "\nAppDataPath => " + AppDataPath + "SoftwareVersion => " + SoftwareVersion + "GetLogsFilePath() => " + GetLogsFilePath() + "\n";
		}
	}
}
