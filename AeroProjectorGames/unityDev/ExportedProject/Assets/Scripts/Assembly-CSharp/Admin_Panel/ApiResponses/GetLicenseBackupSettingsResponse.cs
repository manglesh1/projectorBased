using Admin_Panel.Models;

namespace Admin_Panel.ApiResponses
{
	public class GetLicenseBackupSettingsResponse
	{
		public LicenseBackupSettingsModel Data { get; set; }

		public bool Success { get; set; }

		public string Reason { get; set; }
	}
}
