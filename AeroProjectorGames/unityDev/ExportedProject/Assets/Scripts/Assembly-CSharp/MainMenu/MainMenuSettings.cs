using Settings;

namespace MainMenu
{
	public class MainMenuSettings : BaseResizableAndMovableSettings
	{
		public bool UseDemo { get; set; }

		public int WaitBeforeDemoTimeInMinutes { get; set; } = 30;

		public bool ShowWeeklyFreeTrial { get; set; } = true;

		public override SettingsKey StorageKey => SettingsKey.MainMenu;

		public MainMenuSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			base.Width = 1f;
			base.Height = 1f;
			base.PositionX = 0f;
			base.PositionY = -218.85f;
		}
	}
}
