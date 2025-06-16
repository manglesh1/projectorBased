using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MHLab.Patch.Core.Client.IO;

namespace MHLab.Patch.Core.Client.Advanced.IO.Chunked
{
	public class ChunkStorage
	{
		public readonly struct DownloadableChunkInfo
		{
			public readonly DownloadableChunk Chunk;

			public readonly bool IsStarted;

			public readonly bool IsLast;

			public DownloadableChunkInfo(DownloadableChunk chunk, bool isStarted, bool isLast)
			{
				Chunk = chunk;
				IsLast = isLast;
				IsStarted = isStarted;
			}
		}

		private readonly ConcurrentQueue<DownloadableChunk> _downloadQueue;

		private readonly Dictionary<DownloadEntry, int> _remainingChunks;

		private readonly HashSet<DownloadEntry> _startedEntries;

		public bool IsEmpty => _downloadQueue.IsEmpty;

		public int ChunksAmount => _downloadQueue.Count;

		public ChunkStorage()
		{
			_downloadQueue = new ConcurrentQueue<DownloadableChunk>();
			_remainingChunks = new Dictionary<DownloadEntry, int>();
			_startedEntries = new HashSet<DownloadEntry>();
		}

		public void EnqueueDownloadableChunk(DownloadableChunk chunk)
		{
			Dictionary<DownloadEntry, int> remainingChunks = _remainingChunks;
			lock (remainingChunks)
			{
				if (_remainingChunks.TryGetValue(chunk.DownloadEntry, out var value))
				{
					_remainingChunks[chunk.DownloadEntry] = value + 1;
				}
				else
				{
					_remainingChunks.Add(chunk.DownloadEntry, 1);
				}
			}
			_downloadQueue.Enqueue(chunk);
		}

		public DownloadableChunk[] GetDownloadableChunks()
		{
			return _downloadQueue.ToArray();
		}

		public bool FetchNext(out DownloadableChunkInfo chunkInfo)
		{
			if (_downloadQueue.TryDequeue(out var result))
			{
				bool isStarted = false;
				bool isLast = false;
				HashSet<DownloadEntry> startedEntries = _startedEntries;
				lock (startedEntries)
				{
					if (!_startedEntries.Contains(result.DownloadEntry))
					{
						_startedEntries.Add(result.DownloadEntry);
						isStarted = true;
					}
				}
				Dictionary<DownloadEntry, int> remainingChunks = _remainingChunks;
				lock (remainingChunks)
				{
					if (_remainingChunks.ContainsKey(result.DownloadEntry))
					{
						int num = _remainingChunks[result.DownloadEntry];
						_remainingChunks[result.DownloadEntry] = num - 1;
						if (num <= 1)
						{
							isLast = true;
						}
					}
					else
					{
						isLast = true;
					}
				}
				chunkInfo = new DownloadableChunkInfo(result, isStarted, isLast);
				return true;
			}
			chunkInfo = default(DownloadableChunkInfo);
			return false;
		}

		public static ChunkStorage CalculateChunks(List<DownloadEntry> entries, ChunkedDownloaderSettings settings)
		{
			ChunkStorage chunkStorage = new ChunkStorage();
			foreach (DownloadEntry entry in entries)
			{
				long size = entry.Definition.Size;
				long val = size / settings.TasksAmount;
				val = Math.Max(val, settings.ChunkSize);
				val = Math.Min(val, settings.MaxChunkSize);
				for (long num = 0L; num < size; num += val)
				{
					long offsetStart = num;
					long val2 = num + val - 1;
					val2 = Math.Min(size - 1, val2);
					DownloadableChunk chunk = new DownloadableChunk(offsetStart, val2, entry);
					chunkStorage.EnqueueDownloadableChunk(chunk);
				}
			}
			return chunkStorage;
		}
	}
}
