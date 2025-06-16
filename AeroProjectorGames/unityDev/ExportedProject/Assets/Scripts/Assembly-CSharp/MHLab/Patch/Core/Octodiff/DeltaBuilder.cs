using System.Collections.Generic;
using System.IO;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class DeltaBuilder
	{
		private const int ReadBufferSize = 4194304;

		public IProgressReporter ProgressReporter { get; set; }

		public DeltaBuilder()
		{
			ProgressReporter = new NullProgressReporter();
		}

		public void BuildDelta(Stream newFileStream, ISignatureReader signatureReader, IDeltaWriter deltaWriter)
		{
			Signature signature = signatureReader.ReadSignature();
			List<ChunkSignature> chunks = signature.Chunks;
			byte[] expectedNewFileHash = signature.HashAlgorithm.ComputeHash(newFileStream);
			newFileStream.Seek(0L, SeekOrigin.Begin);
			deltaWriter.WriteMetadata(signature.HashAlgorithm, expectedNewFileHash);
			chunks = OrderChunksByChecksum(chunks);
			int maxChunkSize;
			int minChunkSize;
			Dictionary<uint, int> dictionary = CreateChunkMap(chunks, out maxChunkSize, out minChunkSize);
			byte[] array = new byte[4194304];
			long num = 0L;
			long length = newFileStream.Length;
			ProgressReporter.ReportProgress("Building delta", 0L, length);
			while (true)
			{
				long position = newFileStream.Position;
				int num2 = newFileStream.Read(array, 0, array.Length);
				if (num2 < 0)
				{
					break;
				}
				IRollingChecksum rollingChecksumAlgorithm = signature.RollingChecksumAlgorithm;
				uint num3 = 0u;
				int num4 = maxChunkSize;
				for (int i = 0; i < num2 - minChunkSize + 1; i++)
				{
					long num5 = position + i;
					int num6 = num2 - i;
					if (num6 < maxChunkSize)
					{
						num4 = minChunkSize;
					}
					if (i == 0 || num6 < maxChunkSize)
					{
						num3 = rollingChecksumAlgorithm.Calculate(array, i, num4);
					}
					else
					{
						byte remove = array[i - 1];
						byte add = array[i + num4 - 1];
						num3 = rollingChecksumAlgorithm.Rotate(num3, remove, add, num4);
					}
					ProgressReporter.ReportProgress("Building delta", num5, length);
					if (num5 - (num - num4) < num4 || !dictionary.ContainsKey(num3))
					{
						continue;
					}
					for (int j = dictionary[num3]; j < chunks.Count && chunks[j].RollingChecksum == num3; j++)
					{
						ChunkSignature chunkSignature = chunks[j];
						if (BinaryComparer.CompareArray(signature.HashAlgorithm.ComputeHash(array, i, num4), chunks[j].Hash))
						{
							num5 += num4;
							long num7 = num5 - num;
							if (num7 > num4)
							{
								deltaWriter.WriteDataCommand(newFileStream, num, num7 - num4);
							}
							deltaWriter.WriteCopyCommand(new DataRange(chunkSignature.StartOffset, chunkSignature.Length));
							num = num5;
							break;
						}
					}
				}
				if (num2 < array.Length)
				{
					break;
				}
				newFileStream.Position = newFileStream.Position - maxChunkSize + 1;
			}
			if (newFileStream.Length != num)
			{
				deltaWriter.WriteDataCommand(newFileStream, num, newFileStream.Length - num);
			}
			deltaWriter.Finish();
		}

		private static List<ChunkSignature> OrderChunksByChecksum(List<ChunkSignature> chunks)
		{
			chunks.Sort(new ChunkSignatureChecksumComparer());
			return chunks;
		}

		private Dictionary<uint, int> CreateChunkMap(IList<ChunkSignature> chunks, out int maxChunkSize, out int minChunkSize)
		{
			ProgressReporter.ReportProgress("Creating chunk map", 0L, chunks.Count);
			maxChunkSize = 0;
			minChunkSize = int.MaxValue;
			Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
			for (int i = 0; i < chunks.Count; i++)
			{
				ChunkSignature chunkSignature = chunks[i];
				if (chunkSignature.Length > maxChunkSize)
				{
					maxChunkSize = chunkSignature.Length;
				}
				if (chunkSignature.Length < minChunkSize)
				{
					minChunkSize = chunkSignature.Length;
				}
				if (!dictionary.ContainsKey(chunkSignature.RollingChecksum))
				{
					dictionary[chunkSignature.RollingChecksum] = i;
				}
				ProgressReporter.ReportProgress("Creating chunk map", i, chunks.Count);
			}
			return dictionary;
		}
	}
}
