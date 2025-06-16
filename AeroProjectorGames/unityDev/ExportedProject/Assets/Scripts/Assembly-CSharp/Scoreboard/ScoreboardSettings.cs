using System;
using Settings;

namespace Scoreboard
{
	[Serializable]
	public class ScoreboardSettings : BaseResizableAndMovableSettings
	{
		public override SettingsKey StorageKey => SettingsKey.Scoreboard;

		public ScoreboardSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			base.Width = 1385f;
			base.Height = 350f;
			base.PositionX = 1.7f;
			base.PositionY = -134f;
		}
	}
}
