using System;
using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class BinaryDeltaWriter : IDeltaWriter
	{
		private readonly BinaryWriter writer;

		public BinaryDeltaWriter(Stream stream)
		{
			writer = new BinaryWriter(stream);
		}

		public void WriteMetadata(IHashAlgorithm hashAlgorithm, byte[] expectedNewFileHash)
		{
			writer.Write(BinaryFormat.DeltaHeader);
			writer.Write(1);
			writer.Write(hashAlgorithm.Name);
			writer.Write(expectedNewFileHash.Length);
			writer.Write(expectedNewFileHash);
			writer.Write(BinaryFormat.EndOfMetadata);
		}

		public void WriteCopyCommand(DataRange segment)
		{
			writer.Write(96);
			writer.Write(segment.StartOffset);
			writer.Write(segment.Length);
		}

		public void WriteDataCommand(Stream source, long offset, long length)
		{
			writer.Write(128);
			writer.Write(length);
			long position = source.Position;
			try
			{
				source.Seek(offset, SeekOrigin.Begin);
				byte[] array = new byte[Math.Min((int)length, 1048576)];
				long num = 0L;
				int num2;
				while ((num2 = source.Read(array, 0, (int)Math.Min(length - num, array.Length))) > 0)
				{
					num += num2;
					writer.Write(array, 0, num2);
				}
			}
			finally
			{
				source.Seek(position, SeekOrigin.Begin);
			}
		}

		public void Finish()
		{
		}
	}
}
