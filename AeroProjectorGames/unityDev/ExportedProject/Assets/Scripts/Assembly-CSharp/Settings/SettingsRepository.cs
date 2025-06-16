using System.Collections.Generic;
using System.IO;
using Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace Settings
{
	public class SettingsRepository
	{
		private const string FILE_NAME = "settings.json";

		private string _filePath;

		private JsonSerializerSettings _serializerSettings;

		public SettingsRepository()
		{
			_filePath = DataPathHelpers.GetManagedFilePath(Application.persistentDataPath, "settings.json");
			_serializerSettings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				Error = HandleDeserializationErrors
			};
		}

		private void HandleDeserializationErrors(object obj, Newtonsoft.Json.Serialization.ErrorEventArgs args)
		{
			args.ErrorContext.Handled = true;
		}

		public string GetRawSettingsFile()
		{
			return File.ReadAllText(_filePath);
		}

		public Dictionary<string, object> GetSettings()
		{
			Dictionary<string, object> dictionary = null;
			if (File.Exists(_filePath))
			{
				dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(GetRawSettingsFile(), _serializerSettings);
			}
			return dictionary ?? new Dictionary<string, object>();
		}

		public void Save(Dictionary<string, object> settings)
		{
			File.WriteAllText(_filePath, JsonConvert.SerializeObject(settings, _serializerSettings));
		}

		public void Overwrite(string rawSettingsData)
		{
			if ((JsonConvert.DeserializeObject<Dictionary<string, object>>(rawSettingsData, _serializerSettings)?.Count ?? 0) > 0)
			{
				File.WriteAllText(_filePath, rawSettingsData);
			}
		}
	}
}
