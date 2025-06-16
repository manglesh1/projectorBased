using Settings;

public class FontSettingsStore : ISettings
{
	private const int DEFAULT_FONT_SIZE = 28;

	public int FontSize { get; set; } = 28;

	public string FontType { get; set; } = "Roboto Black SDF";

	public int DefaultFontSize => 28;

	public SettingsKey StorageKey => SettingsKey.Font;

	public void Save()
	{
		SettingsStore.Set(this);
	}
}
