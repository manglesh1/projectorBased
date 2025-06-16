using Settings;

namespace Target
{
	public class TargetSettings : ISettings
	{
		public bool FiveFrameGame { get; set; }

		public bool PlayWinAnimations { get; set; } = true;

		public bool RotateTargets { get; set; } = true;

		public bool ShowKillZones { get; set; } = true;

		public bool ThrowsPerTurnEnabled { get; set; } = true;

		public bool UseShorterBattleshipAnimations { get; set; }

		public bool UseSixRingTarget { get; set; }

		public SettingsKey StorageKey => SettingsKey.Target;

		public void Save()
		{
			SettingsStore.Set(this);
		}
	}
}
