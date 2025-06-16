using System.Collections.Generic;
using Backgrounds.TargetDimmer;
using Settings;

namespace Backgrounds
{
	public class BackgroundSettings : BaseResizableAndMovableSettings
	{
		public override SettingsKey StorageKey => SettingsKey.Backgrounds;

		public List<BackgroundSetting> Backgrounds { get; set; } = new List<BackgroundSetting>();

		public Dictionary<int, string> GameBackgrounds { get; set; } = new Dictionary<int, string>();

		public TargetDimmerSettings TargetDimmer { get; set; } = new TargetDimmerSettings();

		public BackgroundSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			base.Width = 1f;
			base.Height = 1f;
			base.PositionX = 0f;
			base.PositionY = 0f;
		}
	}
}
