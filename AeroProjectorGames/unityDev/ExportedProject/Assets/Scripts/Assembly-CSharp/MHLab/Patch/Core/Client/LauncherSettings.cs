using System;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Versioning;

namespace MHLab.Patch.Core.Client
{
	[Serializable]
	public class LauncherSettings : Settings, ILauncherSettings, ISettings
	{
		public int PatchDownloadAttempts { get; set; } = 3;

		public string RemoteUrl { get; set; } = "http://localhost/patch";

		public bool PatcherUpdaterSafeMode { get; set; }

		public bool EnableDiskSpaceCheck { get; set; } = true;

		public virtual string GetVersionFilePath()
		{
			return PathsManager.Combine(base.RootPath, base.GameFolderName, base.VersionFileName);
		}

		public virtual string GetGamePath()
		{
			return PathsManager.Combine(base.RootPath, base.GameFolderName);
		}

		public virtual string GetTempPath()
		{
			return PathsManager.Combine(base.AppDataPath, base.TempFolderName);
		}

		public virtual FilePath GetSettingsOverridePath()
		{
			return new FilePath(base.RootPath, PathsManager.Combine(base.RootPath, "settings.json"));
		}

		public virtual string GetDownloadedPatchArchivePath(IVersion from, IVersion to)
		{
			return PathsManager.Combine(GetTempPath(), string.Format(base.PatchFileName, from, to));
		}

		public virtual string GetUncompressedPatchArchivePath(IVersion from, IVersion to)
		{
			return PathsManager.Combine(GetTempPath(), $"{from}_{to}_uncompressed");
		}

		public virtual string GetUpdaterSafeModeTempPath()
		{
			return PathsManager.Combine(GetTempPath(), "UpdaterSafeMode");
		}

		public virtual string GetUpdaterSafeModeBackupPath()
		{
			return PathsManager.Combine(GetTempPath(), "UpdaterSafeModeBackup");
		}

		public virtual FilePath GetUpdaterSafeModeLockFilePath()
		{
			return new FilePath(GetTempPath(), PathsManager.Combine(GetTempPath(), "patch_tmp_safemode_lock"));
		}

		public virtual string GetRemoteBuildsIndexUrl()
		{
			return PathsManager.UriCombine(RemoteUrl, base.BuildsFolderName, base.BuildsIndexFileName);
		}

		public virtual string GetRemoteBuildDefinitionUrl(IVersion version)
		{
			return PathsManager.UriCombine(RemoteUrl, base.BuildsFolderName, string.Format(base.BuildDefinitionFileName, version));
		}

		public virtual string GetRemoteBuildUrl(IVersion version)
		{
			return PathsManager.UriCombine(RemoteUrl, base.BuildsFolderName, version.ToString());
		}

		public virtual string GetPartialRemoteBuildUrl(IVersion version)
		{
			return PathsManager.UriCombine(base.BuildsFolderName, version.ToString());
		}

		public virtual string GetRemotePatchesIndexUrl()
		{
			return PathsManager.UriCombine(RemoteUrl, base.PatchesFolderName, base.PatchesIndexFileName);
		}

		public virtual string GetRemotePatchDefinitionUrl(IVersion from, IVersion to)
		{
			return PathsManager.UriCombine(RemoteUrl, base.PatchesFolderName, string.Format(base.PatchDefinitionFileName, from, to));
		}

		public virtual string GetRemotePatchArchiveUrl(IVersion from, IVersion to)
		{
			return PathsManager.UriCombine(RemoteUrl, base.PatchesFolderName, string.Format(base.PatchFileName, from, to));
		}

		public virtual string GetRemoteUpdaterIndexUrl()
		{
			return PathsManager.UriCombine(RemoteUrl, base.UpdaterFolderName, base.UpdaterIndexFileName);
		}

		public virtual string GetRemoteUpdaterSafeModeIndexUrl()
		{
			return PathsManager.UriCombine(RemoteUrl, base.UpdaterFolderName, base.UpdaterSafeModeIndexFileName);
		}

		public virtual string GetRemoteUpdaterSafeModeArchiveUrl(string archiveName)
		{
			return PathsManager.UriCombine(RemoteUrl, base.UpdaterFolderName, archiveName);
		}

		public virtual string GetRemoteUpdaterFileUrl(string relativePath)
		{
			return PathsManager.UriCombine(RemoteUrl, base.UpdaterFolderName, relativePath);
		}

		public override string ToDebugString()
		{
			string text = base.ToDebugString();
			text = text + "Remote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\nRemote URL => " + RemoteUrl + "\n";
			return text + "GetVersionFilePath() => " + GetVersionFilePath() + "\nGetGamePath() => " + GetGamePath() + "\nGetTempPath() => " + GetTempPath() + "\nGetRemoteBuildsIndexUrl() => " + GetRemoteBuildsIndexUrl() + "\nGetRemotePatchesIndexUrl() => " + GetRemotePatchesIndexUrl() + "\nGetRemoteUpdaterIndexUrl() => " + GetRemoteUpdaterIndexUrl() + "\nGetRemoteUpdaterSafeModeIndexUrl() => " + GetRemoteUpdaterSafeModeIndexUrl() + "\n" + $"PatcherUpdaterSafeMode => {PatcherUpdaterSafeMode}\n";
		}
	}
}
