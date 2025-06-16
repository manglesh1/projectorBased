using System;
using System.IO;

namespace MHLab.Patch.Core.IO
{
	public static class DirectoriesManager
	{
		public static void Create(string path)
		{
			Directory.CreateDirectory(path);
		}

		public static string GetCurrentDirectory()
		{
			return Directory.GetCurrentDirectory();
		}

		public static bool IsEmpty(string path)
		{
			if (!Directory.Exists(path))
			{
				throw new DirectoryNotFoundException();
			}
			string[] directories = Directory.GetDirectories(path);
			string[] files = Directory.GetFiles(path);
			return directories.Length == 0 && files.Length == 0;
		}

		public static void Copy(string sourceFolder, string destFolder)
		{
			Create(destFolder);
			string[] files = Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories);
			foreach (string text in files)
			{
				string text2 = text.Replace(sourceFolder, destFolder);
				Create(Path.GetDirectoryName(text2));
				File.Copy(text, text2);
			}
		}

		public static bool Delete(string directory)
		{
			try
			{
				Clean(directory);
				Directory.Delete(directory);
				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void Clean(string path)
		{
			try
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				DirectoryInfo directoryInfo = new DirectoryInfo(path);
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					fileInfo.Attributes &= ~FileAttributes.ReadOnly;
					fileInfo.Delete();
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				for (int j = 0; j < directories.Length; j++)
				{
					DeleteRecursiveFolder(directories[j].FullName);
				}
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private static void DeleteRecursiveFolder(string pFolderPath)
		{
			string[] directories = Directory.GetDirectories(pFolderPath);
			for (int i = 0; i < directories.Length; i++)
			{
				DeleteRecursiveFolder(directories[i]);
			}
			string[] files = Directory.GetFiles(pFolderPath);
			foreach (string path in files)
			{
				FileInfo fileInfo = new FileInfo(Path.Combine(pFolderPath, path));
				File.SetAttributes(fileInfo.FullName, FileAttributes.Normal);
				File.Delete(fileInfo.FullName);
			}
			Directory.Delete(pFolderPath);
		}
	}
}
