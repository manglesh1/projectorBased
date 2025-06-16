using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core.Client.IO
{
	public class FileDownloader : IDownloader
	{
		public const int DownloadBlockSize = 32768;

		public const int MaxDownloadRetries = 10;

		public const int DelayForRetryMilliseconds = 50;

		protected readonly IFileSystem FileSystem;

		public NetworkCredential Credentials { get; set; }

		public IDownloadSpeedMeter DownloadSpeedMeter { get; set; }

		public IDownloadMetrics DownloadMetrics { get; set; }

		public IWebProxy Proxy { get; set; }

		public bool IsCanceled { get; private set; }

		public bool IsPaused { get; private set; }

		public event EventHandler DownloadComplete;

		public FileDownloader(IFileSystem fileSystem)
		{
			FileSystem = fileSystem;
			DownloadSpeedMeter = new DownloadSpeedMeter();
			DownloadMetrics = new DownloadMetrics
			{
				RunningThreads = 1
			};
		}

		private void OnDownloadComplete()
		{
			if (this.DownloadComplete != null)
			{
				this.DownloadComplete(this, new EventArgs());
			}
		}

		public virtual void Download(List<DownloadEntry> entries, Action<DownloadEntry> onEntryStarted, Action<long> onChunkDownloaded, Action<DownloadEntry> onEntryCompleted)
		{
			IsCanceled = false;
			foreach (DownloadEntry entry in entries)
			{
				onEntryStarted?.Invoke(entry);
				Download(entry.RemoteUrl, entry.DestinationFolder, onChunkDownloaded);
				onEntryCompleted?.Invoke(entry);
			}
			DownloadSpeedMeter.Reset();
		}

		public virtual void Download(string url, string destFolder)
		{
			Download(url, destFolder, null);
		}

		public virtual void Download(string url, string destFolder, Action<long> onChunkDownloaded)
		{
			string filename = FileSystem.GetFilename((FilePath)url);
			destFolder = destFolder.Replace("file:///", "").Replace("file://", "");
			FilePath path = new FilePath(destFolder, FileSystem.CombinePaths(destFolder, filename).FullPath);
			FileSystem.CreateDirectory((FilePath)destFolder);
			if (!FileSystem.FileExists(path))
			{
				using (Stream stream = FileSystem.GetFileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
				{
					stream.Flush();
					stream.Dispose();
					stream.Close();
				}
			}
			byte[] buffer = new byte[32768];
			bool flag = false;
			using (Stream stream2 = FileSystem.GetFileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				int num = 0;
				while (num < 10)
				{
					DownloadData downloadData = null;
					try
					{
						long num2 = 0L;
						try
						{
							downloadData = DownloadData.Create(url, stream2, Credentials, Proxy);
							if (downloadData.Start > 0)
							{
								onChunkDownloaded?.Invoke(downloadData.Start);
							}
							int num3;
							while ((num3 = downloadData.DownloadStream.Read(buffer, 0, 32768)) > 0)
							{
								if (IsCanceled)
								{
									flag = true;
									downloadData.Close();
									break;
								}
								num2 += num3;
								SaveToFile(buffer, num3, stream2, path.FullPath);
								DownloadSpeedMeter.UpdateDownloadSpeed(num3);
								onChunkDownloaded?.Invoke(num3);
								if (IsCanceled)
								{
									flag = true;
									downloadData.Close();
									break;
								}
								while (IsPaused)
								{
									Thread.Sleep(150);
								}
							}
							num = 10;
						}
						catch
						{
							num++;
							onChunkDownloaded?.Invoke(-num2);
							if (num >= 10)
							{
								stream2.Dispose();
								stream2.Close();
								throw new WebException("All retries have been tried for " + url + ".");
							}
							Thread.Sleep(50 + 50 * num);
						}
					}
					catch (WebException innerException)
					{
						throw new WebException("The URL " + url + " generated an exception.", innerException);
					}
					catch (UriFormatException innerException2)
					{
						throw new ArgumentException($"Could not parse the URL \"{url}\" - it's either malformed or is an unknown protocol.", innerException2);
					}
					finally
					{
						downloadData?.Close();
					}
				}
				stream2.Flush();
				stream2.Dispose();
				stream2.Close();
			}
			if (!flag)
			{
				OnDownloadComplete();
			}
		}

		public virtual T DownloadJson<T>(DownloadEntry entry, ISerializer serializer) where T : IJsonSerializable
		{
			string data = DownloadString(entry);
			return serializer.Deserialize<T>(data);
		}

		public virtual string DownloadString(DownloadEntry entry)
		{
			ServicePointManager.ServerCertificateValidationCallback = DownloaderHelper.RemoteCertificateValidationCallback;
			using (WebClient webClient = new WebClient())
			{
				webClient.Credentials = Credentials;
				try
				{
					return Encoding.UTF8.GetString(webClient.DownloadData(entry.RemoteUrl));
				}
				catch (WebException innerException)
				{
					throw new WebException("The URL " + entry.RemoteUrl + " generated an exception.", innerException);
				}
			}
		}

		public virtual void Cancel()
		{
			IsCanceled = true;
		}

		public virtual void Pause()
		{
			IsPaused = true;
		}

		public virtual void Resume()
		{
			IsPaused = false;
		}

		private void SaveToFile(byte[] buffer, int count, Stream file, string fileName)
		{
			try
			{
				file.Write(buffer, 0, count);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error trying to save file \"{fileName}\": {ex.Message}", ex);
			}
		}
	}
}
