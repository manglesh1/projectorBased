using System;
using System.Collections.Generic;
using System.Net;
using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core.Client.IO
{
	public interface IDownloader
	{
		NetworkCredential Credentials { get; set; }

		IWebProxy Proxy { get; set; }

		IDownloadSpeedMeter DownloadSpeedMeter { get; set; }

		IDownloadMetrics DownloadMetrics { get; }

		bool IsCanceled { get; }

		bool IsPaused { get; }

		event EventHandler DownloadComplete;

		void Download(List<DownloadEntry> entries, Action<DownloadEntry> onEntryStarted, Action<long> onChunkDownloaded, Action<DownloadEntry> onEntryCompleted);

		void Download(string url, string destinationFolder);

		void Download(string url, string destinationFolder, Action<long> onChunkDownloaded);

		T DownloadJson<T>(DownloadEntry entry, ISerializer serializer) where T : IJsonSerializable;

		string DownloadString(DownloadEntry entry);

		void Cancel();

		void Pause();

		void Resume();
	}
}
