using System;
using System.IO;

namespace MHLab.Patch.Core.IO
{
	public static class PathsManager
	{
		public static string GetDirectoryPath(string path)
		{
			return Path.GetDirectoryName(path);
		}

		public static string GetDirectoryParent(string path)
		{
			return Directory.GetParent(path).FullName;
		}

		public static string GetFilename(string path)
		{
			return Path.GetFileName(path);
		}

		public static string Combine(params string[] paths)
		{
			return Path.Combine(paths);
		}

		public static string Combine(string path1, string path2)
		{
			return Path.Combine(path1, path2);
		}

		public static string UriCombine(params string[] uriParts)
		{
			string text = string.Empty;
			if (uriParts != null && uriParts.Length != 0)
			{
				char[] trimChars = new char[2] { '\\', '/' };
				text = (uriParts[0] ?? string.Empty).TrimEnd(trimChars);
				for (int i = 1; i < uriParts.Length; i++)
				{
					text = $"{text.TrimEnd(trimChars)}/{(uriParts[i] ?? string.Empty).TrimStart(trimChars)}";
				}
			}
			return text;
		}

		public static string GetSpecialPath(Environment.SpecialFolder specialFolder)
		{
			return Environment.GetFolderPath(specialFolder);
		}
	}
}
