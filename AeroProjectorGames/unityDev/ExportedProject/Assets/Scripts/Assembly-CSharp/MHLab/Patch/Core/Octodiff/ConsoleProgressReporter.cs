using System;

namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class ConsoleProgressReporter : IProgressReporter
	{
		private string currentOperation;

		private int progressPercentage;

		public void ReportProgress(string operation, long currentPosition, long total)
		{
			int num = (int)((double)currentPosition / (double)total * 100.0 + 0.5);
			if (currentOperation != operation)
			{
				progressPercentage = -1;
				currentOperation = operation;
			}
			if (progressPercentage != num && num % 10 == 0)
			{
				progressPercentage = num;
				Console.WriteLine("{0}: {1}%", currentOperation, num);
			}
		}
	}
}
