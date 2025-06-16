using Settings;

namespace Logo
{
	public class LogoSettings : BaseResizableAndMovableSettings
	{
		public string FileNameWithExtension { get; set; }

		public override SettingsKey StorageKey => SettingsKey.Logo;

		public LogoSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			base.Width = 664.02f;
			base.Height = 117.37f;
			base.PositionX = 333f;
			base.PositionY = -60f;
		}
	}
}
