using System;
using System.Linq;
using MHLab.Patch.Core.Client.Exceptions;
using MHLab.Patch.Core.Client.Utilities;
using MHLab.Patch.Core.Compressing;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Logging;
using MHLab.Patch.Core.Octodiff;
using MHLab.Patch.Core.Utilities;
using MHLab.Patch.Core.Utilities.Asserts;
using MHLab.Patch.Core.Versioning;

namespace MHLab.Patch.Core.Client
{
	public class Updater : IUpdater
	{
		private readonly UpdatingContext _context;

		public Updater(UpdatingContext context)
		{
			_context = context;
		}

		public void Update()
		{
			_context.Logger.Info("Update process started.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Updater.cs", 24L, "Update");
			int num = 0;
			CheckAvailableDiskSpace();
			foreach (PatchDefinition item in _context.PatchesPath)
			{
				ILogger logger = _context.Logger;
				string[] array = new string[7] { "Applying update ", null, null, null, null, null, null };
				int num2 = 1;
				array[num2] = item.From?.ToString();
				array[2] = "_";
				int num3 = 3;
				array[num3] = item.To?.ToString();
				array[4] = " [";
				array[5] = item.Hash;
				array[6] = "]";
				logger.Info(string.Concat(array), "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Updater.cs", 31L, "Update");
				PerformUpdate(item);
				num += item.Entries.Count;
			}
			if (_context.Settings.DebugMode)
			{
				IVersion localVersion = _context.GetLocalVersion();
				Assert.AlwaysCheck(ValidatorHelper.ValidateLocalFiles(_context.GetBuildDefinition(localVersion), _context.FileSystem, _context.Logger, _context.Settings.GetGamePath()));
			}
			_context.Logger.Info($"Update process completed. Applied {_context.PatchesPath.Count} patches with {num} operations.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Updater.cs", 43L, "Update");
		}

		private void CheckAvailableDiskSpace()
		{
			long num = 0L;
			foreach (PatchDefinition item in _context.PatchesPath)
			{
				num += item.TotalSize;
			}
			if (_context.Settings.EnableDiskSpaceCheck)
			{
				long availableDiskSpace = _context.FileSystem.GetAvailableDiskSpace((FilePath)_context.Settings.RootPath);
				if (num > availableDiskSpace && availableDiskSpace != 0)
				{
					throw new Exception($"Not enough disk space for the update. Required space [{num}], available space [{availableDiskSpace}]");
				}
			}
		}

		public long ProgressRangeAmount()
		{
			long num = 0L;
			foreach (PatchDefinition item in _context.PatchesPath)
			{
				foreach (PatchDefinitionEntry entry in item.Entries)
				{
					num += entry.Size;
				}
				num += item.TotalSize;
			}
			return num;
		}

		private void PerformUpdate(PatchDefinition definition)
		{
			_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateDownloadingArchive, definition.From, definition.To));
			DownloadPatch(definition);
			_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateDownloadedArchive, definition.From, definition.To));
			_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateDecompressingArchive, definition.From, definition.To));
			DecompressPatch(definition);
			_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateDecompressedArchive, definition.From, definition.To));
			foreach (PatchDefinitionEntry entry in definition.Entries)
			{
				ProcessFile(definition, entry);
			}
			_context.FileSystem.DeleteDirectory((FilePath)_context.Settings.GetTempPath());
		}

		private void DownloadPatch(PatchDefinition definition)
		{
			_context.FileSystem.CreateDirectory((FilePath)_context.Settings.GetTempPath());
			string downloadedPatchArchivePath = _context.Settings.GetDownloadedPatchArchivePath(definition.From, definition.To);
			int num = _context.Settings.PatchDownloadAttempts;
			bool flag = false;
			do
			{
				try
				{
					_context.Downloader.Download(_context.Settings.GetRemotePatchArchiveUrl(definition.From, definition.To), _context.Settings.GetTempPath(), delegate(long size)
					{
						_context.ReportProgress(size);
					});
					if (Hashing.GetFileHash(downloadedPatchArchivePath, _context.FileSystem) == definition.Hash)
					{
						flag = true;
						break;
					}
				}
				catch
				{
				}
				_context.FileSystem.DeleteFile((FilePath)downloadedPatchArchivePath);
				num--;
				ILogger logger = _context.Logger;
				string[] array = new string[5] { "The patch ", null, null, null, null };
				int num2 = 1;
				array[num2] = definition.From?.ToString();
				array[2] = "_";
				int num3 = 3;
				array[num3] = definition.To?.ToString();
				array[4] = " failed to download. Retrying...";
				logger.Debug(string.Concat(array), "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Updater.cs", 129L, "DownloadPatch");
			}
			while (num > 0);
			if (!flag)
			{
				throw new PatchCannotBeDownloadedException();
			}
		}

		private void DecompressPatch(PatchDefinition definition)
		{
			string uncompressedPatchArchivePath = _context.Settings.GetUncompressedPatchArchivePath(definition.From, definition.To);
			_context.FileSystem.CreateDirectory((FilePath)uncompressedPatchArchivePath);
			Compressor.Decompress(uncompressedPatchArchivePath, _context.Settings.GetDownloadedPatchArchivePath(definition.From, definition.To), null);
		}

		private void ProcessFile(PatchDefinition definition, PatchDefinitionEntry entry)
		{
			switch (entry.Operation)
			{
			case PatchOperation.Deleted:
				_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingDeletedFile, entry.RelativePath));
				HandleDeletedFile(definition, entry);
				_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateProcessedDeletedFile, entry.RelativePath), entry.Size);
				break;
			case PatchOperation.Updated:
				_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingUpdatedFile, entry.RelativePath));
				HandleUpdatedFile(definition, entry);
				_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateProcessedUpdatedFile, entry.RelativePath), entry.Size);
				break;
			case PatchOperation.ChangedAttributes:
				_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingChangedAttributesFile, entry.RelativePath));
				HandleChangedAttributesFile(definition, entry);
				_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateProcessedChangedAttributesFile, entry.RelativePath), entry.Size);
				break;
			case PatchOperation.Added:
				_context.LogProgress(string.Format(_context.LocalizedMessages.UpdateProcessingNewFile, entry.RelativePath));
				HandleAddedFile(definition, entry);
				_context.ReportProgress(string.Format(_context.LocalizedMessages.UpdateProcessedNewFile, entry.RelativePath), entry.Size);
				break;
			}
		}

		private void HandleAddedFile(PatchDefinition definition, PatchDefinitionEntry entry)
		{
			FilePath sourcePath = _context.FileSystem.CombinePaths(_context.Settings.GetUncompressedPatchArchivePath(definition.From, definition.To), entry.RelativePath);
			FilePath filePath = _context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath);
			_context.FileSystem.DeleteFile(filePath);
			_context.FileSystem.MoveFile(sourcePath, filePath);
			EnsureDefinition(filePath.FullPath, entry);
		}

		private void HandleDeletedFile(PatchDefinition definition, PatchDefinitionEntry entry)
		{
			FilePath path = _context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath);
			_context.FileSystem.DeleteFile(path);
		}

		private void HandleUpdatedFile(PatchDefinition definition, PatchDefinitionEntry entry)
		{
			FilePath sourcePath = _context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath);
			FilePath filePath = (FilePath)(sourcePath.ToString() + ".bak");
			FilePath filePath2 = _context.FileSystem.CombinePaths(_context.Settings.GetUncompressedPatchArchivePath(definition.From, definition.To), entry.RelativePath + ".patch");
			try
			{
				_context.FileSystem.RenameFile(sourcePath, filePath);
				DeltaFileApplier.Apply(filePath.FullPath, filePath2.FullPath, sourcePath.FullPath);
				EnsureDefinition(sourcePath.FullPath, entry);
			}
			catch
			{
			}
			finally
			{
				_context.FileSystem.DeleteFile(filePath);
			}
		}

		private void HandleChangedAttributesFile(PatchDefinition definition, PatchDefinitionEntry entry)
		{
			EnsureDefinition(_context.FileSystem.CombinePaths(_context.Settings.GetGamePath(), entry.RelativePath).FullPath, entry);
		}

		private void EnsureDefinition(string filePath, PatchDefinitionEntry entry)
		{
			_context.FileSystem.SetFileAttributes((FilePath)filePath, entry.Attributes);
			_context.FileSystem.SetLastWriteTime((FilePath)filePath, entry.LastWriting);
		}

		public bool IsUpdateAvailable()
		{
			return _context.PatchesIndex.Patches.Any((PatchIndexEntry p) => p.From.Equals(_context.CurrentVersion));
		}
	}
}
