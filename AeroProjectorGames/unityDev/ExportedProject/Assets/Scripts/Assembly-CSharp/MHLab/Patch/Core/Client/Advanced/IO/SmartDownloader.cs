using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.Utilities.Asserts;

namespace MHLab.Patch.Core.Client.Advanced.IO
{
	public class SmartDownloader : FileDownloader
	{
		private static readonly int TasksAmount = Math.Max(1, Math.Min(8, Environment.ProcessorCount));

		private Task[] _tasks;

		private readonly Stopwatch _stopwatch;

		private readonly UpdatingContext _context;

		public SmartDownloader(UpdatingContext context)
			: base(context.FileSystem)
		{
			_tasks = new Task[TasksAmount];
			_stopwatch = new Stopwatch();
			_context = context;
			base.DownloadMetrics = new SmartDownloadMetrics(_tasks);
			base.DownloadSpeedMeter = new SmartDownloadSpeedMeter(base.DownloadMetrics);
		}

		public override void Download(List<DownloadEntry> entries, Action<DownloadEntry> onEntryStarted, Action<long> onChunkDownloaded, Action<DownloadEntry> onEntryCompleted)
		{
			_stopwatch.Restart();
			entries.Sort((DownloadEntry entry1, DownloadEntry entry2) => entry1.Definition.Size.CompareTo(entry2.Definition.Size));
			_stopwatch.Stop();
			_context.Logger.Info($"Sorted {entries.Count} files in [{_stopwatch.Elapsed.TotalSeconds}s].", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\IO\\Advanced\\SmartDownloader.cs", 37L, "Download");
			Queue<DownloadEntry> queue = new Queue<DownloadEntry>(entries);
			_stopwatch.Restart();
			for (int num = 0; num < TasksAmount; num++)
			{
				Task task = Task.Factory.StartNew(delegate
				{
					Action<DownloadEntry> action = onEntryStarted;
					Action<DownloadEntry> action2 = onEntryCompleted;
					while (true)
					{
						bool flag = true;
						DownloadEntry downloadEntry;
						do
						{
							lock (queue)
							{
								if (queue.Count == 0)
								{
									return;
								}
								downloadEntry = queue.Dequeue();
							}
							if (base.IsCanceled)
							{
								return;
							}
							action?.Invoke(downloadEntry);
							int num2 = 0;
							while (num2 < 10)
							{
								try
								{
									Download(downloadEntry.RemoteUrl, downloadEntry.DestinationFolder, onChunkDownloaded);
									num2 = 10;
								}
								catch (Exception)
								{
									num2++;
									if (num2 >= 10)
									{
										throw new WebException("All retries have been tried for " + downloadEntry.RemoteUrl + ".");
									}
									Thread.Sleep(50 + 50 * num2);
								}
							}
						}
						while (action2 == null);
						action2(downloadEntry);
					}
				}, TaskCreationOptions.LongRunning);
				_tasks[num] = task;
			}
			Task.WaitAll(_tasks);
			_stopwatch.Stop();
			_context.Logger.Info($"Downloaded {entries.Count} files in [{_stopwatch.Elapsed.TotalSeconds}s].", "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\IO\\Advanced\\SmartDownloader.cs", 97L, "Download");
			if (_context.Settings.DebugMode)
			{
				Assert.AlwaysCheck(DownloaderHelper.ValidateDownloadedResult(entries, FileSystem, _context.Logger));
			}
			base.DownloadSpeedMeter.Reset();
		}
	}
}
