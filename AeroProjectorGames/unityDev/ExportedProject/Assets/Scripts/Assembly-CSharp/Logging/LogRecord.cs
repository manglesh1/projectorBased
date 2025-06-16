using System;

namespace Logging
{
	[Serializable]
	public class LogRecord
	{
		public string IpAddress { get; set; }

		public int LaneNumber { get; set; }

		public DateTime LicenseExpiration { get; set; }

		public string LicenseKey { get; set; }

		public string MacAddress { get; set; }

		public string Message { get; set; }

		public string ProcessName { get; set; }

		public int SeverityId { get; set; }
	}
}
