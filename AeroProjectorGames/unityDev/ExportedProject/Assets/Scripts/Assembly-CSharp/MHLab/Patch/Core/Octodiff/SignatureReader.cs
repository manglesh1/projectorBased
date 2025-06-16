using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class SignatureReader : ISignatureReader
	{
		private readonly IProgressReporter reporter;

		private readonly BinaryReader reader;

		public SignatureReader(string signatureFileName, IProgressReporter reporter)
		{
			this.reporter = reporter;
			reader = new BinaryReader(new FileStream(signatureFileName, FileMode.Open, FileAccess.Read));
		}

		public Signature ReadSignature()
		{
			Progress();
			byte[] strB = reader.ReadBytes(BinaryFormat.SignatureHeader.Length);
			if (!BinaryComparer.CompareArray(BinaryFormat.SignatureHeader, strB))
			{
				throw new CorruptFileFormatException("The signature file appears to be corrupt.");
			}
			if (reader.ReadByte() != 1)
			{
				throw new CorruptFileFormatException("The signature file uses a newer file format than this program can handle.");
			}
			string algorithm = reader.ReadString();
			string algorithm2 = reader.ReadString();
			byte[] strB2 = reader.ReadBytes(BinaryFormat.EndOfMetadata.Length);
			if (!BinaryComparer.CompareArray(BinaryFormat.EndOfMetadata, strB2))
			{
				throw new CorruptFileFormatException("The signature file appears to be corrupt.");
			}
			Progress();
			IHashAlgorithm hashAlgorithm = SupportedAlgorithms.Hashing.Create(algorithm);
			Signature signature = new Signature(hashAlgorithm, SupportedAlgorithms.Checksum.Create(algorithm2));
			int hashLength = hashAlgorithm.HashLength;
			long num = 0L;
			long length = reader.BaseStream.Length;
			bool flag = length - reader.BaseStream.Position != 0;
			int num2 = 6 + hashLength;
			if ((long)(flag ? 1 : 0) % (long)num2 != 0)
			{
				throw new CorruptFileFormatException("The signature file appears to be corrupt; at least one chunk has data missing.");
			}
			while (reader.BaseStream.Position < length - 1)
			{
				short num3 = reader.ReadInt16();
				uint rollingChecksum = reader.ReadUInt32();
				byte[] hash = reader.ReadBytes(hashLength);
				signature.Chunks.Add(new ChunkSignature
				{
					StartOffset = num,
					Length = num3,
					RollingChecksum = rollingChecksum,
					Hash = hash
				});
				num += num3;
				Progress();
			}
			return signature;
		}

		private void Progress()
		{
			reporter.ReportProgress("Reading signature", reader.BaseStream.Position, reader.BaseStream.Length);
		}
	}
}
