using System;
using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class BinaryDeltaReader : IDeltaReader
	{
		private readonly BinaryReader reader;

		private readonly IProgressReporter progressReporter;

		private byte[] expectedHash;

		private IHashAlgorithm hashAlgorithm;

		private bool hasReadMetadata;

		public byte[] ExpectedHash
		{
			get
			{
				EnsureMetadata();
				return expectedHash;
			}
		}

		public IHashAlgorithm HashAlgorithm
		{
			get
			{
				EnsureMetadata();
				return hashAlgorithm;
			}
		}

		public BinaryDeltaReader(Stream stream, IProgressReporter progressReporter)
		{
			reader = new BinaryReader(stream);
			this.progressReporter = progressReporter ?? new NullProgressReporter();
		}

		private void EnsureMetadata()
		{
			if (!hasReadMetadata)
			{
				reader.BaseStream.Seek(0L, SeekOrigin.Begin);
				if (!BinaryComparer.CompareArray(reader.ReadBytes(BinaryFormat.DeltaHeader.Length), BinaryFormat.DeltaHeader))
				{
					throw new CorruptFileFormatException("The delta file appears to be corrupt.");
				}
				if (reader.ReadByte() != 1)
				{
					throw new CorruptFileFormatException("The delta file uses a newer file format than this program can handle.");
				}
				string algorithm = reader.ReadString();
				hashAlgorithm = SupportedAlgorithms.Hashing.Create(algorithm);
				int count = reader.ReadInt32();
				expectedHash = reader.ReadBytes(count);
				byte[] strB = reader.ReadBytes(BinaryFormat.EndOfMetadata.Length);
				if (!BinaryComparer.CompareArray(BinaryFormat.EndOfMetadata, strB))
				{
					throw new CorruptFileFormatException("The signature file appears to be corrupt.");
				}
				hasReadMetadata = true;
			}
		}

		public void Apply(Action<byte[]> writeData, Action<long, long> copy)
		{
			long length = reader.BaseStream.Length;
			EnsureMetadata();
			while (reader.BaseStream.Position != length)
			{
				byte b = reader.ReadByte();
				progressReporter.ReportProgress("Applying delta", reader.BaseStream.Position, length);
				switch (b)
				{
				case 96:
				{
					long arg = reader.ReadInt64();
					long arg2 = reader.ReadInt64();
					copy(arg, arg2);
					break;
				}
				case 128:
				{
					long num = reader.ReadInt64();
					long num2 = 0L;
					while (num2 < num)
					{
						byte[] array = reader.ReadBytes((int)Math.Min(num - num2, 4194304L));
						num2 += array.Length;
						writeData(array);
					}
					break;
				}
				}
			}
		}
	}
}
