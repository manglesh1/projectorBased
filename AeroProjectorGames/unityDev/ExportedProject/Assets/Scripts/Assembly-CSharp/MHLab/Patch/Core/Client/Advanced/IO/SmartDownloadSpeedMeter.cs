using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.Utilities;

namespace MHLab.Patch.Core.Client.Advanced.IO
{
	public class SmartDownloadSpeedMeter : DownloadSpeedMeter
	{
		private readonly IDownloadMetrics _metrics;

		public override string FormattedDownloadSpeed => FormatUtility.FormatSizeDecimal(base.DownloadSpeed, 2) + $"/s on {_metrics.RunningThreads} workers";

		public SmartDownloadSpeedMeter(IDownloadMetrics metrics)
		{
			_metrics = metrics;
		}
	}
}
