using System.Collections.Generic;
using Admin_Panel;
using Backgrounds;
using Detection;
using Games;
using Games.IATF.Ring_Target.Scripts;
using Interaction;
using Logo;
using MainMenu;
using Players;
using ResizingAndMoving;
using Scoreboard;
using Target;
using Timer;
using UI;
using UnityEngine;

namespace Settings
{
	public class SettingsStore
	{
		private readonly SettingsRepository _repo = new SettingsRepository();

		private static SettingsStore _instance = new SettingsStore();

		private readonly Dictionary<string, object> _settings;

		public static AdminSettings Admin => Get<AdminSettings>(SettingsKey.Admin) ?? new AdminSettings();

		public static DetectionSettings DetectionSettings => Get<DetectionSettings>(SettingsKey.HitDetection) ?? new DetectionSettings();

		public static EndOfTimerActionsSettings EndofTimerActions => Get<EndOfTimerActionsSettings>(SettingsKey.EndofTimerActions) ?? new EndOfTimerActionsSettings();

		public static FontSettingsStore Font => Get<FontSettingsStore>(SettingsKey.Font) ?? new FontSettingsStore();

		public static GameAccessSettings GameAccessVisibility => Get<GameAccessSettings>(SettingsKey.GameAccess) ?? new GameAccessSettings();

		public static GameboardSettings Gameboard => Get<GameboardSettings>(SettingsKey.Gameboard) ?? new GameboardSettings();

		public static BackgroundSettings Backgrounds => Get<BackgroundSettings>(SettingsKey.Backgrounds) ?? new BackgroundSettings();

		public static GlobalViewSettings GlobalViewSettings => Get<GlobalViewSettings>(SettingsKey.GlobalView) ?? new GlobalViewSettings();

		public static IATFTargetSettings IATFTarget => Get<IATFTargetSettings>(SettingsKey.IATFTarget) ?? new IATFTargetSettings();

		public static InteractionSettings Interaction => Get<InteractionSettings>(SettingsKey.InteractionSettings) ?? new InteractionSettings();

		public static LogoSettings Logo => Get<LogoSettings>(SettingsKey.Logo) ?? new LogoSettings();

		public static MainMenuSettings MainMenu => Get<MainMenuSettings>(SettingsKey.MainMenu) ?? new MainMenuSettings();

		public static PlayerNameSettings PlayerNames => Get<PlayerNameSettings>(SettingsKey.PlayerNames) ?? new PlayerNameSettings();

		public static SafetyVideoSettings SafetyVideo => Get<SafetyVideoSettings>(SettingsKey.SafetyVideo) ?? new SafetyVideoSettings();

		public static ScoreboardSettings Scoreboard => Get<ScoreboardSettings>(SettingsKey.Scoreboard) ?? new ScoreboardSettings();

		public static SizeAndPositionPanelSettings SizeAndPositionPanel => Get<SizeAndPositionPanelSettings>(SettingsKey.SizeAndPositionPanel) ?? new SizeAndPositionPanelSettings();

		public static TargetSettings Target => Get<TargetSettings>(SettingsKey.Target) ?? new TargetSettings();

		public static TargetColorSettings TargetColor => Get<TargetColorSettings>(SettingsKey.TargetColor) ?? new TargetColorSettings();

		public static TimerSettings Timer => Get<TimerSettings>(SettingsKey.Timer) ?? new TimerSettings();

		public static TimerPinSettings TimerPin => Get<TimerPinSettings>(SettingsKey.TimerPin) ?? new TimerPinSettings();

		public static VersionSettings Version => Get<VersionSettings>(SettingsKey.Version) ?? new VersionSettings();

		private Dictionary<string, object> Settings => _settings;

		private SettingsRepository Repo => _repo;

		private SettingsStore()
		{
			_settings = _repo.GetSettings();
		}

		private static T Get<T>(SettingsKey key) where T : ISettings
		{
			T result = default(T);
			string key2 = key.ToString();
			if (_instance.Settings.ContainsKey(key2))
			{
				try
				{
					result = (T)_instance.Settings[key2];
					return result;
				}
				catch
				{
					Debug.Log("Removing from the key");
					_instance.Settings.Remove(key2);
				}
			}
			return result;
		}

		public static void Reload()
		{
			_instance = new SettingsStore();
		}

		public static void Set(ISettings settings)
		{
			string key = settings.StorageKey.ToString();
			if (_instance.Settings.ContainsKey(key))
			{
				_instance.Settings.Remove(key);
			}
			_instance.Settings.Add(key, settings);
			_instance.Repo.Save(_instance.Settings);
		}
	}
}
