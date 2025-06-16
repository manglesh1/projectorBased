using System;
using System.IO;
using System.Threading.Tasks;
using MHLab.Patch.Core;
using MHLab.Patch.Core.Client;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Launcher.Scripts.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace MHLab.Patch.Launcher.Scripts
{
	public class BackgroundLauncherUpdater : LauncherBase
	{
		public GameObject DebugSection;

		public float TimeBetweenEachCheckInSeconds = 60f;

		public UnityEvent RestartNeeded;

		private PatcherUpdater _patcherUpdater;

		private float _timeFromLastCheck;

		private bool _isPreviousUpdateCompleted;

		protected override string UpdateProcessName => "Background Launcher Updating";

		protected override void OverrideSettings(ILauncherSettings settings)
		{
			string empty = string.Empty;
			string fullName = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
			settings.RootPath = FilesManager.SanitizePath(fullName);
		}

		protected override void Initialize(UpdatingContext context)
		{
			context.Logger.Info("===> [" + UpdateProcessName + "] process STARTED! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 78L, "Initialize");
			context.OverrideSettings(delegate(ILauncherSettings originalSettings, SettingsOverride settingsOverride)
			{
				originalSettings.DebugMode = settingsOverride.DebugMode;
				originalSettings.PatcherUpdaterSafeMode = settingsOverride.PatcherUpdaterSafeMode;
				originalSettings.RemoteUrl = settingsOverride.RemoteUrl;
			});
			Context.Downloader.DownloadComplete += Data.DownloadComplete;
			NetworkChecker = new NetworkChecker();
			_patcherUpdater = new PatcherUpdater(context);
			context.RegisterUpdateStep(_patcherUpdater);
			context.Runner.PerformedStep += delegate
			{
				if (context.IsDirty(out var reasons, out var data))
				{
					string text = "";
					foreach (string item in reasons)
					{
						text = text + item + ", ";
					}
					context.Logger.Debug("Context is set to dirty: updater restart required. The files " + text.Substring(0, text.Length - 2) + " have been replaced.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 78L, "Initialize");
					if (data.Count > 0 && data[0] is UpdaterSafeModeDefinition)
					{
						UpdateRestartNeeded(((UpdaterSafeModeDefinition)data[0]).ExecutableToRun);
					}
					else
					{
						UpdateRestartNeeded();
					}
				}
			};
			_timeFromLastCheck = TimeBetweenEachCheckInSeconds;
			_isPreviousUpdateCompleted = true;
			Data.ProgressBar.gameObject.SetActive(value: false);
		}

		private void CheckForDebugInfoEnabling()
		{
			Context.Settings.DebugMode = Data.DebugMode;
			DebugSection.SetActive(Context.Settings.DebugMode && Data.ProgressBar.gameObject.activeSelf);
		}

		private void Update()
		{
			CheckForDebugInfoEnabling();
			_timeFromLastCheck += Time.deltaTime;
			if (!((double)_timeFromLastCheck < (double)TimeBetweenEachCheckInSeconds))
			{
				if (_isPreviousUpdateCompleted)
				{
					Task.Run((Action)StartCheckingForLauncherUpdates);
				}
				_timeFromLastCheck = 0f;
			}
		}

		protected override void OnStart()
		{
		}

		private void StartCheckingForLauncherUpdates()
		{
			_isPreviousUpdateCompleted = false;
			if (FilesManager.IsDirectoryWritable(Context.Settings.GetLogsDirectoryPath()))
			{
				try
				{
					Context.Logger.Info("===> [" + UpdateProcessName + "] process STARTED! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 136L, "StartCheckingForLauncherUpdates");
					if (!CheckForNetworkAvailability())
					{
						Data.Dispatcher.Invoke(delegate
						{
							Data.Dialog.ShowCloseDialog(Context.LocalizedMessages.NotAvailableNetwork, string.Empty, delegate
							{
								Data.Dialog.CloseDialog();
							});
						});
					}
					else if (!CheckForRemoteServiceAvailability())
					{
						Data.Dispatcher.Invoke(delegate
						{
							Data.Dialog.ShowCloseDialog(Context.LocalizedMessages.NotAvailableServers, string.Empty, delegate
							{
								Data.Dialog.CloseDialog();
							});
						});
					}
					else
					{
						Context.Initialize();
						if (!_patcherUpdater.IsUpdateAvailable())
						{
							_isPreviousUpdateCompleted = true;
							Data.Dispatcher.Invoke(delegate
							{
								Data.ProgressBar.gameObject.SetActive(value: false);
							});
						}
						else
						{
							Task.Run((Action)base.CheckForUpdates);
						}
					}
					return;
				}
				catch (Exception e)
				{
					UpdateFailed(e);
					return;
				}
			}
			Data.Dispatcher.Invoke(delegate
			{
				Data.Log(Context.LocalizedMessages.LogsFileNotWritable);
				Data.Dialog.ShowDialog(Context.LocalizedMessages.LogsFileNotWritable, Context.Settings.GetLogsFilePath(), delegate
				{
					Data.Dialog.CloseDialog();
				}, delegate
				{
					Data.Dialog.CloseDialog();
				});
				Data.ProgressBar.gameObject.SetActive(value: false);
			});
			Context.Logger.Error(null, "Updating process FAILED! The Launcher has not enough privileges to write into its folder!", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 194L, "StartCheckingForLauncherUpdates");
			_isPreviousUpdateCompleted = true;
		}

		protected override void UpdateStarted()
		{
			_isPreviousUpdateCompleted = false;
			Data.Dispatcher.Invoke(delegate
			{
				Data.StartTimer(UpdateDownloadSpeed);
				Data.ProgressBar.gameObject.SetActive(value: true);
			});
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
			Data.Dispatcher.Invoke(delegate
			{
				Data.Log(Context.LocalizedMessages.UpdateProcessCompleted);
			});
			Context.Logger.Info("===> [" + UpdateProcessName + "] process COMPLETED! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 229L, "UpdateCompleted");
			_isPreviousUpdateCompleted = true;
		}

		protected override void UpdateFailed(Exception e)
		{
			Data.Dispatcher.Invoke(delegate
			{
				Data.Log(Context.LocalizedMessages.UpdateProcessFailed);
				Data.Dialog.ShowDialog(Context.LocalizedMessages.UpdateProcessFailed, e.Message, delegate
				{
					Data.Dialog.CloseDialog();
				}, delegate
				{
					Data.Dialog.CloseDialog();
				});
				Data.ProgressBar.gameObject.SetActive(value: false);
				Debug.LogException(e);
			});
			Context.Logger.Error(e, "===> [" + UpdateProcessName + "] process FAILED! <=== - " + e.Message + " - " + e.StackTrace, "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 249L, "UpdateFailed");
			_isPreviousUpdateCompleted = true;
		}

		protected override void UpdateRestartNeeded(string executableName = "")
		{
			Data.Dispatcher.Invoke(delegate
			{
				Data.Log(Context.LocalizedMessages.UpdateRestartNeeded);
			});
			Context.Logger.Info("===> [" + UpdateProcessName + "] process INCOMPLETE: restart is needed! <===", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 257L, "UpdateRestartNeeded");
			EnsureExecutePrivileges(PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName));
			string filePath = (string.IsNullOrWhiteSpace(executableName) ? PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName) : PathsManager.Combine(Context.Settings.RootPath, executableName));
			Data.Dispatcher.Invoke(delegate
			{
				Data.Dialog.ShowDialog("Pending update!", Context.LocalizedMessages.UpdateRestartNeeded, delegate
				{
					Data.Dialog.CloseDialog();
				}, delegate
				{
					try
					{
						ApplicationStarter.StartApplication(Path.Combine(Context.Settings.RootPath, Data.LauncherExecutableName), "");
						Data.Dispatcher.Invoke(Application.Quit);
					}
					catch (Exception e)
					{
						Context.Logger.Error(null, "Unable to start the Launcher at " + filePath + ".", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\BackgroundLauncherUpdater.cs", 292L, "UpdateRestartNeeded");
						UpdateFailed(e);
					}
				});
				RestartNeeded?.Invoke();
			});
			_isPreviousUpdateCompleted = false;
		}

		protected override void UpdateDownloadSpeed()
		{
			Context.Downloader.DownloadSpeedMeter.Tick();
			if (Context.Downloader.DownloadSpeedMeter.DownloadSpeed > 0)
			{
				Data.Dispatcher.Invoke(delegate
				{
					Data.DownloadSpeed.text = Context.Downloader.DownloadSpeedMeter.FormattedDownloadSpeed;
				});
			}
			else
			{
				Data.Dispatcher.Invoke(delegate
				{
					Data.DownloadSpeed.text = string.Empty;
				});
			}
		}

		protected override void StartApp()
		{
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
