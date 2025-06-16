using System;
using System.IO;
using MHLab.Patch.Core.Client;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Launcher.Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MHLab.Patch.Launcher.Scripts
{
	public sealed class LauncherUpdater : LauncherBase
	{
		public int SceneToLoad;

		private PatcherUpdater _patcherUpdater;

		protected override string UpdateProcessName => "Launcher updating";

		protected override void Initialize(UpdatingContext context)
		{
			context.Logger.Info("LauncherUpdater initialize", "C:\\AxcitementApp\\ilspycode\\Assembly-CSharp\\MHLab\\Patch\\Launcher\\Scripts\\LauncherUpdater.cs", 21L, "Initialize");
		}

		protected override void OverrideSettings(ILauncherSettings settings)
		{
			string empty = string.Empty;
			empty = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
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
			Data.Dispatcher.Invoke(delegate
			{
				Data.ProgressBar.Progress = 1f;
				Data.ProgressPercentage.text = "100%";
			});
			Repairer repairer = new Repairer(Context);
			Updater updater = new Updater(Context);
			if (repairer.IsRepairNeeded() || updater.IsUpdateAvailable())
			{
				UpdateRestartNeeded();
				return;
			}
			Data.Log(Context.LocalizedMessages.UpdateProcessCompleted);
			Context.Logger.Info("===> [" + UpdateProcessName + "] process COMPLETED! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\LauncherUpdater.cs", 125L, "UpdateCompleted");
			StartGameScene();
		}

		protected override void StartApp()
		{
			StartGameScene();
		}

		private void StartGameScene()
		{
			Data.Dispatcher.Invoke(delegate
			{
				Context.Logger.Info("loading scene", "C:\\AxcitementApp\\ilspycode\\Assembly-CSharp\\MHLab\\Patch\\Launcher\\Scripts\\LauncherUpdater.cs", 124L, "StartGameScene");
				SceneManager.LoadScene(1);
			});
		}

		protected override void UpdateFailed(Exception e)
		{
			Data.Log(Context.LocalizedMessages.UpdateProcessFailed);
			Context.Logger.Error(e, "===> [" + UpdateProcessName + "] process FAILED! <=== - " + e.Message + " - " + e.StackTrace, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\LauncherUpdater.cs", 145L, "UpdateFailed");
			if (Data.LaunchAnywayOnError)
			{
				StartGameScene();
			}
		}

		protected override void UpdateRestartNeeded(string executableName = "")
		{
			Data.Log(Context.LocalizedMessages.UpdateRestartNeeded);
			Context.Logger.Info("===> [" + UpdateProcessName + "] process INCOMPLETE: restart is needed! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\LauncherUpdater.cs", 156L, "UpdateRestartNeeded");
			EnsureExecutePrivileges(PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName));
			string text = (string.IsNullOrWhiteSpace(executableName) ? PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName) : PathsManager.Combine(Context.Settings.RootPath, executableName));
			try
			{
				ApplicationStarter.StartApplication(Path.Combine(Context.Settings.RootPath, Data.LauncherExecutableName), "");
				Data.Dispatcher.Invoke(Application.Quit);
			}
			catch (Exception e)
			{
				Context.Logger.Error(null, "Unable to start the Launcher at " + text + ".", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\LauncherUpdater.cs", 180L, "UpdateRestartNeeded");
				UpdateFailed(e);
			}
		}

		public void GenerateDebugReport()
		{
			GenerateDebugReport("debug_report_pregame.txt");
		}

		private void OnDisable()
		{
			Context.Downloader.Cancel();
		}
	}
}
