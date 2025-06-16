using System;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.Compressing;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Utilities;

namespace MHLab.Patch.Core.Client
{
	public class PatcherUpdater : IUpdater
	{
		[Flags]
		private enum FileValidityDifference
		{
			None = 0,
			Size = 1,
			LastWriting = 2,
			Attributes = 4,
			Hash = 8
		}

		private readonly UpdatingContext _context;

		public PatcherUpdater(UpdatingContext context)
		{
			_context = context;
		}

		public void Update()
		{
			if (_context.CurrentUpdaterDefinition == null)
			{
				_context.Logger.Warning("No updater definition found. The Launcher cannot be validated or updated.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 33L, "Update");
				return;
			}
			_context.Logger.Info($"Launcher update started. The update contains {_context.CurrentUpdaterDefinition.Entries.Length} operations.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 37L, "Update");
			_context.FileSystem.DeleteTemporaryDeletingFiles((FilePath)_context.Settings.RootPath);
			CheckAvailableDiskSpace();
			if (_context.Settings.PatcherUpdaterSafeMode)
			{
				_context.Logger.Info("Launcher update SAFE MODE: ENABLED", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 45L, "Update");
				if (!HandleSafeModeUpdate())
				{
					HandleUpdate();
				}
			}
			else
			{
				HandleUpdate();
			}
			_context.Logger.Info("Launcher update completed.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 55L, "Update");
		}

		private void CheckAvailableDiskSpace()
		{
			long num = 0L;
			UpdaterDefinitionEntry[] entries = _context.CurrentUpdaterDefinition.Entries;
			foreach (UpdaterDefinitionEntry updaterDefinitionEntry in entries)
			{
				num += updaterDefinitionEntry.Size;
			}
			if (_context.Settings.EnableDiskSpaceCheck)
			{
				long availableDiskSpace = _context.FileSystem.GetAvailableDiskSpace((FilePath)_context.Settings.RootPath);
				if (num > availableDiskSpace && availableDiskSpace != 0)
				{
					throw new Exception($"Not enough disk space for the Launcher update. Required space [{num}], available space [{availableDiskSpace}]");
				}
			}
		}

		private void HandleUpdate()
		{
			UpdaterDefinitionEntry[] entries = _context.CurrentUpdaterDefinition.Entries;
			foreach (UpdaterDefinitionEntry updaterDefinitionEntry in entries)
			{
				switch (updaterDefinitionEntry.Operation)
				{
				case PatchOperation.Unchanged:
					HandleUnchangedFile(updaterDefinitionEntry);
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateUnchangedFile, updaterDefinitionEntry.RelativePath));
					break;
				case PatchOperation.Deleted:
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingDeletedFile, updaterDefinitionEntry.RelativePath));
					HandleDeletedFile(updaterDefinitionEntry);
					_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateProcessedDeletedFile, updaterDefinitionEntry.RelativePath), updaterDefinitionEntry.Size);
					break;
				case PatchOperation.Updated:
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingUpdatedFile, updaterDefinitionEntry.RelativePath));
					HandleUpdatedFile(updaterDefinitionEntry);
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessedUpdatedFile, updaterDefinitionEntry.RelativePath));
					break;
				case PatchOperation.ChangedAttributes:
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingChangedAttributesFile, updaterDefinitionEntry.RelativePath));
					HandleChangedAttributesFile(updaterDefinitionEntry);
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessedChangedAttributesFile, updaterDefinitionEntry.RelativePath));
					break;
				case PatchOperation.Added:
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingNewFile, updaterDefinitionEntry.RelativePath));
					HandleAddedFile(updaterDefinitionEntry);
					_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessedNewFile, updaterDefinitionEntry.RelativePath));
					break;
				}
			}
		}

		private bool HandleSafeModeUpdate()
		{
			if (!TryCheckForSafeModeLockFile())
			{
				return false;
			}
			if (!TryDownloadUpdaterSafeModeIndex(out var definition))
			{
				return false;
			}
			if (!TryDownloadUpdaterSafeModeArchive(definition))
			{
				return false;
			}
			if (!DecompressSafeModeArchive(definition))
			{
				return false;
			}
			if (!CreateBackupForOldLauncherFiles())
			{
				return false;
			}
			if (!HandleDecompressedSafeModeArchive(definition))
			{
				RollbackSafeModeUpdate();
				return false;
			}
			CleanAfterSafeModeUpdate();
			_context.DisableSafeMode();
			_context.SetDirtyFlag(definition.ExecutableToRun, definition);
			_context.Logger.Debug("Launcher update SAFE MODE: COMPLETED", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 129L, "HandleSafeModeUpdate");
			return true;
		}

		private bool TryCheckForSafeModeLockFile()
		{
			FilePath updaterSafeModeLockFilePath = _context.Settings.GetUpdaterSafeModeLockFilePath();
			if (_context.FileSystem.FileExists(updaterSafeModeLockFilePath))
			{
				_context.FileSystem.DeleteFile(updaterSafeModeLockFilePath);
				_context.Logger.Debug("Safe Mode file lock found. Deleting it and exiting Safe Mode.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 141L, "TryCheckForSafeModeLockFile");
				return false;
			}
			bool result;
			using (_context.FileSystem.CreateFile(updaterSafeModeLockFilePath))
			{
				result = true;
			}
			return result;
		}

		private bool TryDownloadUpdaterSafeModeIndex(out UpdaterSafeModeDefinition definition)
		{
			try
			{
				string remoteUpdaterSafeModeIndexUrl = _context.Settings.GetRemoteUpdaterSafeModeIndexUrl();
				string text = _context.Settings.GetRemoteUpdaterSafeModeIndexUrl();
				if (!string.IsNullOrWhiteSpace(_context.Settings.RemoteUrl))
				{
					text = text.Replace(_context.Settings.RemoteUrl, string.Empty);
				}
				DownloadEntry entry = new DownloadEntry(remoteUpdaterSafeModeIndexUrl, text, null, null, null);
				definition = _context.Downloader.DownloadJson<UpdaterSafeModeDefinition>(entry, _context.Serializer);
				return true;
			}
			catch
			{
				_context.Logger.Warning("Cannot download the updater safe mode INDEX. Falling back on normal self-update process.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 172L, "TryDownloadUpdaterSafeModeIndex");
				definition = null;
				return false;
			}
		}

		private bool TryDownloadUpdaterSafeModeArchive(UpdaterSafeModeDefinition definition)
		{
			try
			{
				string remoteUpdaterSafeModeArchiveUrl = _context.Settings.GetRemoteUpdaterSafeModeArchiveUrl(definition.ArchiveName);
				string tempPath = _context.Settings.GetTempPath();
				FilePath path = _context.FileSystem.CombinePaths(tempPath, definition.ArchiveName);
				_context.FileSystem.DeleteFile(path);
				_context.Downloader.Download(remoteUpdaterSafeModeArchiveUrl, tempPath);
				return true;
			}
			catch
			{
				_context.Logger.Warning("Cannot download the updater safe mode ARCHIVE. Falling back on normal self-update process.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 193L, "TryDownloadUpdaterSafeModeArchive");
				return false;
			}
		}

		private bool CreateBackupForOldLauncherFiles()
		{
			string updaterSafeModeBackupPath = _context.Settings.GetUpdaterSafeModeBackupPath();
			string text = string.Empty;
			try
			{
				UpdaterDefinitionEntry[] entries = _context.CurrentUpdaterDefinition.Entries;
				for (int i = 0; i < entries.Length; i++)
				{
					text = entries[i].RelativePath;
					FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.RootPath, text);
					FilePath destinationPath = _context.FileSystem.CombinePaths(updaterSafeModeBackupPath, text);
					if (_context.FileSystem.FileExists(filePath))
					{
						_context.FileSystem.MoveFile(filePath, destinationPath);
					}
				}
				return true;
			}
			catch (Exception exception)
			{
				_context.Logger.Error(exception, "Safe mode: a launcher file cannot be moved [" + text + "]", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 221L, "CreateBackupForOldLauncherFiles");
				return false;
			}
		}

		private bool DecompressSafeModeArchive(UpdaterSafeModeDefinition definition)
		{
			FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.GetTempPath(), definition.ArchiveName);
			string updaterSafeModeTempPath = _context.Settings.GetUpdaterSafeModeTempPath();
			try
			{
				Compressor.Decompress(updaterSafeModeTempPath, filePath.FullPath, null);
				return true;
			}
			catch (Exception exception)
			{
				_context.Logger.Error(exception, "Cannot decompress the SAFE MODE archive.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 238L, "DecompressSafeModeArchive");
				return false;
			}
		}

		private bool HandleDecompressedSafeModeArchive(UpdaterSafeModeDefinition definition)
		{
			string updaterSafeModeTempPath = _context.Settings.GetUpdaterSafeModeTempPath();
			string updaterSafeModeBackupPath = _context.Settings.GetUpdaterSafeModeBackupPath();
			string text = string.Empty;
			try
			{
				LocalFileInfo[] filesInfo = _context.FileSystem.GetFilesInfo((FilePath)updaterSafeModeTempPath);
				for (int i = 0; i < filesInfo.Length; i++)
				{
					text = filesInfo[i].RelativePath;
					FilePath sourcePath = _context.FileSystem.CombinePaths(updaterSafeModeTempPath, text);
					FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.RootPath, text);
					if (_context.FileSystem.FileExists(filePath))
					{
						if (_context.FileSystem.IsFileLocked(filePath))
						{
							_context.FileSystem.CopyFile(filePath, _context.FileSystem.CombinePaths(updaterSafeModeBackupPath, text));
							_context.FileSystem.RenameFile(filePath, (FilePath)_context.FileSystem.GetTemporaryDeletingFileName(filePath));
						}
						else
						{
							_context.FileSystem.MoveFile(filePath, _context.FileSystem.CombinePaths(updaterSafeModeBackupPath, text));
						}
					}
					_context.FileSystem.MoveFile(sourcePath, filePath);
				}
				return true;
			}
			catch (Exception exception)
			{
				_context.Logger.Error(exception, "Cannot apply the SAFE MODE update. A file [" + text + "] cannot be moved. Maybe it's locked by another process.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 280L, "HandleDecompressedSafeModeArchive");
				return false;
			}
		}

		private void CleanAfterSafeModeUpdate()
		{
			try
			{
				_context.FileSystem.DeleteDirectory((FilePath)_context.Settings.GetUpdaterSafeModeTempPath());
				_context.FileSystem.DeleteDirectory((FilePath)_context.Settings.GetUpdaterSafeModeBackupPath());
				_context.FileSystem.DeleteFile(_context.Settings.GetUpdaterSafeModeLockFilePath());
			}
			catch (Exception arg)
			{
				_context.Logger.Warning($"Cannot clean the SAFE MODE temporary data. {arg}", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 295L, "CleanAfterSafeModeUpdate");
			}
		}

		private void RollbackSafeModeUpdate()
		{
			FilePath path = (FilePath)_context.Settings.GetUpdaterSafeModeBackupPath();
			string text = string.Empty;
			try
			{
				LocalFileInfo[] filesInfo = _context.FileSystem.GetFilesInfo(path);
				for (int i = 0; i < filesInfo.Length; i++)
				{
					text = filesInfo[i].RelativePath;
					FilePath sourcePath = _context.FileSystem.CombinePaths(path.FullPath, text);
					FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.RootPath, text);
					if (_context.FileSystem.FileExists(filePath))
					{
						_context.FileSystem.DeleteFile(filePath);
					}
					_context.FileSystem.MoveFile(sourcePath, filePath);
				}
			}
			catch (Exception exception)
			{
				_context.Logger.Error(exception, "Cannot process the SAFE MODE backup. A file [" + text + "] cannot be moved back. There is a high chance that the Launcher is now corrupted and you need to re-download it from scratch.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\PatcherUpdater.cs", 326L, "RollbackSafeModeUpdate");
			}
		}

		public long ProgressRangeAmount()
		{
			long num = 0L;
			if (_context.CurrentUpdaterDefinition != null)
			{
				UpdaterDefinitionEntry[] entries = _context.CurrentUpdaterDefinition.Entries;
				foreach (UpdaterDefinitionEntry updaterDefinitionEntry in entries)
				{
					num += updaterDefinitionEntry.Size;
				}
			}
			return num;
		}

		private bool IsValid(UpdaterDefinitionEntry entry, out FileValidityDifference difference)
		{
			FilePath path = _context.FileSystem.CombinePaths(_context.Settings.RootPath, entry.RelativePath);
			LocalFileInfo fileInfo = _context.FileSystem.GetFileInfo(path);
			difference = FileValidityDifference.None;
			if (fileInfo.Size != entry.Size)
			{
				difference |= FileValidityDifference.Size;
			}
			else if (!string.IsNullOrWhiteSpace(entry.Hash))
			{
				string fileHash = Hashing.GetFileHash(path.FullPath, _context.FileSystem);
				if (entry.Hash != fileHash)
				{
					difference |= FileValidityDifference.Hash;
				}
			}
			return difference == FileValidityDifference.None;
		}

		private bool AreLastWritingsEqual(DateTime lastWriting1, DateTime lastWriting2)
		{
			return lastWriting1.Year == lastWriting2.Year && lastWriting1.Month == lastWriting2.Month && lastWriting1.Day == lastWriting2.Day && lastWriting1.Hour == lastWriting2.Hour && lastWriting1.Minute == lastWriting2.Minute && lastWriting1.Second == lastWriting2.Second;
		}

		private void RemoveInvalidFile(FilePath filePath)
		{
			if (_context.FileSystem.IsFileLocked(filePath))
			{
				FilePath filePath2 = (FilePath)_context.FileSystem.GetTemporaryDeletingFileName(filePath);
				if (_context.FileSystem.FileExists(filePath2))
				{
					_context.FileSystem.DeleteFile(filePath2);
				}
				_context.FileSystem.RenameFile(filePath, filePath2);
			}
			else
			{
				_context.FileSystem.DeleteFile(filePath);
			}
		}

		private void HandleAddedFile(UpdaterDefinitionEntry entry)
		{
			FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.RootPath, entry.RelativePath);
			FileValidityDifference difference = FileValidityDifference.None;
			bool flag = _context.FileSystem.FileExists(filePath);
			_context.FileSystem.UnlockFile(filePath);
			if (flag && IsValid(entry, out difference))
			{
				EnsureDefinition(entry);
				return;
			}
			if (difference.HasFlag(FileValidityDifference.Size))
			{
				RemoveInvalidFile(filePath);
				_context.Downloader.Download(_context.Settings.GetRemoteUpdaterFileUrl(entry.RelativePath), _context.FileSystem.GetDirectoryPath(filePath).FullPath, delegate(long size)
				{
					_context.ReportProgress(size);
				});
				EnsureDefinition(entry);
				_context.SetDirtyFlag(entry.RelativePath);
				return;
			}
			if (difference.HasFlag(FileValidityDifference.Hash))
			{
				RemoveInvalidFile(filePath);
				flag = false;
			}
			if (!flag)
			{
				_context.Downloader.Download(_context.Settings.GetRemoteUpdaterFileUrl(entry.RelativePath), _context.FileSystem.GetDirectoryPath(filePath).FullPath, delegate(long size)
				{
					_context.ReportProgress(size);
				});
				_context.SetDirtyFlag(entry.RelativePath);
			}
			if (!_context.FileSystem.IsFileLocked(filePath))
			{
				EnsureDefinition(entry);
			}
		}

		private void HandleDeletedFile(UpdaterDefinitionEntry entry)
		{
			FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.RootPath, entry.RelativePath);
			if (_context.FileSystem.IsFileLocked(filePath))
			{
				FilePath destinationPath = (FilePath)_context.FileSystem.GetTemporaryDeletingFileName(filePath);
				_context.FileSystem.RenameFile(filePath, destinationPath);
			}
			else
			{
				_context.FileSystem.DeleteFile(filePath);
			}
		}

		private void HandleChangedAttributesFile(UpdaterDefinitionEntry entry)
		{
			HandleAddedFile(entry);
		}

		private void HandleUpdatedFile(UpdaterDefinitionEntry entry)
		{
			HandleAddedFile(entry);
		}

		private void HandleUnchangedFile(UpdaterDefinitionEntry entry)
		{
			HandleAddedFile(entry);
		}

		private void EnsureDefinition(UpdaterDefinitionEntry entry)
		{
			FilePath path = _context.FileSystem.CombinePaths(_context.Settings.RootPath, entry.RelativePath);
			_context.FileSystem.SetFileAttributes(path, entry.Attributes);
			_context.FileSystem.SetLastWriteTime(path, entry.LastWriting);
		}

		public bool IsUpdateAvailable()
		{
			if (_context.CurrentUpdaterDefinition == null)
			{
				return false;
			}
			UpdaterDefinitionEntry[] entries = _context.CurrentUpdaterDefinition.Entries;
			foreach (UpdaterDefinitionEntry updaterDefinitionEntry in entries)
			{
				FilePath path = _context.FileSystem.CombinePaths(_context.Settings.RootPath, updaterDefinitionEntry.RelativePath);
				if (!_context.FileSystem.FileExists(path))
				{
					return true;
				}
				if (!IsValid(updaterDefinitionEntry, out var _))
				{
					return true;
				}
			}
			return false;
		}
	}
}
