using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MHLab.Patch.Core.IO
{
	public static class FilesManager
	{
		public static bool Exists(string path)
		{
			return File.Exists(path);
		}

		public static string[] GetFiles(string path, string pattern = "*")
		{
			return (from f in Directory.GetFiles(path, pattern, SearchOption.AllDirectories)
				where !IsFileOsRelated(f)
				select f).ToArray();
		}

		private static bool IsFileOsRelated(string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			return fileName == ".DS_Store" || fileName == "desktop.ini";
		}

		public static LocalFileInfo[] GetFilesInfo(string rootPath)
		{
			string[] files = GetFiles(rootPath);
			LocalFileInfo[] array = new LocalFileInfo[files.Length];
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				FileInfo fileInfo = new FileInfo(text);
				array[i] = new LocalFileInfo
				{
					Size = fileInfo.Length,
					LastWriting = fileInfo.LastWriteTimeUtc,
					Attributes = fileInfo.Attributes,
					RelativePath = SanitizeToRelativePath(rootPath, text)
				};
			}
			return array;
		}

		public static void GetFilesInfo(string rootPath, out LocalFileInfo[] fileInfoArray, out Dictionary<string, LocalFileInfo> fileInfoMap)
		{
			string[] files = GetFiles(rootPath);
			fileInfoArray = new LocalFileInfo[files.Length];
			fileInfoMap = new Dictionary<string, LocalFileInfo>();
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				FileInfo fileInfo = new FileInfo(text);
				LocalFileInfo localFileInfo = new LocalFileInfo();
				localFileInfo.Size = fileInfo.Length;
				localFileInfo.LastWriting = fileInfo.LastWriteTime;
				localFileInfo.Attributes = fileInfo.Attributes;
				localFileInfo.RelativePath = SanitizeToRelativePath(rootPath, text);
				fileInfoArray[i] = localFileInfo;
				fileInfoMap.Add(localFileInfo.RelativePath, localFileInfo);
			}
		}

		public static LocalFileInfo GetFileInfo(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			return new LocalFileInfo
			{
				Size = fileInfo.Length,
				LastWriting = fileInfo.LastWriteTimeUtc,
				Attributes = fileInfo.Attributes,
				RelativePath = SanitizeToRelativePath(PathsManager.GetDirectoryPath(filePath), filePath)
			};
		}

		public static string SanitizeToRelativePath(string rootPath, string fullPath)
		{
			if (string.IsNullOrWhiteSpace(rootPath))
			{
				return fullPath;
			}
			fullPath = SanitizePath(fullPath);
			rootPath = SanitizePath(rootPath);
			string text = SanitizePath(fullPath.Replace(rootPath, string.Empty));
			if (text.StartsWith("/"))
			{
				text = text.Substring(1);
			}
			return text;
		}

		public static string SanitizePath(string path)
		{
			return path.Replace('\\', '/');
		}

		public static void Delete(string path)
		{
			if (!Exists(path))
			{
				return;
			}
			try
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				File.Delete(path);
			}
			catch (DirectoryNotFoundException)
			{
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception ex3) when (ex3 is IOException || ex3 is UnauthorizedAccessException)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Rename(path, GetTemporaryDeletingFileName(path));
			}
		}

		public static int DeleteMultiple(string directory, string pattern)
		{
			string[] files = GetFiles(directory, pattern);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				Delete(array[i]);
			}
			return files.Length;
		}

		public static void Rename(string path, string newPath)
		{
			File.Move(path, newPath);
		}

		public static void Copy(string sourcePath, string destinationPath, bool overwrite = true)
		{
			DirectoriesManager.Create(PathsManager.GetDirectoryPath(destinationPath));
			File.Copy(sourcePath, destinationPath, overwrite);
		}

		public static void Move(string source, string dest)
		{
			try
			{
				DirectoriesManager.Create(Path.GetDirectoryName(dest));
				File.Move(source, dest);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void EnsureShortcutOnDesktop(string targetFile, string shortcutName)
		{
			string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), shortcutName + ".url");
			if (!Exists(text))
			{
				CreateShortcut(targetFile, text);
			}
		}

		public static void CreateShortcut(string targetFile, string shortcutFile)
		{
			File.Delete(shortcutFile);
			using FileStream stream = new FileStream(shortcutFile, FileMode.Create, FileAccess.ReadWrite);
			using StreamWriter streamWriter = new StreamWriter(stream);
			streamWriter.WriteLine("[InternetShortcut]");
			streamWriter.WriteLine("URL=file:///" + targetFile);
			streamWriter.WriteLine("IconIndex=0");
			streamWriter.WriteLine("WorkingDirectory=" + PathsManager.GetDirectoryPath(targetFile));
			streamWriter.WriteLine("IconFile=" + targetFile.Replace('\\', '/'));
			streamWriter.Flush();
		}

		public static bool IsDirectoryWritable(string directoryPath, bool throwOnFail = false)
		{
			try
			{
				DirectoriesManager.Create(directoryPath);
				using (File.Create(Path.Combine(directoryPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
				{
				}
				return true;
			}
			catch (UnauthorizedAccessException)
			{
				if (throwOnFail)
				{
					throw;
				}
				return false;
			}
		}

		public static bool IsFileLocked(string targetFile)
		{
			try
			{
				using (File.Open(targetFile, FileMode.Open))
				{
				}
			}
			catch (FileNotFoundException)
			{
				GC.WaitForPendingFinalizers();
				return false;
			}
			catch (IOException ex2)
			{
				int num = ex2.HResult & 0xFFFF;
				return num == 32 || num == 33;
			}
			GC.WaitForPendingFinalizers();
			return false;
		}

		public static string GetTemporaryDeletingFileName(string filePath)
		{
			return filePath + ".temp.delete_me";
		}

		public static int DeleteTemporaryDeletingFiles(string folderPath)
		{
			return DeleteMultiple(folderPath, "*.temp.delete_me");
		}

		public static long GetAvailableDiskSpace(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return 0L;
			}
			try
			{
				DriveInfo driveInfo = new DriveInfo(path);
				if (driveInfo.IsReady)
				{
					return driveInfo.AvailableFreeSpace;
				}
			}
			catch
			{
				return 0L;
			}
			return 0L;
		}
	}
}
