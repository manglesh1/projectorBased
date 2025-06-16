using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class AggregateCopyOperationsDecorator : IDeltaWriter
	{
		private readonly IDeltaWriter decorated;

		private DataRange bufferedCopy;

		public AggregateCopyOperationsDecorator(IDeltaWriter decorated)
		{
			this.decorated = decorated;
		}

		public void WriteDataCommand(Stream source, long offset, long length)
		{
			FlushCurrentCopyCommand();
			decorated.WriteDataCommand(source, offset, length);
		}

		public void WriteMetadata(IHashAlgorithm hashAlgorithm, byte[] expectedNewFileHash)
		{
			decorated.WriteMetadata(hashAlgorithm, expectedNewFileHash);
		}

		public void WriteCopyCommand(DataRange chunk)
		{
			if (bufferedCopy.Length > 0 && bufferedCopy.StartOffset + bufferedCopy.Length == chunk.StartOffset)
			{
				bufferedCopy.Length += chunk.Length;
				return;
			}
			FlushCurrentCopyCommand();
			bufferedCopy = chunk;
		}

		private void FlushCurrentCopyCommand()
		{
			if (bufferedCopy.Length > 0)
			{
				decorated.WriteCopyCommand(bufferedCopy);
				bufferedCopy = default(DataRange);
			}
		}

		public void Finish()
		{
			FlushCurrentCopyCommand();
			decorated.Finish();
		}
	}
}
