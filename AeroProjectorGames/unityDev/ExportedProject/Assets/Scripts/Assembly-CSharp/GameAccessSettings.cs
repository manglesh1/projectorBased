using System.Collections.Generic;
using Settings;

public class GameAccessSettings : BaseResizableAndMovableSettings
{
	public Dictionary<int, bool> UserVisibleGameDictionary { get; set; } = new Dictionary<int, bool>();

	public List<int> DetectionEnabledForGameId { get; set; } = new List<int>();

	public override SettingsKey StorageKey => SettingsKey.GameAccess;

	public GameAccessSettings()
	{
		SetDefaults();
	}

	public override void SetDefaults()
	{
	}
}
