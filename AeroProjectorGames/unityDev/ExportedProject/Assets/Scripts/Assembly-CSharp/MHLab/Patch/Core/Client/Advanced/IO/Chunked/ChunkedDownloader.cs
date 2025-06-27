using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Serializing;
using MHLab.Patch.Core.Utilities.Asserts;

namespace MHLab.Patch.Core.Client.Advanced.IO.Chunked
{
	public class ChunkedDownloader : IDownloader
	{
		private readonly UpdatingContext _context;

		private readonly ChunkedDownloaderSettings _settings;

		private Task[] _runningTasks;

		private bool _isCanceled;

		private bool _isPaused;

		public NetworkCredential Credentials { get; set; }

		public IWebProxy Proxy { get; set; }

		public IDownloadSpeedMeter DownloadSpeedMeter { get; set; }

		public IDownloadMetrics DownloadMetrics { get; }

		public bool IsCanceled => _isCanceled;

		public bool IsPaused => _isPaused;

		public event EventHandler DownloadComplete;

		private static void ValidatePartialContentResponse(HttpWebResponse response)
		{
			Assert.AlwaysCheck(response.StatusCode == HttpStatusCode.PartialContent, "The server does not support chunked downloading. Consider using a different downloader.");
		}

		private static void ValidateResponse(HttpWebResponse response, DownloadableChunk chunk)
		{
			ValidatePartialContentResponse(response);
			Assert.AlwaysCheck(response.ContentLength == chunk.Size, $"The response from {chunk.DownloadEntry.RemoteUrl} has an unexpected size. Expected: {chunk.Size} - Actual: {response.ContentLength}");
		}

		private void StartJobs(DownloaderSharedContext sharedContext)
		{
			int chunksAmount = sharedContext.ChunkStorage.ChunksAmount;
			int num = Math.Min(_settings.TasksAmount, chunksAmount);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Restart();
			for (int i = 0; i < num; i++)
			{
				_runningTasks[i] = Task.Factory.StartNew(ProcessJob, sharedContext, TaskCreationOptions.LongRunning);
			}
			Task.WaitAll(_runningTasks.Where((Task t) => t != null).ToArray());
			stopwatch.Stop();
			_context.Logger.Info($"Downloaded and stored on disk [{chunksAmount - sharedContext.ChunkStorage.ChunksAmount}/{chunksAmount}] chunks in [{stopwatch.Elapsed.TotalSeconds}s] on [{num}] threads.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\IO\\Advanced\\Chunked\\ChunkedDownloader.cs", 61L, "StartJobs");
			if (_isCanceled)
			{
				_context.Logger.Info("Download canceled.", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\IO\\Advanced\\Chunked\\ChunkedDownloader.cs", 66L, "StartJobs");
			}
			else
			{
				this.DownloadComplete?.Invoke(this, null);
			}
			DownloadSpeedMeter.Reset();
		}

		private void ProcessJob(object state)
		{
			DownloaderSharedContext downloaderSharedContext = (DownloaderSharedContext)state;
			DownloaderContext downloaderContext = new DownloaderContext(downloaderSharedContext, new byte[_settings.MaxChunkSize]);
			while (!_isCanceled && !downloaderSharedContext.ChunkStorage.IsEmpty)
			{
				ChunkStorage.DownloadableChunkInfo chunkInfo;
				if (_isPaused)
				{
					Thread.Sleep(150);
				}
				else if (downloaderSharedContext.ChunkStorage.FetchNext(out chunkInfo))
				{
					if (chunkInfo.IsStarted)
					{
						downloaderSharedContext.Callbacks.OnEntryStarted?.Invoke(chunkInfo.Chunk.DownloadEntry);
					}
					long downloadedSize = DownloadChunk(downloaderContext, chunkInfo.Chunk);
					WriteToDisk(downloaderContext, chunkInfo.Chunk, downloadedSize);
					if (chunkInfo.IsLast)
					{
						downloaderSharedContext.Callbacks.OnEntryCompleted?.Invoke(chunkInfo.Chunk.DownloadEntry);
					}
				}
			}
			downloaderContext.Clear();
		}

		private long DownloadChunk(DownloaderContext context, DownloadableChunk chunk)
		{
			DownloaderSharedContext sharedContext = context.SharedContext;
			byte[] buffer = context.Buffer;
			int num = 0;
			while (num < _settings.MaxDownloadRetries)
			{
				if (_isCanceled)
				{
					return -1L;
				}
				HttpWebResponse httpWebResponse = null;
				try
				{
					httpWebResponse = DownloaderHelper.GetResponse(DownloaderHelper.GetRequest(chunk.DownloadEntry.RemoteUrl, chunk.OffsetStart, chunk.OffsetEnd, Proxy, Credentials));
					if (_context.Settings.DebugMode)
					{
						ValidateResponse(httpWebResponse, chunk);
					}
					Stream responseStream = httpWebResponse.GetResponseStream();
					Assert.AlwaysNotNull(responseStream, "The response stream for " + chunk.DownloadEntry.RemoteUrl + " is not valid.");
					long size = chunk.Size;
					int num2 = (int)size;
					int num3 = 0;
					int num4;
					while ((num4 = responseStream.Read(buffer, num3, num2)) > 0 && !_isCanceled)
					{
						num2 -= num4;
						num3 += num4;
						DownloadSpeedMeter.UpdateDownloadSpeed(num4);
						sharedContext.Callbacks.OnChunkDownloaded?.Invoke(num4);
					}
					Assert.AlwaysCheck(num3 == size, $"The read operation from the resource {chunk.DownloadEntry.RemoteUrl} has an unexpected size. Expected: {size} - Actual: {num3}");
					return num3;
				}
				catch (Exception ex)
				{
					num++;
					if (num >= _settings.MaxDownloadRetries)
					{
						_isCanceled = true;
						throw new WebException($"Tried to download [{chunk.DownloadEntry.RemoteUrl}] for [{num}] times, but failed. Reason: {ex.Message} - {ex.StackTrace}");
					}
					Thread.Sleep(_settings.DelayForRetry + _settings.DelayForRetry * num);
				}
				finally
				{
					httpWebResponse?.Close();
				}
			}
			throw new WebException($"Tried to download [{chunk.DownloadEntry.RemoteUrl}] for [{num}] times, but failed.");
		}

		private void WriteToDisk(DownloaderContext context, DownloadableChunk chunk, long downloadedSize)
		{
			Stream stream = context.GetStream(chunk, _context.FileSystem);
			stream.Seek(chunk.OffsetStart, SeekOrigin.Begin);
			stream.Write(context.Buffer, 0, (int)downloadedSize);
		}

		private void EnsureEmptyFilesOnDisk(List<DownloadEntry> entries, DownloaderCallbacks downloaderCallbacks)
		{
			foreach (DownloadEntry entry in entries)
			{
				if (entry.Definition.Size == 0)
				{
					downloaderCallbacks.OnEntryStarted?.Invoke(entry);
					EnsureEmptyFileOnDisk(entry);
					downloaderCallbacks.OnEntryCompleted?.Invoke(entry);
				}
			}
		}

		private void EnsureEmptyFileOnDisk(DownloadEntry entry)
		{
			_context.FileSystem.CreateDirectory((FilePath)entry.DestinationFolder);
			using (_context.FileSystem.CreateFile((FilePath)entry.DestinationFile))
			{
			}
		}

		public ChunkedDownloader(UpdatingContext context, ChunkedDownloaderSettings settings)
		{
			_context = context;
			_settings = settings;
			_runningTasks = new Task[settings.TasksAmount];
			DownloadMetrics = new SmartDownloadMetrics(_runningTasks);
			DownloadSpeedMeter = new SmartDownloadSpeedMeter(DownloadMetrics);
			InitializeServicePointManager();
		}

		public ChunkedDownloader(UpdatingContext context)
			: this(context, new ChunkedDownloaderSettings())
		{
		}

		private static void InitializeServicePointManager()
		{
			ServicePointManager.ServerCertificateValidationCallback = DownloaderHelper.RemoteCertificateValidationCallback;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.DefaultConnectionLimit = 1000;
			ServicePointManager.MaxServicePointIdleTime = 10000;
		}

		public void Download(List<DownloadEntry> entries, Action<DownloadEntry> onEntryStarted, Action<long> onChunkDownloaded, Action<DownloadEntry> onEntryCompleted)
		{
			_isCanceled = false;
			if (entries != null && entries.Count != 0)
			{
				DownloaderCallbacks downloaderCallbacks = new DownloaderCallbacks(onEntryStarted, onEntryCompleted, onChunkDownloaded);
				EnsureEmptyFilesOnDisk(entries, downloaderCallbacks);
				DownloaderSharedContext sharedContext = new DownloaderSharedContext
				{
					Callbacks = downloaderCallbacks,
					ChunkStorage = ChunkStorage.CalculateChunks(entries, _settings)
				};
				StartJobs(sharedContext);
				if (_context.Settings.DebugMode)
				{
					Assert.AlwaysCheck(DownloaderHelper.ValidateDownloadedResult(entries, _context.FileSystem, _context.Logger));
				}
			}
		}

		public void Download(string url, string destinationFolder)
		{
			Download(url, destinationFolder, null);
		}

		public void Download(string url, string destinationFolder, Action<long> onChunkDownloaded)
		{
			long contentLength = DownloaderHelper.GetResponse(DownloaderHelper.GetRequest(url, Proxy, Credentials)).ContentLength;
			FilePath filePath = _context.FileSystem.CombinePaths(destinationFolder, _context.FileSystem.GetFilename((FilePath)url));
			BuildDefinitionEntry definition = new BuildDefinitionEntry
			{
				Size = contentLength
			};
			DownloadEntry item = new DownloadEntry(url, null, null, filePath.FullPath, definition);
			List<DownloadEntry> entries = new List<DownloadEntry>(1) { item };
			Download(entries, null, onChunkDownloaded, null);
		}

		public T DownloadJson<T>(DownloadEntry entry, ISerializer serializer) where T : IJsonSerializable
		{
			string data = DownloadString(entry);
			T val = Activator.CreateInstance<T>();
			_context.Serializer.DeserializeOn(val, data);
			return val;
		}

		public string DownloadString(DownloadEntry entry)
		{
			using WebClient webClient = new WebClient();
			webClient.Credentials = Credentials;
			webClient.Proxy = Proxy;
			return webClient.DownloadString(entry.RemoteUrl);
		}

		public void Cancel()
		{
			_isCanceled = true;
		}

		public void Resume()
		{
			_isPaused = false;
		}

		public void Pause()
		{
			_isPaused = true;
		}
	}
}
