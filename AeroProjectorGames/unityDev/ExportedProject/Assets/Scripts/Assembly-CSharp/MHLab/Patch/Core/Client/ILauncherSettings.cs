using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Versioning;

namespace MHLab.Patch.Core.Client
{
	public interface ILauncherSettings : ISettings
	{
		int PatchDownloadAttempts { get; set; }

		string RemoteUrl { get; set; }

		bool PatcherUpdaterSafeMode { get; set; }

		bool EnableDiskSpaceCheck { get; set; }

		string GetVersionFilePath();

		string GetGamePath();

		string GetTempPath();

		FilePath GetSettingsOverridePath();

		string GetDownloadedPatchArchivePath(IVersion from, IVersion to);

		string GetUncompressedPatchArchivePath(IVersion from, IVersion to);

		string GetUpdaterSafeModeTempPath();

		string GetUpdaterSafeModeBackupPath();

		FilePath GetUpdaterSafeModeLockFilePath();

		string GetRemoteBuildsIndexUrl();

		string GetRemoteBuildDefinitionUrl(IVersion version);

		string GetRemoteBuildUrl(IVersion version);

		string GetPartialRemoteBuildUrl(IVersion version);

		string GetRemotePatchesIndexUrl();

		string GetRemotePatchDefinitionUrl(IVersion from, IVersion to);

		string GetRemotePatchArchiveUrl(IVersion from, IVersion to);

		string GetRemoteUpdaterIndexUrl();

		string GetRemoteUpdaterSafeModeIndexUrl();

		string GetRemoteUpdaterSafeModeArchiveUrl(string archiveName);

		string GetRemoteUpdaterFileUrl(string relativePath);
	}
}
