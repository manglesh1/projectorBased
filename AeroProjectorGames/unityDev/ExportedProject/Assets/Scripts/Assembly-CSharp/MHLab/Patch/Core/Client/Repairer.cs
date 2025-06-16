using System;
using System.Collections.Generic;
using System.IO;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.Client.Utilities;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Utilities;
using MHLab.Patch.Core.Utilities.Asserts;

namespace MHLab.Patch.Core.Client
{
	public class Repairer : IUpdater
	{
		[Flags]
		private enum FileIntegrity
		{
			None = 0,
			Valid = 1,
			NotExisting = 2,
			InvalidSize = 4,
			InvalidLastWriting = 8,
			InvalidAttributes = 0x10
		}

		private readonly UpdatingContext _context;

		public Repairer(UpdatingContext context)
		{
			_context = context;
		}

		public void Update()
		{
			_context.Logger.Info("Repairing process started.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Repairer.cs", 34L, "Update");
			int num = 0;
			List<DownloadEntry> list = new List<DownloadEntry>();
			long num2 = 0L;
			BuildDefinitionEntry[] entries = _context.CurrentBuildDefinition.Entries;
			foreach (BuildDefinitionEntry buildDefinitionEntry in entries)
			{
				bool flag = false;
				FileIntegrity fileIntegrity = GetFileIntegrity(buildDefinitionEntry);
				FilePath path = _context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), buildDefinitionEntry.RelativePath);
				if (fileIntegrity == FileIntegrity.Valid)
				{
					flag = true;
					_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateCheckedFile, buildDefinitionEntry.RelativePath), buildDefinitionEntry.Size);
				}
				else if (fileIntegrity == FileIntegrity.InvalidAttributes)
				{
					HandleInvalidAttributes(buildDefinitionEntry);
					flag = true;
					_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateFixedAttributes, buildDefinitionEntry.RelativePath), buildDefinitionEntry.Size);
				}
				else if (fileIntegrity == FileIntegrity.InvalidLastWriting || fileIntegrity == (FileIntegrity.InvalidLastWriting | FileIntegrity.InvalidAttributes))
				{
					if (HandleInvalidLastWriting(buildDefinitionEntry))
					{
						flag = true;
						_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateFixedMetadata, buildDefinitionEntry.RelativePath), buildDefinitionEntry.Size);
					}
				}
				else if (fileIntegrity.HasFlag(FileIntegrity.InvalidSize))
				{
					_context.FileSystem.DeleteFile(path);
				}
				if (!flag)
				{
					_context.FileSystem.CreateDirectory(_context.FileSystem.GetDirectoryPath(path));
					FilePath filePath = _context.FileSystem.CombineUri(_context.Settings.GetRemoteBuildUrl(_context.CurrentVersion), _context.Settings.GameFolderName, buildDefinitionEntry.RelativePath);
					FilePath filePath2 = _context.FileSystem.CombineUri(_context.Settings.GetPartialRemoteBuildUrl(_context.CurrentVersion), _context.Settings.GameFolderName, buildDefinitionEntry.RelativePath);
					list.Add(new DownloadEntry(filePath.FullPath, filePath2.FullPath, _context.FileSystem.GetDirectoryPath(path).FullPath, path.FullPath, buildDefinitionEntry));
					num2 += buildDefinitionEntry.Size;
					num++;
				}
			}
			if (_context.Settings.EnableDiskSpaceCheck)
			{
				long availableDiskSpace = _context.FileSystem.GetAvailableDiskSpace((FilePath)_context.Settings.RootPath);
				if (num2 > availableDiskSpace && availableDiskSpace != 0)
				{
					throw new Exception($"Not enough disk space for repairing. Required space [{num2}], available space [{availableDiskSpace}]");
				}
			}
			_context.Downloader.Download(list, delegate(DownloadEntry entry)
			{
				_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateDownloadingFile, entry.Definition.RelativePath));
			}, delegate(long size)
			{
				_context.ReportProgress(size);
			}, delegate(DownloadEntry entry)
			{
				SetDefinition((FilePath)entry.DestinationFile, entry.Definition);
				_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateRepairedFile, entry.Definition.RelativePath));
			});
			if (_context.Settings.DebugMode)
			{
				Assert.AlwaysCheck(ValidatorHelper.ValidateLocalFiles(_context.CurrentBuildDefinition, _context.FileSystem, _context.Logger, _context.Settings.GetGamePath()));
			}
			_context.Logger.Info($"Repairing process completed. Checked {_context.CurrentBuildDefinition.Entries.Length} files, repaired {num} files, skipped {_context.CurrentBuildDefinition.Entries.Length - num} files.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Repairer.cs", 130L, "Update");
		}

		public long ProgressRangeAmount()
		{
			long num = 0L;
			BuildDefinitionEntry[] entries = _context.CurrentBuildDefinition.Entries;
			foreach (BuildDefinitionEntry buildDefinitionEntry in entries)
			{
				num += buildDefinitionEntry.Size;
			}
			return num;
		}

		private FileIntegrity GetFileIntegrity(BuildDefinitionEntry entry)
		{
			string fullPath = _context.FileSystem.SanitizePath(_context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath)).FullPath;
			fullPath = fullPath.Replace(_context.FileSystem.SanitizePath((FilePath)_context.Settings.RootPath).FullPath, "");
			if (fullPath.StartsWith("/"))
			{
				fullPath = fullPath.Substring(1);
			}
			if (!_context.ExistingFilesMap.TryGetValue(fullPath, out var value))
			{
				return FileIntegrity.NotExisting;
			}
			FileIntegrity fileIntegrity = FileIntegrity.None;
			if (value.Size != entry.Size)
			{
				fileIntegrity |= FileIntegrity.InvalidSize;
			}
			if (!AreLastWritingsEqual(value.LastWriting, entry.LastWriting))
			{
				fileIntegrity |= FileIntegrity.InvalidLastWriting;
			}
			if (value.Attributes != entry.Attributes)
			{
				fileIntegrity |= FileIntegrity.InvalidAttributes;
			}
			if (fileIntegrity == FileIntegrity.None)
			{
				return FileIntegrity.Valid;
			}
			return fileIntegrity;
		}

		private FileIntegrity GetRelaxedFileIntegrity(BuildDefinitionEntry entry)
		{
			string fullPath = _context.FileSystem.SanitizePath(_context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath)).FullPath;
			fullPath = fullPath.Replace(_context.Settings.RootPath, "");
			if (fullPath.StartsWith("/"))
			{
				fullPath = fullPath.Substring(1);
			}
			if (!_context.ExistingFilesMap.TryGetValue(fullPath, out var value))
			{
				return FileIntegrity.NotExisting;
			}
			FileIntegrity fileIntegrity = FileIntegrity.None;
			if (value.Size != entry.Size)
			{
				fileIntegrity |= FileIntegrity.InvalidSize;
			}
			if (fileIntegrity == FileIntegrity.None)
			{
				return FileIntegrity.Valid;
			}
			return fileIntegrity;
		}

		private bool AreLastWritingsEqual(DateTime lastWriting1, DateTime lastWriting2)
		{
			return lastWriting1.Year == lastWriting2.Year && lastWriting1.Month == lastWriting2.Month && lastWriting1.Day == lastWriting2.Day && lastWriting1.Hour == lastWriting2.Hour && lastWriting1.Minute == lastWriting2.Minute;
		}

		private void HandleInvalidAttributes(BuildDefinitionEntry entry)
		{
			FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath);
			SetDefinition(filePath, entry);
		}

		private bool HandleInvalidLastWriting(BuildDefinitionEntry entry)
		{
			FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath);
			string fileHash = Hashing.GetFileHash(filePath.FullPath, _context.FileSystem);
			if (entry.Hash != fileHash)
			{
				_context.FileSystem.DeleteFile(filePath);
				return false;
			}
			SetDefinition(filePath, entry);
			return true;
		}

		private void SetDefinition(FilePath filePath, BuildDefinitionEntry currentEntry)
		{
			File.SetAttributes(filePath.FullPath, currentEntry.Attributes);
			File.SetLastWriteTimeUtc(filePath.FullPath, currentEntry.LastWriting);
			File.SetLastWriteTime(filePath.FullPath, currentEntry.LastWriting);
		}

		public bool IsRepairNeeded()
		{
			if (_context.IsRepairNeeded())
			{
				return true;
			}
			BuildDefinitionEntry[] entries = _context.CurrentBuildDefinition.Entries;
			foreach (BuildDefinitionEntry entry in entries)
			{
				if (GetRelaxedFileIntegrity(entry) != FileIntegrity.Valid)
				{
					return true;
				}
			}
			return false;
		}
	}
}
