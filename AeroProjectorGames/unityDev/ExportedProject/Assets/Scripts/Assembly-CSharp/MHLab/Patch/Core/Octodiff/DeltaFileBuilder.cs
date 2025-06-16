using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	public static class DeltaFileBuilder
	{
		public static void Build(string fromFile, string toFile, string patchFile, string signatureFile)
		{
			SignatureBuilder signatureBuilder = new SignatureBuilder();
			DeltaBuilder deltaBuilder = new DeltaBuilder();
			FileStream fileStream = new FileStream(fromFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			FileStream fileStream2 = new FileStream(toFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
			FileStream fileStream3 = new FileStream(patchFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			FileStream fileStream4 = new FileStream(signatureFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			signatureBuilder.Build(fileStream, new SignatureWriter(fileStream4));
			fileStream4.Close();
			fileStream4.Dispose();
			deltaBuilder.BuildDelta(fileStream2, new SignatureReader(fileStream4.Name, deltaBuilder.ProgressReporter), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(fileStream3)));
			fileStream.Close();
			fileStream.Dispose();
			fileStream2.Close();
			fileStream2.Dispose();
			fileStream3.Close();
			fileStream3.Dispose();
		}
	}
}
