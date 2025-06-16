using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHLab.Patch.Core.Client.Exceptions;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.Client.Localization;
using MHLab.Patch.Core.Client.Progresses;
using MHLab.Patch.Core.Client.Runners;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Logging;
using MHLab.Patch.Core.Serializing;
using MHLab.Patch.Core.Utilities;
using MHLab.Patch.Core.Versioning;

namespace MHLab.Patch.Core.Client
{
	public class UpdatingContext
	{
		public readonly ILauncherSettings Settings;

		private readonly IProgress<UpdateProgress> _progressReporter;

		private UpdateProgress _progress;

		private bool _isDirty;

		private List<string> _dirtyReasons;

		private List<object> _dirtyData;

		private bool _isRepairNeeded;

		private SettingsOverride _currentSettingsOverride;

		public BuildsIndex BuildsIndex { get; set; }

		public PatchIndex PatchesIndex { get; set; }

		public LocalFileInfo[] ExistingFiles { get; set; }

		public Dictionary<string, LocalFileInfo> ExistingFilesMap { get; set; }

		public IVersion CurrentVersion { get; set; }

		private IVersionFactory VersionFactory { get; set; }

		public BuildDefinition CurrentBuildDefinition { get; set; }

		public List<PatchDefinition> PatchesPath { get; set; }

		public UpdaterDefinition CurrentUpdaterDefinition { get; set; }

		public IUpdateRunner Runner { get; set; }

		public IUpdaterLocalizedMessages LocalizedMessages { get; set; }

		public ILogger Logger { get; set; }

		public ISerializer Serializer { get; set; }

		public IDownloader Downloader { get; set; }

		public IFileSystem FileSystem { get; set; }

		public UpdatingContext(ILauncherSettings settings, IProgress<UpdateProgress> progress)
		{
			_isDirty = false;
			_dirtyReasons = new List<string>();
			_dirtyData = new List<object>();
			_isRepairNeeded = false;
			Settings = settings;
			_progressReporter = progress;
			PatchesPath = new List<PatchDefinition>();
			VersionFactory = new VersionFactory();
			FileSystem = new FileSystem();
			Runner = new UpdateRunner();
			Downloader = new FileDownloader(FileSystem);
		}

		public void Initialize()
		{
			Logger.Info("Update context initializing...", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 74L, "Initialize");
			Logger.Info("Software version: " + Settings.SoftwareVersion, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 75L, "Initialize");
			Logger.Info("Update context points to " + Settings.RemoteUrl, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 76L, "Initialize");
			Logger.Debug("===> Debug Mode: ENABLED <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 77L, "Initialize");
			_progress = new UpdateProgress();
			SetCurrentVersion();
			int num = CleanWorkspace();
			Logger.Info($"Workspace cleaned. Removed {num} files", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 84L, "Initialize");
			FileSystem.CreateDirectory((FilePath)Settings.GetTempPath());
			FetchIndexes();
			_progress.TotalSteps = Runner.GetProgressAmount();
			Logger.Info("Update context completed initialization.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 91L, "Initialize");
		}

		public void FetchIndexes()
		{
			Task.WaitAll(GetUpdaterDefinition(), GetBuildsIndex(), GetPatchesIndex());
			Task.WaitAll(GetLocalFiles(), GetBuildDefinition());
			Task.WaitAll(GetPatchesShortestPath());
		}

		public IVersion GetLocalVersion()
		{
			if (LocalVersionExists())
			{
				try
				{
					string data = Rijndael.Decrypt(FileSystem.ReadAllTextFromFile((FilePath)Settings.GetVersionFilePath()), Settings.EncryptionKeyphrase);
					IVersion obj = VersionFactory.Create();
					return Serializer.DeserializeOn(obj, data);
				}
				catch (Exception ex)
				{
					Logger.Debug("Cannot retrieve local version: " + ex.Message + " - " + ex.StackTrace, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 125L, "GetLocalVersion");
					return null;
				}
			}
			return null;
		}

		public bool LocalVersionExists()
		{
			return FileSystem.FileExists((FilePath)Settings.GetVersionFilePath());
		}

		public void Update()
		{
			Runner.Update();
		}

		public void RegisterUpdateStep(IUpdater updater)
		{
			Runner.RegisterStep(updater);
		}

		private void SetCurrentVersion()
		{
			if (LocalVersionExists())
			{
				try
				{
					string data = Rijndael.Decrypt(FileSystem.ReadAllTextFromFile((FilePath)Settings.GetVersionFilePath()), Settings.EncryptionKeyphrase);
					CurrentVersion = VersionFactory.Create();
					Serializer.DeserializeOn(CurrentVersion, data);
					Logger.Info($"Retrieved current version: {CurrentVersion}", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 161L, "SetCurrentVersion");
					return;
				}
				catch (Exception ex)
				{
					CurrentVersion = null;
					Logger.Warning("Current version file cannot be read. Error: " + ex.Message, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 166L, "SetCurrentVersion");
					return;
				}
			}
			CurrentVersion = null;
			Logger.Warning("No current version found. A full repair may be required.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 172L, "SetCurrentVersion");
		}

		private int CleanWorkspace()
		{
			return FileSystem.DeleteMultipleFiles((FilePath)Settings.RootPath, "*.delete_tmp");
		}

		private Task GetUpdaterDefinition()
		{
			return Task.Run(delegate
			{
				try
				{
					string remoteUpdaterIndexUrl = Settings.GetRemoteUpdaterIndexUrl();
					string text = Settings.GetRemoteUpdaterIndexUrl();
					if (!string.IsNullOrWhiteSpace(Settings.RemoteUrl))
					{
						text = text.Replace(Settings.RemoteUrl, string.Empty);
					}
					DownloadEntry entry = new DownloadEntry(remoteUpdaterIndexUrl, text, null, null, null);
					CurrentUpdaterDefinition = Downloader.DownloadJson<UpdaterDefinition>(entry, Serializer);
				}
				catch (Exception ex)
				{
					CurrentUpdaterDefinition = null;
					Logger.Warning("No updater definition found on remote server. The Launcher update will be skipped. Problem reference: " + ex.Message, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 204L, "GetUpdaterDefinition");
				}
			});
		}

		private Task GetBuildsIndex()
		{
			return Task.Run(delegate
			{
				try
				{
					string remoteBuildsIndexUrl = Settings.GetRemoteBuildsIndexUrl();
					string text = Settings.GetRemoteBuildsIndexUrl();
					if (!string.IsNullOrWhiteSpace(Settings.RemoteUrl))
					{
						text = text.Replace(Settings.RemoteUrl, string.Empty);
					}
					DownloadEntry entry = new DownloadEntry(remoteBuildsIndexUrl, text, null, null, null);
					BuildsIndex = Downloader.DownloadJson<BuildsIndex>(entry, Serializer);
				}
				catch (Exception arg)
				{
					BuildsIndex = new BuildsIndex
					{
						AvailableBuilds = new List<IVersion>()
					};
					Logger.Warning($"No builds index found on remote server. The Repair process will be skipped. Problem reference: {arg}", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 236L, "GetBuildsIndex");
				}
			});
		}

		private Task GetPatchesIndex()
		{
			return Task.Run(delegate
			{
				try
				{
					string remotePatchesIndexUrl = Settings.GetRemotePatchesIndexUrl();
					string text = Settings.GetRemotePatchesIndexUrl();
					if (!string.IsNullOrWhiteSpace(Settings.RemoteUrl))
					{
						text = text.Replace(Settings.RemoteUrl, string.Empty);
					}
					DownloadEntry entry = new DownloadEntry(remotePatchesIndexUrl, text, null, null, null);
					PatchesIndex = Downloader.DownloadJson<PatchIndex>(entry, Serializer);
				}
				catch
				{
					PatchesIndex = new PatchIndex
					{
						Patches = new List<PatchIndexEntry>()
					};
					Logger.Warning("No patches index found on the remote server. The Update process will be skipped.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 268L, "GetPatchesIndex");
				}
			});
		}

		private Task GetLocalFiles()
		{
			return Task.Run(delegate
			{
				FileSystem.GetFilesInfo((FilePath)Settings.RootPath, out var fileInfo, out var fileInfoMap);
				ExistingFiles = fileInfo;
				ExistingFilesMap = fileInfoMap;
				Logger.Info($"Collected information on {ExistingFiles.Length} local files.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 280L, "GetLocalFiles");
			});
		}

		private Task GetBuildDefinition()
		{
			return Task.Run(delegate
			{
				if (CurrentVersion == null)
				{
					CurrentVersion = BuildsIndex.GetLast();
				}
				if (CurrentVersion == null)
				{
					Logger.Error(null, "Cannot retrieve any new version...", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 295L, "GetBuildDefinition");
					throw new NoAvailableBuildsException();
				}
				if (!BuildsIndex.Contains(CurrentVersion) && CurrentVersion.IsLower(BuildsIndex.GetFirst()))
				{
					CurrentVersion = BuildsIndex.GetLast();
					SetRepairNeeded();
				}
				try
				{
					CurrentBuildDefinition = GetBuildDefinition(CurrentVersion);
					Logger.Info($"Retrieved definition for {CurrentVersion}", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 308L, "GetBuildDefinition");
				}
				catch
				{
					CurrentBuildDefinition = new BuildDefinition
					{
						Entries = new BuildDefinitionEntry[0]
					};
					Logger.Warning($"Cannot retrieve the build definition for {CurrentVersion} on the remote server.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 316L, "GetBuildDefinition");
				}
			});
		}

		public BuildDefinition GetBuildDefinition(IVersion version)
		{
			string remoteBuildDefinitionUrl = Settings.GetRemoteBuildDefinitionUrl(version);
			string text = Settings.GetRemoteBuildDefinitionUrl(version);
			if (!string.IsNullOrWhiteSpace(Settings.RemoteUrl))
			{
				text = text.Replace(Settings.RemoteUrl, string.Empty);
			}
			DownloadEntry entry = new DownloadEntry(remoteBuildDefinitionUrl, text, null, null, null);
			return Downloader.DownloadJson<BuildDefinition>(entry, Serializer);
		}

		private Task GetPatchesShortestPath()
		{
			return Task.Run(delegate
			{
				IVersion version = CurrentVersion;
				List<PatchIndexEntry> list;
				do
				{
					IVersion version2 = version;
					list = PatchesIndex.Patches.Where((PatchIndexEntry p) => p.From.Equals(version2)).ToList();
					if (list.Count != 0)
					{
						PatchIndexEntry patchIndexEntry = list.OrderBy((PatchIndexEntry p) => p.To).Last();
						string remotePatchDefinitionUrl = Settings.GetRemotePatchDefinitionUrl(patchIndexEntry.From, patchIndexEntry.To);
						string text = Settings.GetRemotePatchDefinitionUrl(patchIndexEntry.From, patchIndexEntry.To);
						if (!string.IsNullOrWhiteSpace(Settings.RemoteUrl))
						{
							text = text.Replace(Settings.RemoteUrl, string.Empty);
						}
						DownloadEntry entry = new DownloadEntry(remotePatchDefinitionUrl, text, null, null, null);
						PatchesPath.Add(Downloader.DownloadJson<PatchDefinition>(entry, Serializer));
						version = patchIndexEntry.To;
					}
				}
				while (list.Count > 0);
				Logger.Info($"Found {PatchesPath.Count} applicable updates.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\UpdatingContext.cs", 367L, "GetPatchesShortestPath");
			});
		}

		public void ReportProgress(string log, long increment)
		{
			_progress.IncrementStep(increment);
			_progress.StepMessage = log;
			_progressReporter.Report(new UpdateProgress
			{
				CurrentSteps = _progress.CurrentSteps,
				StepMessage = log,
				TotalSteps = _progress.TotalSteps
			});
		}

		public void ReportProgress(long increment)
		{
			_progress.IncrementStep(increment);
			_progressReporter.Report(new UpdateProgress
			{
				CurrentSteps = _progress.CurrentSteps,
				StepMessage = _progress.StepMessage,
				TotalSteps = _progress.TotalSteps
			});
		}

		public void LogProgress(string log)
		{
			_progress.StepMessage = log;
			_progressReporter.Report(new UpdateProgress
			{
				CurrentSteps = _progress.CurrentSteps,
				StepMessage = log,
				TotalSteps = _progress.TotalSteps
			});
		}

		public void SetDirtyFlag(string reason, object data = null)
		{
			_dirtyReasons.Add(reason);
			if (data != null)
			{
				_dirtyData.Add(data);
			}
			_isDirty = true;
		}

		public bool IsDirty(out List<string> reasons, out List<object> data)
		{
			reasons = _dirtyReasons;
			data = _dirtyData;
			return _isDirty;
		}

		public void SetRepairNeeded()
		{
			_isRepairNeeded = true;
		}

		public bool IsRepairNeeded()
		{
			return _isRepairNeeded;
		}

		public void OverrideSettings<TSettings>(Action<ILauncherSettings, TSettings> settingsOverrider) where TSettings : SettingsOverride
		{
			FilePath settingsOverridePath = Settings.GetSettingsOverridePath();
			if (FileSystem.FileExists(settingsOverridePath))
			{
				string data = FileSystem.ReadAllTextFromFile(settingsOverridePath);
				settingsOverrider(arg2: (TSettings)(_currentSettingsOverride = Serializer.Deserialize<TSettings>(data)), arg1: Settings);
			}
		}

		public void DisableSafeMode()
		{
			SettingsOverride currentSettingsOverride = _currentSettingsOverride;
			currentSettingsOverride.PatcherUpdaterSafeMode = false;
			FilePath settingsOverridePath = Settings.GetSettingsOverridePath();
			FileSystem.DeleteFile(settingsOverridePath);
			FileSystem.WriteAllTextToFile(settingsOverridePath, Serializer.Serialize(currentSettingsOverride));
		}
	}
}
