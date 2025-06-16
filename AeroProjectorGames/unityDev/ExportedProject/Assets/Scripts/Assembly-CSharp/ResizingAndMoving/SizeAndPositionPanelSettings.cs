using Settings;

namespace ResizingAndMoving
{
	public class SizeAndPositionPanelSettings : BaseResizableAndMovableSettings
	{
		public override SettingsKey StorageKey => SettingsKey.SizeAndPositionPanel;

		public SizeAndPositionPanelSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			base.Width = 1f;
			base.Height = 1f;
			base.PositionX = 38f;
			base.PositionY = -659f;
		}
	}
}
