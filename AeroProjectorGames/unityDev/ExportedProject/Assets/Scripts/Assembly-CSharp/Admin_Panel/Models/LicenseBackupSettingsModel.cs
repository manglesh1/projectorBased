using System;

namespace Admin_Panel.Models
{
	public class LicenseBackupSettingsModel
	{
		public string LicenseKey { get; set; }

		public DateTime LastModifiedDateTime { get; set; }

		public string SettingsData { get; set; }
	}
}
