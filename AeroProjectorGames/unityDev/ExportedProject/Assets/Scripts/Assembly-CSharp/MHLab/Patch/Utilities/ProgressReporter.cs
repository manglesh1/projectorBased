using System;
using MHLab.Patch.Core.Client.Progresses;

namespace MHLab.Patch.Utilities
{
	public sealed class ProgressReporter : IProgress<UpdateProgress>
	{
		public ProgressReporterEvent ProgressChanged;

		public ProgressReporter()
		{
			ProgressChanged = new ProgressReporterEvent();
		}

		private void OnProgressChanged(UpdateProgress progress)
		{
			ProgressReporterEvent progressChanged = ProgressChanged;
			if (progressChanged != null)
			{
				try
				{
					progressChanged.Invoke(progress);
				}
				catch
				{
				}
			}
		}

		public void Report(UpdateProgress value)
		{
			OnProgressChanged(value);
		}
	}
}
