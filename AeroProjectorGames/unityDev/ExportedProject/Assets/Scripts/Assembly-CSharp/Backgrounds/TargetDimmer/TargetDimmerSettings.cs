using System;
using Settings;

namespace Backgrounds.TargetDimmer
{
	[Serializable]
	public class TargetDimmerSettings : BaseResizableAndMovableSettings
	{
		public float Alpha { get; set; }

		public override SettingsKey StorageKey { get; } = SettingsKey.MainMenu;

		public TargetDimmerSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			Alpha = 0f;
			base.Height = 1f;
			base.Width = 1f;
			base.PositionX = 0f;
			base.PositionY = 0f;
		}
	}
}
