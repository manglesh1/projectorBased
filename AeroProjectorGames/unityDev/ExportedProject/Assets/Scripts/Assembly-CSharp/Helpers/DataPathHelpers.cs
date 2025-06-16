using System;
using System.IO;
using UnityEngine;

namespace Helpers
{
	public static class DataPathHelpers
	{
		private const string LAUNCHER_FILE_NAME = "launcher.x86_64";

		private const string WINDOWS_LAUNCHER_FILE_NAME = "launcher.exe";

		private const string FILE_PATH_DIRECTORY_NAME = "Managed";

		private const string CUSTOM_BACKGROUNDS_FOLDER = "CustomBackgrounds";

		private const string OLD_LICENSE_FILE = "appsettings.json";

		private const string OVERRIDE_SETTINGS_FILE = "settings.json";

		public static string GetApplicationDataPath()
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		}

		public static string GetCustomImagesDirectory()
		{
			return GetDirectoryInPersistentDataPath("CustomBackgrounds", createIfNotExists: true);
		}

		public static string GetManagedFilePath(string dataPath, string fileName)
		{
			return Path.Combine(GetAndCreateManagedDirectory(dataPath), fileName);
		}

		private static string GetDirectoryInPersistentDataPath(string dir, bool createIfNotExists = false)
		{
			string text = Path.Combine(Application.persistentDataPath, dir);
			if (createIfNotExists && !Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		private static string GetAndCreateManagedDirectory(string dataPath)
		{
			string text = Path.Combine(dataPath, "Managed");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string GetLauncherPath()
		{
			return Path.Combine(Directory.GetParent(GetRootPath()).FullName, GetLauncherFilename());
		}

		public static string GetLegacySettingsFilePath()
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "appsettings.json");
			if (!File.Exists(text))
			{
				return string.Empty;
			}
			return text;
		}

		public static string GetOverrideSettingsFilePath()
		{
			return Path.Combine(Directory.GetParent(Application.dataPath).FullName, "settings.json");
		}

		public static string GetRootPath()
		{
			return Directory.GetParent(Application.dataPath).FullName;
		}

		private static string GetLauncherFilename()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.WindowsPlayer:
				return "launcher.exe";
			case RuntimePlatform.LinuxPlayer:
				return "launcher.x86_64";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
