using System.Threading;

namespace MHLab.Patch.Core.Client.Progresses
{
	public class UpdateProgress
	{
		private long _currentSteps;

		public long TotalSteps { get; set; }

		public long CurrentSteps
		{
			get
			{
				return _currentSteps;
			}
			set
			{
				if (value > TotalSteps)
				{
					_currentSteps = TotalSteps;
				}
				_currentSteps = value;
			}
		}

		public string StepMessage { get; set; }

		public void IncrementStep(long increment)
		{
			Interlocked.Add(ref _currentSteps, increment);
		}
	}
}
