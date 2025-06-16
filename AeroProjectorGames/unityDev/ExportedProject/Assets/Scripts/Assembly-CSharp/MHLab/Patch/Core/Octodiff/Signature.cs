using System.Collections.Generic;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class Signature
	{
		public IHashAlgorithm HashAlgorithm { get; private set; }

		public IRollingChecksum RollingChecksumAlgorithm { get; private set; }

		public List<ChunkSignature> Chunks { get; private set; }

		public Signature(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm)
		{
			HashAlgorithm = hashAlgorithm;
			RollingChecksumAlgorithm = rollingChecksumAlgorithm;
			Chunks = new List<ChunkSignature>();
		}
	}
}
