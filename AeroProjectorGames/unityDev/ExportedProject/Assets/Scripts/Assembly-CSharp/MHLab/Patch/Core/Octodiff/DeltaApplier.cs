using System;
using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class DeltaApplier
	{
		public bool SkipHashCheck { get; set; }

		public DeltaApplier()
		{
			SkipHashCheck = false;
		}

		public void Apply(Stream basisFileStream, IDeltaReader delta, Stream outputStream)
		{
			delta.Apply(delegate(byte[] data)
			{
				outputStream.Write(data, 0, data.Length);
			}, delegate(long startPosition, long length)
			{
				basisFileStream.Seek(startPosition, SeekOrigin.Begin);
				byte[] array = new byte[4194304];
				long num = 0L;
				int num2;
				while ((num2 = basisFileStream.Read(array, 0, (int)Math.Min(length - num, array.Length))) > 0)
				{
					num += num2;
					outputStream.Write(array, 0, num2);
				}
			});
			if (!SkipHashCheck)
			{
				outputStream.Seek(0L, SeekOrigin.Begin);
				byte[] expectedHash = delta.ExpectedHash;
				byte[] strB = delta.HashAlgorithm.ComputeHash(outputStream);
				if (!BinaryComparer.CompareArray(expectedHash, strB))
				{
					throw new UsageException("Verification of the patched file failed. The SHA1 hash of the patch result file, and the file that was used as input for the delta, do not match. This can happen if the basis file changed since the signatures were calculated.");
				}
			}
		}
	}
}
