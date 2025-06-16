namespace Timer.RequestModels
{
	public class SaveGameTimeRequest
	{
		public string LicenseKey { get; set; }

		public int Minutes { get; set; }

		public string TimerStatus { get; set; }
	}
}
