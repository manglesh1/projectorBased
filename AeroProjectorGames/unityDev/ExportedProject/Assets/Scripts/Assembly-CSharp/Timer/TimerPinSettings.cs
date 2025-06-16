using Settings;

namespace Timer
{
	public class TimerPinSettings : BaseResizableAndMovableSettings
	{
		public bool IsDisabled { get; set; } = true;

		public string TimerPin { get; set; } = string.Empty;

		public override SettingsKey StorageKey => SettingsKey.TimerPin;

		public TimerPinSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
		}
	}
}
