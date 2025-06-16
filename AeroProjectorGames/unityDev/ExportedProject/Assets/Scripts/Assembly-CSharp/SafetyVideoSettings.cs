using Settings;

public class SafetyVideoSettings : BaseResizableAndMovableSettings
{
	public string FileLocationAndName { get; set; }

	public bool IsDisabled { get; set; }

	public override SettingsKey StorageKey => SettingsKey.SafetyVideo;

	public SafetyVideoSettings()
	{
		SetDefaults();
	}

	public override void SetDefaults()
	{
		base.Width = 1256f;
		base.Height = 942f;
		base.PositionX = 0f;
		base.PositionY = 0f;
	}
}
