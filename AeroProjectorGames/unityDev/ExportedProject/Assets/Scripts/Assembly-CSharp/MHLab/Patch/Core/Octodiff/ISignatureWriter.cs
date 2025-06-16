namespace MHLab.Patch.Core.Octodiff
{
	internal interface ISignatureWriter
	{
		void WriteMetadata(IHashAlgorithm hashAlgorithm, IRollingChecksum rollingChecksumAlgorithm, byte[] hash);

		void WriteChunk(ChunkSignature signature);
	}
}
