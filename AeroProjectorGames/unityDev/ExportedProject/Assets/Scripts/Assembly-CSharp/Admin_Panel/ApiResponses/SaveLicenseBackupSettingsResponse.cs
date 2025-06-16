using Admin_Panel.Models;

namespace Admin_Panel.ApiResponses
{
	public class SaveLicenseBackupSettingsResponse
	{
		public LicenseBackupSettingsModel Data { get; set; }

		public bool Success { get; set; }

		public string Reason { get; set; }
	}
}
