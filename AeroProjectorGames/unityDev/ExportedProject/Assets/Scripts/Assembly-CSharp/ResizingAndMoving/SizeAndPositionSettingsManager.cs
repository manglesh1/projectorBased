using System;
using Games;
using Logo;
using MainMenu;
using Scoreboard;
using Settings;
using Timer;
using UnityEngine;
using UnityEngine.Events;

namespace ResizingAndMoving
{
	public class SizeAndPositionSettingsManager : MonoBehaviour
	{
		private GameboardSettings _gameboardSettings;

		private LogoSettings _logoSettings;

		private MainMenuSettings _mainMenuSettings;

		private SafetyVideoSettings _safetyVideoSettings;

		private ScoreboardSettings _scoreboardSettings;

		private SizeAndPositionPanelSettings _sizeAndPositionPanelSettings;

		private TimerSettings _timerSettings;

		[Header("Events")]
		[SerializeField]
		private SettingsEventsSO settingEvents;

		[Header("Main Camera")]
		[SerializeField]
		private Camera mainCamera;

		[Header("State Components")]
		[SerializeField]
		private SizeAndPositionStateSO detectionSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO gameboardSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO globalBackgroundState;

		[SerializeField]
		private SizeAndPositionStateSO logoSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO mainMenuSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO safetyVideoSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO scoreboardSizeAndPositionState;

		[SerializeField]
		private SizeAndPositionStateSO sizeAndPositionPanelState;

		[SerializeField]
		private SizeAndPositionStateSO timerSizeAndPositionState;

		private void Awake()
		{
			LoadAllSettings();
			SizeAndPositionStateSO sizeAndPositionStateSO = detectionSizeAndPositionState;
			sizeAndPositionStateSO.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO.OnDoneEditing, new UnityAction(SaveDetectionSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO2 = gameboardSizeAndPositionState;
			sizeAndPositionStateSO2.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO2.OnDoneEditing, new UnityAction(SaveGameboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO3 = globalBackgroundState;
			sizeAndPositionStateSO3.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO3.OnDoneEditing, new UnityAction(SaveGlobalBackgroundSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO4 = logoSizeAndPositionState;
			sizeAndPositionStateSO4.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO4.OnDoneEditing, new UnityAction(SaveLogoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO5 = mainMenuSizeAndPositionState;
			sizeAndPositionStateSO5.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO5.OnDoneEditing, new UnityAction(SaveMainMenuSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO6 = safetyVideoSizeAndPositionState;
			sizeAndPositionStateSO6.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO6.OnDoneEditing, new UnityAction(SaveSafetyVideoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO7 = scoreboardSizeAndPositionState;
			sizeAndPositionStateSO7.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO7.OnDoneEditing, new UnityAction(SaveScoreboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO8 = sizeAndPositionPanelState;
			sizeAndPositionStateSO8.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO8.OnDoneEditing, new UnityAction(SaveSizeAndPositionPanelSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO9 = timerSizeAndPositionState;
			sizeAndPositionStateSO9.OnDoneEditing = (UnityAction)Delegate.Combine(sizeAndPositionStateSO9.OnDoneEditing, new UnityAction(SaveTimerSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO10 = detectionSizeAndPositionState;
			sizeAndPositionStateSO10.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO10.OnReset, new UnityAction(ResetDetectionSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO11 = gameboardSizeAndPositionState;
			sizeAndPositionStateSO11.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO11.OnReset, new UnityAction(ResetGameboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO12 = globalBackgroundState;
			sizeAndPositionStateSO12.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO12.OnReset, new UnityAction(ResetGlobalBackground));
			SizeAndPositionStateSO sizeAndPositionStateSO13 = logoSizeAndPositionState;
			sizeAndPositionStateSO13.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO13.OnReset, new UnityAction(ResetLogoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO14 = mainMenuSizeAndPositionState;
			sizeAndPositionStateSO14.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO14.OnReset, new UnityAction(ResetMainMenuSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO15 = safetyVideoSizeAndPositionState;
			sizeAndPositionStateSO15.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO15.OnReset, new UnityAction(ResetSafetyVideoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO16 = scoreboardSizeAndPositionState;
			sizeAndPositionStateSO16.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO16.OnReset, new UnityAction(ResetScoreboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO17 = sizeAndPositionPanelState;
			sizeAndPositionStateSO17.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO17.OnReset, new UnityAction(ResetSizeAndPositionPanelSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO18 = timerSizeAndPositionState;
			sizeAndPositionStateSO18.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO18.OnReset, new UnityAction(ResetTimerSettings));
			settingEvents.OnSettingsReloaded += SettingsReloaded;
		}

		private void OnDestroy()
		{
			SizeAndPositionStateSO sizeAndPositionStateSO = detectionSizeAndPositionState;
			sizeAndPositionStateSO.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO.OnDoneEditing, new UnityAction(SaveDetectionSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO2 = gameboardSizeAndPositionState;
			sizeAndPositionStateSO2.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO2.OnDoneEditing, new UnityAction(SaveGameboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO3 = globalBackgroundState;
			sizeAndPositionStateSO3.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO3.OnDoneEditing, new UnityAction(SaveGlobalBackgroundSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO4 = logoSizeAndPositionState;
			sizeAndPositionStateSO4.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO4.OnDoneEditing, new UnityAction(SaveLogoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO5 = mainMenuSizeAndPositionState;
			sizeAndPositionStateSO5.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO5.OnDoneEditing, new UnityAction(SaveMainMenuSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO6 = safetyVideoSizeAndPositionState;
			sizeAndPositionStateSO6.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO6.OnDoneEditing, new UnityAction(SaveSafetyVideoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO7 = scoreboardSizeAndPositionState;
			sizeAndPositionStateSO7.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO7.OnDoneEditing, new UnityAction(SaveScoreboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO8 = sizeAndPositionPanelState;
			sizeAndPositionStateSO8.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO8.OnDoneEditing, new UnityAction(SaveSizeAndPositionPanelSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO9 = timerSizeAndPositionState;
			sizeAndPositionStateSO9.OnDoneEditing = (UnityAction)Delegate.Remove(sizeAndPositionStateSO9.OnDoneEditing, new UnityAction(SaveTimerSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO10 = detectionSizeAndPositionState;
			sizeAndPositionStateSO10.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO10.OnReset, new UnityAction(ResetDetectionSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO11 = gameboardSizeAndPositionState;
			sizeAndPositionStateSO11.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO11.OnReset, new UnityAction(ResetGameboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO12 = globalBackgroundState;
			sizeAndPositionStateSO12.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO12.OnReset, new UnityAction(ResetGlobalBackground));
			SizeAndPositionStateSO sizeAndPositionStateSO13 = logoSizeAndPositionState;
			sizeAndPositionStateSO13.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO13.OnReset, new UnityAction(ResetLogoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO14 = mainMenuSizeAndPositionState;
			sizeAndPositionStateSO14.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO14.OnReset, new UnityAction(ResetMainMenuSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO15 = safetyVideoSizeAndPositionState;
			sizeAndPositionStateSO15.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO15.OnReset, new UnityAction(ResetSafetyVideoSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO16 = scoreboardSizeAndPositionState;
			sizeAndPositionStateSO16.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO16.OnReset, new UnityAction(ResetScoreboardSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO17 = sizeAndPositionPanelState;
			sizeAndPositionStateSO17.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO17.OnReset, new UnityAction(ResetSizeAndPositionPanelSettings));
			SizeAndPositionStateSO sizeAndPositionStateSO18 = timerSizeAndPositionState;
			sizeAndPositionStateSO18.OnReset = (UnityAction)Delegate.Remove(sizeAndPositionStateSO18.OnReset, new UnityAction(ResetTimerSettings));
			settingEvents.OnSettingsReloaded -= SettingsReloaded;
		}

		private void ResetDetectionSettings()
		{
			SettingsStore.DetectionSettings.SetDefaults();
			SetSizeAndPosition(SettingsStore.DetectionSettings, detectionSizeAndPositionState);
		}

		private void ResetGameboardSettings()
		{
			_gameboardSettings.SetDefaults();
			SetSizeAndPosition(_gameboardSettings, gameboardSizeAndPositionState);
		}

		private void ResetGlobalBackground()
		{
			SettingsStore.Backgrounds.SetDefaults();
			SetSizeAndPosition(SettingsStore.Backgrounds, globalBackgroundState);
		}

		private void ResetLogoSettings()
		{
			_logoSettings.SetDefaults();
			SetSizeAndPosition(_logoSettings, logoSizeAndPositionState);
		}

		private void ResetMainMenuSettings()
		{
			_mainMenuSettings.SetDefaults();
			SetSizeAndPosition(_mainMenuSettings, mainMenuSizeAndPositionState);
		}

		private void ResetSafetyVideoSettings()
		{
			_safetyVideoSettings.SetDefaults();
			SetSizeAndPosition(_safetyVideoSettings, safetyVideoSizeAndPositionState);
		}

		private void ResetScoreboardSettings()
		{
			_scoreboardSettings.SetDefaults();
			SetSizeAndPosition(_scoreboardSettings, scoreboardSizeAndPositionState);
		}

		private void ResetSizeAndPositionPanelSettings()
		{
			_sizeAndPositionPanelSettings.SetDefaults();
			SetSizeAndPosition(_sizeAndPositionPanelSettings, sizeAndPositionPanelState);
		}

		private void ResetTimerSettings()
		{
			_timerSettings.SetDefaults();
			SetSizeAndPosition(_timerSettings, timerSizeAndPositionState);
		}

		private void SaveDetectionSettings()
		{
			SettingsStore.DetectionSettings.Height = detectionSizeAndPositionState.Height;
			SettingsStore.DetectionSettings.Width = detectionSizeAndPositionState.Width;
			SettingsStore.DetectionSettings.PositionX = detectionSizeAndPositionState.PositionX;
			SettingsStore.DetectionSettings.PositionY = detectionSizeAndPositionState.PositionY;
			SettingsStore.DetectionSettings.Save();
		}

		private void SaveGameboardSettings()
		{
			_gameboardSettings.Height = gameboardSizeAndPositionState.Height;
			_gameboardSettings.Width = gameboardSizeAndPositionState.Width;
			_gameboardSettings.PositionX = gameboardSizeAndPositionState.PositionX;
			_gameboardSettings.PositionY = gameboardSizeAndPositionState.PositionY;
			SettingsStore.Set(_gameboardSettings);
		}

		private void SaveGlobalBackgroundSettings()
		{
			SettingsStore.Backgrounds.Height = globalBackgroundState.Height;
			SettingsStore.Backgrounds.Width = globalBackgroundState.Width;
			SettingsStore.Backgrounds.PositionX = globalBackgroundState.PositionX;
			SettingsStore.Backgrounds.PositionY = globalBackgroundState.PositionY;
			SettingsStore.Backgrounds.Save();
		}

		private void SaveLogoSettings()
		{
			_logoSettings.Height = logoSizeAndPositionState.Height;
			_logoSettings.Width = logoSizeAndPositionState.Width;
			_logoSettings.PositionX = logoSizeAndPositionState.PositionX;
			_logoSettings.PositionY = logoSizeAndPositionState.PositionY;
			SettingsStore.Set(_logoSettings);
		}

		private void SaveMainMenuSettings()
		{
			_mainMenuSettings.Height = mainMenuSizeAndPositionState.Height;
			_mainMenuSettings.Width = mainMenuSizeAndPositionState.Width;
			_mainMenuSettings.PositionX = mainMenuSizeAndPositionState.PositionX;
			_mainMenuSettings.PositionY = mainMenuSizeAndPositionState.PositionY;
			SettingsStore.Set(_mainMenuSettings);
		}

		private void SaveSafetyVideoSettings()
		{
			_safetyVideoSettings.Height = safetyVideoSizeAndPositionState.Height;
			_safetyVideoSettings.Width = safetyVideoSizeAndPositionState.Width;
			_safetyVideoSettings.PositionX = safetyVideoSizeAndPositionState.PositionX;
			_safetyVideoSettings.PositionY = safetyVideoSizeAndPositionState.PositionY;
			SettingsStore.Set(_safetyVideoSettings);
		}

		private void SaveScoreboardSettings()
		{
			_scoreboardSettings.Height = scoreboardSizeAndPositionState.Height;
			_scoreboardSettings.Width = scoreboardSizeAndPositionState.Width;
			_scoreboardSettings.PositionX = scoreboardSizeAndPositionState.PositionX;
			_scoreboardSettings.PositionY = scoreboardSizeAndPositionState.PositionY;
			SettingsStore.Set(_scoreboardSettings);
		}

		private void SaveSizeAndPositionPanelSettings()
		{
			_sizeAndPositionPanelSettings.Height = sizeAndPositionPanelState.Height;
			_sizeAndPositionPanelSettings.Width = sizeAndPositionPanelState.Width;
			_sizeAndPositionPanelSettings.PositionX = sizeAndPositionPanelState.PositionX;
			_sizeAndPositionPanelSettings.PositionY = sizeAndPositionPanelState.PositionY;
			SettingsStore.Set(_sizeAndPositionPanelSettings);
		}

		private void SaveTimerSettings()
		{
			_timerSettings.Height = timerSizeAndPositionState.Height;
			_timerSettings.Width = timerSizeAndPositionState.Width;
			_timerSettings.PositionX = timerSizeAndPositionState.PositionX;
			_timerSettings.PositionY = timerSizeAndPositionState.PositionY;
			SettingsStore.Set(_timerSettings);
		}

		private void SettingsReloaded()
		{
			LoadAllSettings();
		}

		private void LoadAllSettings()
		{
			LoadDetectionSizeAndPosition();
			LoadGameboardSizeAndPosition();
			LoadGlobalBackgroundSizeAndPosition();
			LoadLogoSizeAndPosition();
			LoadMainMenuSizeAndPosition();
			LoadSafetyVideoSizeAndPosition();
			LoadScoreboardSizeAndPosition();
			LoadSizeAndPositionPanelSizeAndPosition();
			LoadTimerSizeAndPosition();
		}

		private void LoadDetectionSizeAndPosition()
		{
			SetSizeAndPosition(SettingsStore.DetectionSettings, detectionSizeAndPositionState);
		}

		private void LoadGameboardSizeAndPosition()
		{
			_gameboardSettings = SettingsStore.Gameboard;
			SetSizeAndPosition(_gameboardSettings, gameboardSizeAndPositionState);
		}

		private void LoadGlobalBackgroundSizeAndPosition()
		{
			SetSizeAndPosition(SettingsStore.Backgrounds, globalBackgroundState);
		}

		private void LoadGlobalViewSettings()
		{
			mainCamera.orthographicSize = SettingsStore.GlobalViewSettings.OrthographicSize;
		}

		private void LoadLogoSizeAndPosition()
		{
			_logoSettings = SettingsStore.Logo;
			SetSizeAndPosition(_logoSettings, logoSizeAndPositionState);
		}

		private void LoadMainMenuSizeAndPosition()
		{
			_mainMenuSettings = SettingsStore.MainMenu;
			SetSizeAndPosition(_mainMenuSettings, mainMenuSizeAndPositionState);
		}

		private void LoadSafetyVideoSizeAndPosition()
		{
			_safetyVideoSettings = SettingsStore.SafetyVideo;
			SetSizeAndPosition(_safetyVideoSettings, safetyVideoSizeAndPositionState);
		}

		private void LoadScoreboardSizeAndPosition()
		{
			_scoreboardSettings = SettingsStore.Scoreboard;
			SetSizeAndPosition(_scoreboardSettings, scoreboardSizeAndPositionState);
		}

		private void LoadSizeAndPositionPanelSizeAndPosition()
		{
			_sizeAndPositionPanelSettings = SettingsStore.SizeAndPositionPanel;
			SetSizeAndPosition(_sizeAndPositionPanelSettings, sizeAndPositionPanelState);
		}

		private void LoadTimerSizeAndPosition()
		{
			_timerSettings = SettingsStore.Timer;
			SetSizeAndPosition(_timerSettings, timerSizeAndPositionState);
		}

		private void SetSizeAndPosition(BaseResizableAndMovableSettings settings, SizeAndPositionStateSO sizeAndPositionState)
		{
			sizeAndPositionState.SetSize(settings.Width, settings.Height);
			sizeAndPositionState.SetPosition(settings.PositionX, settings.PositionY);
			SettingsStore.Set(settings);
		}
	}
}
