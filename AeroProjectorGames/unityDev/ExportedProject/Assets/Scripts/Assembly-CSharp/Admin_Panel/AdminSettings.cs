using Settings;

namespace Admin_Panel
{
	public class AdminSettings : ISettings
	{
		public const string ADMIN_PIN = "1234";

		public string Pin { get; set; }

		public SettingsKey StorageKey => SettingsKey.Admin;

		public AdminSettings()
		{
			SetDefaults();
		}

		public void Save()
		{
			SettingsStore.Set(this);
		}

		private void SetDefaults()
		{
			Pin = "1234";
		}
	}
}
