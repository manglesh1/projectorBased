using System.Collections.Generic;
using System.IO;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.IO;

namespace MHLab.Patch.Core.Client.Advanced.IO.Chunked
{
	public class DownloaderContext
	{
		public DownloaderSharedContext SharedContext { get; private set; }

		public Dictionary<DownloadEntry, Stream> Streams { get; private set; }

		public byte[] Buffer { get; private set; }

		public DownloaderContext(DownloaderSharedContext sharedContext, byte[] buffer)
		{
			SharedContext = sharedContext;
			Buffer = buffer;
			Streams = new Dictionary<DownloadEntry, Stream>();
		}

		public Stream GetStream(DownloadableChunk chunk, IFileSystem fileSystem)
		{
			EnsureStream(chunk, fileSystem);
			return Streams[chunk.DownloadEntry];
		}

		private void EnsureStream(DownloadableChunk chunk, IFileSystem fileSystem)
		{
			if (!Streams.ContainsKey(chunk.DownloadEntry))
			{
				FilePath path = (FilePath)chunk.DownloadEntry.DestinationFile;
				fileSystem.CreateDirectory(fileSystem.GetDirectoryPath(path));
				Stream fileStream = fileSystem.GetFileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
				if (fileStream.Length != chunk.DownloadEntry.Definition.Size)
				{
					fileStream.SetLength(chunk.DownloadEntry.Definition.Size);
				}
				Streams.Add(chunk.DownloadEntry, fileStream);
			}
		}

		public void Clear()
		{
			foreach (KeyValuePair<DownloadEntry, Stream> stream in Streams)
			{
				Stream value = stream.Value;
				value.Flush();
				value.Dispose();
			}
		}
	}
}
