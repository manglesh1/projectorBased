using System.Collections.Generic;
using Settings;

namespace Players
{
	public class PlayerNameSettings : BaseResizableAndMovableSettings
	{
		public List<string> CustomPlayerNames { get; set; } = new List<string>();

		public override SettingsKey StorageKey => SettingsKey.PlayerNames;

		public override void SetDefaults()
		{
		}
	}
}
