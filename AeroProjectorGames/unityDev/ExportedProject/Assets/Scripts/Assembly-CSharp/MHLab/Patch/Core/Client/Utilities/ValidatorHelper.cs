using System.Text;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Logging;
using MHLab.Patch.Core.Utilities;

namespace MHLab.Patch.Core.Client.Utilities
{
	public static class ValidatorHelper
	{
		public static bool ValidateLocalFiles(BuildDefinition buildDefinition, IFileSystem fileSystem, ILogger logger, string rootPath)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("The following files are not valid after debug validation: ");
			BuildDefinitionEntry[] entries = buildDefinition.Entries;
			foreach (BuildDefinitionEntry buildDefinitionEntry in entries)
			{
				FilePath path = fileSystem.CombinePaths(rootPath, buildDefinitionEntry.RelativePath);
				LocalFileInfo fileInfo = fileSystem.GetFileInfo(path);
				if (fileInfo.Size != buildDefinitionEntry.Size)
				{
					stringBuilder.AppendLine($"[{buildDefinitionEntry.RelativePath}] with expected size of [{buildDefinitionEntry.Size}]. Found [{fileInfo.Size}]");
					flag = true;
				}
				else if (buildDefinitionEntry.Hash != null)
				{
					string fileHash = Hashing.GetFileHash(path.FullPath, fileSystem);
					if (fileHash != buildDefinitionEntry.Hash)
					{
						stringBuilder.AppendLine("[" + buildDefinitionEntry.RelativePath + "] with expected hash of [" + buildDefinitionEntry.Hash + "]. Found [" + fileHash + "]");
						flag = true;
					}
				}
				if (buildDefinitionEntry.Attributes != fileInfo.Attributes)
				{
					stringBuilder.AppendLine($"[{buildDefinitionEntry.RelativePath}] with expected attributes of [{buildDefinitionEntry.Attributes}]. Found [{fileInfo.Attributes}]");
					flag = true;
				}
			}
			if (flag)
			{
				logger.Debug(stringBuilder.ToString(), "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Utilities\\ValidatorHelper.cs", 52L, "ValidateLocalFiles");
				return false;
			}
			logger.Debug($"Verified {buildDefinition.Entries.Length} files. All good.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\Utilities\\ValidatorHelper.cs", 56L, "ValidateLocalFiles");
			return true;
		}
	}
}
