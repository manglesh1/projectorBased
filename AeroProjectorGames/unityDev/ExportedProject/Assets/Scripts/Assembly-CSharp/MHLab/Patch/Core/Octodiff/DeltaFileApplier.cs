using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	public static class DeltaFileApplier
	{
		public static void Apply(string fileBackupPath, string patchPath, string filePath)
		{
			DeltaApplier deltaApplier = new DeltaApplier
			{
				SkipHashCheck = true
			};
			using FileStream basisFileStream = new FileStream(fileBackupPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using FileStream stream = new FileStream(patchPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			using FileStream outputStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
			deltaApplier.Apply(basisFileStream, new BinaryDeltaReader(stream, null), outputStream);
		}
	}
}
