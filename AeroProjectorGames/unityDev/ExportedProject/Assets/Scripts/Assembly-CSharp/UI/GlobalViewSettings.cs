using Settings;

namespace UI
{
	public class GlobalViewSettings : ISettings
	{
		public const float DEFAULT_SIZE = 5f;

		public float OrthographicSize { get; set; } = 5f;

		public bool PortraitMode { get; set; }

		public SettingsKey StorageKey => SettingsKey.GlobalView;

		public void Save()
		{
			SettingsStore.Set(this);
		}
	}
}
