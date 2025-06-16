using System;
using System.IO;
using MHLab.Patch.Core;
using MHLab.Patch.Core.Client;
using MHLab.Patch.Core.Client.Advanced.IO.Chunked;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Launcher.Scripts.Utilities;
using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts
{
	public sealed class Launcher : LauncherBase
	{
		private Repairer _repairer;

		private Updater _updater;

		private bool _alreadyTriggeredGameStart;

		protected override string UpdateProcessName => "Game updating";

		protected override void Initialize(UpdatingContext context)
		{
			context.Logger.Info("Launcher initializing...", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Launcher.cs", 47L, "Initialize");
			context.OverrideSettings(delegate(ILauncherSettings originalSettings, SettingsOverride settingsOverride)
			{
				originalSettings.DebugMode = settingsOverride.DebugMode;
				originalSettings.PatcherUpdaterSafeMode = settingsOverride.PatcherUpdaterSafeMode;
				originalSettings.RemoteUrl = settingsOverride.RemoteUrl;
			});
			context.Downloader = new ChunkedDownloader(context);
			context.Downloader.DownloadComplete += Data.DownloadComplete;
			NetworkChecker = new NetworkChecker();
			_repairer = new Repairer(context);
			_updater = new Updater(context);
			context.RegisterUpdateStep(_repairer);
			context.RegisterUpdateStep(_updater);
			context.Runner.PerformedStep += delegate
			{
				if (context.IsDirty(out var reasons, out var data))
				{
					string text = "";
					foreach (string item in reasons)
					{
						text = text + item + ", ";
					}
					text = text.Substring(0, text.Length - 2);
					context.Logger.Debug("Context is set to dirty: updater restart required. The files " + text + " have been replaced.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Launcher.cs", 52L, "Initialize");
					if (data.Count > 0 && data[0] is UpdaterSafeModeDefinition)
					{
						UpdaterSafeModeDefinition updaterSafeModeDefinition = (UpdaterSafeModeDefinition)data[0];
						UpdateRestartNeeded(updaterSafeModeDefinition.ExecutableToRun);
					}
					else
					{
						UpdateRestartNeeded();
					}
				}
			};
		}

		protected override void OverrideSettings(ILauncherSettings settings)
		{
			string empty = string.Empty;
			empty = Directory.GetParent(Application.dataPath).FullName;
			settings.RootPath = FilesManager.SanitizePath(empty);
		}

		protected override void UpdateStarted()
		{
			Data.StartTimer(UpdateDownloadSpeed);
		}

		protected override void UpdateDownloadSpeed()
		{
			Context.Downloader.DownloadSpeedMeter.Tick();
			if (Context.Downloader.DownloadSpeedMeter.DownloadSpeed > 0)
			{
				Data.DownloadSpeed.text = Context.Downloader.DownloadSpeedMeter.FormattedDownloadSpeed;
			}
			else
			{
				Data.DownloadSpeed.text = string.Empty;
			}
		}

		protected override void UpdateCompleted()
		{
			Data.Log(Context.LocalizedMessages.UpdateProcessCompleted);
			Context.Logger.Info("===> [" + UpdateProcessName + "] process COMPLETED! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Launcher.cs", 111L, "UpdateCompleted");
			Data.Dispatcher.Invoke(delegate
			{
				Data.ProgressBar.Progress = 1f;
				Data.ProgressPercentage.text = "100%";
			});
			EnsureExecutePrivileges(PathsManager.Combine(Context.Settings.GetGamePath(), Data.GameExecutableName));
			EnsureExecutePrivileges(PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName));
			Data.Dispatcher.Invoke(delegate
			{
				Invoke("StartGame", 1.5f);
			});
		}

		protected override void UpdateFailed(Exception e)
		{
			Data.Log(Context.LocalizedMessages.UpdateProcessFailed);
			Context.Logger.Error(e, "===> [" + UpdateProcessName + "] process FAILED! <=== - " + e.Message + " - " + e.StackTrace, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Launcher.cs", 131L, "UpdateFailed");
			if (Data.LaunchAnywayOnError)
			{
				StartGame();
			}
		}

		protected override void UpdateRestartNeeded(string executableName = "")
		{
			Data.Log(Context.LocalizedMessages.UpdateRestartNeeded);
			Context.Logger.Info("===> [" + UpdateProcessName + "] process INCOMPLETE: restart is needed! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Launcher.cs", 142L, "UpdateRestartNeeded");
			string text = (string.IsNullOrWhiteSpace(executableName) ? PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName) : PathsManager.Combine(Context.Settings.RootPath, executableName));
			try
			{
				ApplicationStarter.StartApplication(Path.Combine(Context.Settings.RootPath, Data.LauncherExecutableName), "");
				Data.Dispatcher.Invoke(Application.Quit);
			}
			catch (Exception e)
			{
				Context.Logger.Error(null, "Unable to start the Launcher at " + text + ".", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Launcher.cs", 164L, "UpdateRestartNeeded");
				UpdateFailed(e);
			}
		}

		protected override void StartApp()
		{
			StartGame();
		}

		private void StartGame()
		{
			if (!_alreadyTriggeredGameStart)
			{
				_alreadyTriggeredGameStart = true;
				ApplicationStarter.StartApplication(PathsManager.Combine(Context.Settings.GetGamePath(), Data.GameExecutableName), Context.Settings.LaunchArgumentParameter + "=" + Context.Settings.LaunchArgumentValue);
				Application.Quit();
			}
		}

		public void GenerateDebugReport()
		{
			GenerateDebugReport("debug_report_launcher.txt");
		}

		private void OnDestroy()
		{
			Context.Downloader.Cancel();
			Debug.Log("Download canceled");
		}
	}
}
