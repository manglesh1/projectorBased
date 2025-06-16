using Settings;

namespace Timer
{
	public class TimerSettings : BaseResizableAndMovableSettings
	{
		public int AdditionalTimeIncrement1 { get; set; } = 5;

		public int AdditionalTimeIncrement2 { get; set; } = 10;

		public int AdditionalTimeIncrement3 { get; set; } = 15;

		public bool IsDisabled { get; set; }

		public int GameLength1 { get; set; } = 15;

		public int GameLength2 { get; set; } = 30;

		public int GameLength3 { get; set; } = 90;

		public int Pin { get; set; }

		public bool PinIsDisabled { get; set; } = true;

		public bool SendTimeToDashboard { get; set; }

		public EndOfTimerActionStates EndOfTimerActionState { get; set; }

		public override SettingsKey StorageKey => SettingsKey.Timer;

		public TimerSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			base.Width = 254f;
			base.Height = 173f;
			base.PositionX = -130f;
			base.PositionY = -87f;
		}
	}
}
