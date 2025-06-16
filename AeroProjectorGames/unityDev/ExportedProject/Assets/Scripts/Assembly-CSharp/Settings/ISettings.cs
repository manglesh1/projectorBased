namespace Settings
{
	public interface ISettings
	{
		SettingsKey StorageKey { get; }

		void Save();
	}
}
