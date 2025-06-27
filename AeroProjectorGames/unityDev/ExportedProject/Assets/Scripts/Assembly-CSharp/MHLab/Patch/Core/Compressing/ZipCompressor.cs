using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace MHLab.Patch.Core.Compressing
{
	internal class ZipCompressor
	{
		public static void ZipFolder(string outPathname, string password, string folderName, int compressionLevel, Func<string, bool> filesFilter = null)
		{
			ZipConstants.DefaultCodePage = 0;
			using FileStream baseOutputStream = File.Create(outPathname);
			using ZipOutputStream zipOutputStream = new ZipOutputStream(baseOutputStream);
			zipOutputStream.SetLevel(compressionLevel);
			zipOutputStream.Password = password;
			int length = folderName.Length;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			int folderOffset = length + ((!folderName.EndsWith(directorySeparatorChar.ToString())) ? 1 : 0);
			CompressFolder(folderName, zipOutputStream, folderOffset, filesFilter);
			zipOutputStream.IsStreamOwner = true;
			zipOutputStream.Close();
		}

		private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset, Func<string, bool> filesFilter)
		{
			string[] array = Directory.GetFiles(path);
			if (filesFilter != null)
			{
				array = array.Where(filesFilter).ToArray();
			}
			string[] array2 = array;
			foreach (string text in array2)
			{
				FileInfo fileInfo = new FileInfo(text);
				zipStream.PutNextEntry(new ZipEntry(ZipEntry.CleanName(text.Substring(folderOffset)))
				{
					DateTime = fileInfo.LastWriteTime,
					Size = fileInfo.Length
				});
				byte[] buffer = new byte[4096];
				using (FileStream source = File.OpenRead(text))
				{
					StreamUtils.Copy(source, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}
			string[] directories = Directory.GetDirectories(path);
			for (int j = 0; j < directories.Length; j++)
			{
				CompressFolder(directories[j], zipStream, folderOffset, filesFilter);
			}
		}

		public static void ExtractZipFile(string archiveFilenameIn, string password, string outFolder)
		{
			ZipConstants.DefaultCodePage = 0;
			ZipFile zipFile = null;
			try
			{
				zipFile = new ZipFile(File.OpenRead(archiveFilenameIn));
				if (!string.IsNullOrEmpty(password))
				{
					zipFile.Password = password;
				}
				foreach (object item in zipFile)
				{
					ZipEntry zipEntry = (ZipEntry)item;
					if (zipEntry.IsFile)
					{
						string name = zipEntry.Name;
						byte[] buffer = new byte[4096];
						Stream inputStream = zipFile.GetInputStream(zipEntry);
						string path = Path.Combine(outFolder, name);
						string directoryName = Path.GetDirectoryName(path);
						if (!string.IsNullOrEmpty(directoryName))
						{
							Directory.CreateDirectory(directoryName);
						}
						using FileStream destination = File.Create(path);
						StreamUtils.Copy(inputStream, destination, buffer);
					}
				}
			}
			finally
			{
				if (zipFile != null)
				{
					zipFile.IsStreamOwner = true;
					zipFile.Close();
				}
			}
		}
	}
}
