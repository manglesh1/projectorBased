using System;
using System.Collections.Generic;
using System.Threading;
using MHLab.Patch.Core.Utilities;

namespace MHLab.Patch.Core.Client.IO
{
	public class DownloadSpeedMeter : IDownloadSpeedMeter
	{
		private const int DownloadSpeedHistorySize = 5;

		private long _downloadSpeed;

		private long _maxReachedDownloadSpeed;

		private long _accumulator;

		private DateTime _lastTime = DateTime.UtcNow;

		private Queue<long> _accumulatorQueue;

		public virtual string FormattedDownloadSpeed => FormatUtility.FormatSizeDecimal(_downloadSpeed, 2) + "/s";

		public virtual string FormattedMaxReachedSpeed => FormatUtility.FormatSizeDecimal(_maxReachedDownloadSpeed, 2) + "/s";

		public long DownloadSpeed => _downloadSpeed;

		public long MaxReachedDownloadSpeed => _maxReachedDownloadSpeed;

		public DownloadSpeedMeter()
		{
			_accumulatorQueue = new Queue<long>(5);
		}

		public void UpdateDownloadSpeed(long additionalDownloadedSize)
		{
			Interlocked.Add(ref _accumulator, additionalDownloadedSize);
		}

		public void Tick()
		{
			long num = Interlocked.Exchange(ref _accumulator, 0L);
			if (_accumulatorQueue.Count >= 5)
			{
				_accumulatorQueue.Dequeue();
			}
			_accumulatorQueue.Enqueue((long)((double)num / (DateTime.UtcNow - _lastTime).TotalSeconds));
			long num2 = 0L;
			foreach (long item in _accumulatorQueue)
			{
				num2 += item;
			}
			_downloadSpeed = num2 / _accumulatorQueue.Count;
			if (_downloadSpeed > _maxReachedDownloadSpeed)
			{
				_maxReachedDownloadSpeed = _downloadSpeed;
			}
			_lastTime = DateTime.UtcNow;
		}

		public void Reset()
		{
			_downloadSpeed = 0L;
			_lastTime = DateTime.UtcNow;
		}
	}
}
