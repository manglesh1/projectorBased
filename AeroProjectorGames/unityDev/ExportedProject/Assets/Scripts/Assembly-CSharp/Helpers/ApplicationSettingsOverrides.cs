using System.IO;
using Newtonsoft.Json;

namespace Helpers
{
	public class ApplicationSettingsOverrides
	{
		private static ApplicationSettingsOverrides _instance;

		public static ApplicationSettingsOverrides Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = LoadData();
				}
				return _instance;
			}
		}

		public string ApiUrl { get; set; }

		public float HoursToWaitBetweenUpdates { get; set; }

		private ApplicationSettingsOverrides()
		{
			ApplicationSettingsOverrides instance = _instance;
		}

		private static ApplicationSettingsOverrides LoadData()
		{
			ApplicationSettingsOverrides result = new ApplicationSettingsOverrides();
			if (File.Exists(DataPathHelpers.GetOverrideSettingsFilePath()))
			{
				try
				{
					StreamReader streamReader = new StreamReader(DataPathHelpers.GetOverrideSettingsFilePath());
					string value = streamReader.ReadToEnd();
					streamReader.Dispose();
					result = JsonConvert.DeserializeObject<ApplicationSettingsOverrides>(value);
				}
				catch
				{
				}
			}
			return result;
		}
	}
}
