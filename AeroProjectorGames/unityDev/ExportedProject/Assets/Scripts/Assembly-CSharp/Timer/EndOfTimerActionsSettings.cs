using Settings;

namespace Timer
{
	public class EndOfTimerActionsSettings : BaseResizableAndMovableSettings
	{
		public EndOfTimerActionStates EndOfTimerAction { get; set; }

		public override SettingsKey StorageKey => SettingsKey.EndofTimerActions;

		public EndOfTimerActionsSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
		}
	}
}
