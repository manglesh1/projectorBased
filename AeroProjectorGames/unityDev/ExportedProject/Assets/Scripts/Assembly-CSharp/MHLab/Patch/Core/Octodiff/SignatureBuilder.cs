using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class SignatureBuilder
	{
		public static readonly short MinimumChunkSize = 128;

		public static readonly short DefaultChunkSize = 2048;

		public static readonly short MaximumChunkSize = 31744;

		private short chunkSize;

		public IProgressReporter ProgressReporter { get; set; }

		public IHashAlgorithm HashAlgorithm { get; set; }

		public IRollingChecksum RollingChecksumAlgorithm { get; set; }

		public short ChunkSize
		{
			get
			{
				return chunkSize;
			}
			set
			{
				if (value < MinimumChunkSize)
				{
					throw new UsageException($"Chunk size cannot be less than {MinimumChunkSize}");
				}
				if (value > MaximumChunkSize)
				{
					throw new UsageException($"Chunk size cannot be exceed {MaximumChunkSize}");
				}
				chunkSize = value;
			}
		}

		public SignatureBuilder()
		{
			ChunkSize = DefaultChunkSize;
			HashAlgorithm = SupportedAlgorithms.Hashing.Default();
			RollingChecksumAlgorithm = SupportedAlgorithms.Checksum.Default();
			ProgressReporter = new NullProgressReporter();
		}

		public void Build(Stream stream, ISignatureWriter signatureWriter)
		{
			WriteMetadata(stream, signatureWriter);
			WriteChunkSignatures(stream, signatureWriter);
		}

		private void WriteMetadata(Stream stream, ISignatureWriter signatureWriter)
		{
			stream.Seek(0L, SeekOrigin.Begin);
			byte[] hash = HashAlgorithm.ComputeHash(stream);
			signatureWriter.WriteMetadata(HashAlgorithm, RollingChecksumAlgorithm, hash);
		}

		private void WriteChunkSignatures(Stream stream, ISignatureWriter signatureWriter)
		{
			IRollingChecksum rollingChecksumAlgorithm = RollingChecksumAlgorithm;
			IHashAlgorithm hashAlgorithm = HashAlgorithm;
			stream.Seek(0L, SeekOrigin.Begin);
			long num = 0L;
			byte[] array = new byte[ChunkSize];
			int num2;
			while ((num2 = stream.Read(array, 0, array.Length)) > 0)
			{
				signatureWriter.WriteChunk(new ChunkSignature
				{
					StartOffset = num,
					Length = (short)num2,
					Hash = hashAlgorithm.ComputeHash(array, 0, num2),
					RollingChecksum = rollingChecksumAlgorithm.Calculate(array, 0, num2)
				});
				num += num2;
				ProgressReporter.ReportProgress("Building signatures", num, stream.Length);
			}
		}
	}
}
