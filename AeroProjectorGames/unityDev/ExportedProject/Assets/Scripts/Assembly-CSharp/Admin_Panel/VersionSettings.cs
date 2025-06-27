using System.IO;
using Helpers;
using MHLab.Patch.Core;
using MHLab.Patch.Core.Utilities;
using Settings;

namespace Admin_Panel
{
	public class VersionSettings : Settings.ISettings
	{
		private string _versionNumber;

		public string VersionNumber
		{
			get
			{
				if (string.IsNullOrEmpty(_versionNumber))
				{
					SetVersionFromMHPatch();
				}
				return _versionNumber;
			}
			private set
			{
				_versionNumber = value;
			}
		}

		public SettingsKey StorageKey => SettingsKey.Version;

		public VersionSettings()
		{
			SetVersionFromMHPatch();
			Save();
		}

		public void Save()
		{
			SettingsStore.Set(this);
		}

		private void SetVersionFromMHPatch()
		{
			MHLab.Patch.Core.Settings settings = new MHLab.Patch.Core.Settings();
			string path = Path.Combine(DataPathHelpers.GetRootPath(), settings.VersionFileName);
			if (!File.Exists(path))
			{
				return;
			}
			using StreamReader streamReader = new StreamReader(path);
			VersionNumber = Rijndael.Decrypt(streamReader.ReadToEnd(), settings.EncryptionKeyphrase).Replace("\"", "");
		}
	}
}
