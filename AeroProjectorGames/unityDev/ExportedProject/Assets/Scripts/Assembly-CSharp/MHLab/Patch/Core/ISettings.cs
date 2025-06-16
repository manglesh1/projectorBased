namespace MHLab.Patch.Core
{
	public interface ISettings
	{
		string RootPath { get; set; }

		string AppDataPath { get; set; }

		string SoftwareVersion { get; }

		string GameFolderName { get; set; }

		string EncryptionKeyphrase { get; set; }

		string BuildsFolderName { get; set; }

		string PatchesFolderName { get; set; }

		string UpdaterFolderName { get; set; }

		string LogsFolderName { get; set; }

		string TempFolderName { get; set; }

		string VersionFileName { get; set; }

		string BuildsIndexFileName { get; set; }

		string BuildDefinitionFileName { get; set; }

		string PatchesIndexFileName { get; set; }

		string PatchFileName { get; set; }

		string PatchDefinitionFileName { get; set; }

		string UpdaterIndexFileName { get; set; }

		string UpdaterSafeModeIndexFileName { get; set; }

		string LogsFileName { get; set; }

		string LaunchArgumentParameter { get; set; }

		string LaunchArgumentValue { get; set; }

		bool DebugMode { get; set; }

		string GetLogsFilePath();

		string GetLogsDirectoryPath();

		string ToDebugString();
	}
}
