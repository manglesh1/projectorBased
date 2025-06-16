using System;
using System.IO;
using System.Threading.Tasks;
using MHLab.Patch.Core.Client;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.Logging;
using MHLab.Patch.Core.Utilities;
using MHLab.Patch.Launcher.Scripts.Localization;
using MHLab.Patch.Launcher.Scripts.Utilities;
using MHLab.Patch.Utilities;
using MHLab.Patch.Utilities.Serializing;
using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts
{
	public abstract class LauncherBase : MonoBehaviour
	{
		public LauncherData Data;

		protected UpdatingContext Context;

		protected INetworkChecker NetworkChecker;

		protected abstract string UpdateProcessName { get; }

		private ILauncherSettings CreateSettings()
		{
			LauncherSettings launcherSettings = new LauncherSettings();
			launcherSettings.RemoteUrl = Data.RemoteUrl;
			launcherSettings.PatchDownloadAttempts = 3;
			launcherSettings.AppDataPath = Application.persistentDataPath;
			launcherSettings.DebugMode = false;
			Data.DebugMode = true;
			OverrideSettings(launcherSettings);
			return launcherSettings;
		}

		protected abstract void OverrideSettings(ILauncherSettings settings);

		private UpdatingContext CreateContext(ILauncherSettings settings)
		{
			ProgressReporter progressReporter = new ProgressReporter();
			progressReporter.ProgressChanged.AddListener(Data.UpdateProgressChanged);
			UpdatingContext updatingContext = new UpdatingContext(settings, progressReporter);
			updatingContext.Logger = new SimpleLogger(updatingContext.FileSystem, settings.GetLogsFilePath(), settings.DebugMode);
			updatingContext.Logger.Info("starting custom", "C:\\AxcitementApp\\ilspycode\\Assembly-CSharp\\MHLab\\Patch\\Launcher\\Scripts\\LauncherBase.cs", 49L, "CreateContext");
			updatingContext.Serializer = new JsonSerializer();
			updatingContext.LocalizedMessages = new EnglishUpdaterLocalizedMessages();
			return updatingContext;
		}

		private void Initialize(ILauncherSettings settings)
		{
			Context = CreateContext(settings);
			if (Data.SoftwareVersion != null)
			{
				Data.SoftwareVersion.text = "v" + settings.SoftwareVersion;
			}
			Initialize(Context);
		}

		protected abstract void Initialize(UpdatingContext context);

		protected void GenerateDebugReport(string path)
		{
			string systemInfo = DebugHelper.GetSystemInfo();
			string contents = Debugger.GenerateDebugReport(Context.Settings, systemInfo, Context.Serializer);
			File.WriteAllText(path, contents);
		}

		private void Awake()
		{
			Initialize(CreateSettings());
		}

		private void Start()
		{
			Context.Logger.Info("LauncherBase Start", "C:\\AxcitementApp\\ilspycode\\Assembly-CSharp\\MHLab\\Patch\\Launcher\\Scripts\\LauncherBase.cs", 88L, "Start");
			OnStart();
		}

		protected virtual void OnStart()
		{
			Context.Logger.Info("LauncherBase OnStart " + Context.Settings.GetLogsDirectoryPath(), "C:\\AxcitementApp\\ilspycode\\Assembly-CSharp\\MHLab\\Patch\\Launcher\\Scripts\\LauncherBase.cs", 95L, "OnStart");
			StartApp();
		}

		protected void StartUpdateProcess()
		{
			try
			{
				Context.Logger.Info("===> [" + UpdateProcessName + "] process STARTED! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\LauncherBase.cs", 122L, "StartUpdateProcess");
				if (!CheckForNetworkAvailability())
				{
					if (Data.LaunchAnywayOnError)
					{
						StartApp();
					}
					else
					{
						Data.Dialog.ShowCloseDialog(Context.LocalizedMessages.NotAvailableNetwork, string.Empty, Application.Quit);
					}
				}
				else if (!CheckForRemoteServiceAvailability())
				{
					if (Data.LaunchAnywayOnError)
					{
						StartApp();
					}
					else
					{
						Data.Dialog.ShowCloseDialog(Context.LocalizedMessages.NotAvailableServers, string.Empty, Application.Quit);
					}
				}
				else
				{
					Context.Initialize();
					Task.Run((Action)CheckForUpdates);
				}
			}
			catch (Exception e)
			{
				UpdateFailed(e);
				if (Data.LaunchAnywayOnError)
				{
					StartApp();
				}
			}
		}

		protected bool CheckForNetworkAvailability()
		{
			if (!NetworkChecker.IsNetworkAvailable())
			{
				Data.Log(Context.LocalizedMessages.NotAvailableNetwork);
				Context.Logger.Error(null, "[" + UpdateProcessName + "] process FAILED! Network is not available or connectivity is low/weak... Check your connection!", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\LauncherBase.cs", 176L, "CheckForNetworkAvailability");
				return false;
			}
			return true;
		}

		protected bool CheckForRemoteServiceAvailability()
		{
			if (!NetworkChecker.IsRemoteServiceAvailable(Context.Settings.GetRemoteBuildsIndexUrl(), out var exception))
			{
				Data.Log(Context.LocalizedMessages.NotAvailableServers);
				Context.Logger.Error(exception, "[" + UpdateProcessName + "] process FAILED! Our servers are not responding... Wait some minutes and retry!", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\LauncherBase.cs", 191L, "CheckForRemoteServiceAvailability");
				return false;
			}
			return true;
		}

		protected void CheckForUpdates()
		{
			UpdateStarted();
			try
			{
				Context.Update();
				UpdateCompleted();
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
				UpdateFailed(ex);
			}
			finally
			{
				Data.StopTimer();
			}
		}

		protected abstract void UpdateStarted();

		protected abstract void UpdateCompleted();

		protected abstract void UpdateFailed(Exception e);

		protected abstract void UpdateRestartNeeded(string executableName = "");

		protected abstract void UpdateDownloadSpeed();

		protected abstract void StartApp();

		protected void EnsureExecutePrivileges(string filePath)
		{
			try
			{
				PrivilegesSetter.EnsureExecutePrivileges(filePath);
			}
			catch (Exception exception)
			{
				Context.Logger.Error(exception, "Unable to set executing privileges on {FilePath}.", filePath, 242L, "EnsureExecutePrivileges");
			}
		}
	}
}
