using System;
using System.Collections.Generic;

namespace MHLab.Patch.Core.Client.Runners
{
	public class UpdateRunner : IUpdateRunner
	{
		private readonly List<IUpdater> _steps;

		public event EventHandler<IUpdater> StartedStep;

		public event EventHandler<IUpdater> PerformedStep;

		public UpdateRunner()
		{
			_steps = new List<IUpdater>();
		}

		public void Update()
		{
			foreach (IUpdater step in _steps)
			{
				OnStartedStep(step);
				step.Update();
				OnPerformedStep(step);
			}
		}

		public void RegisterStep<T>(T step) where T : IUpdater
		{
			_steps.Add(step);
		}

		public long GetProgressAmount()
		{
			long num = 0L;
			foreach (IUpdater step in _steps)
			{
				num += step.ProgressRangeAmount();
			}
			return num;
		}

		private void OnStartedStep(IUpdater updater)
		{
			this.StartedStep?.Invoke(this, updater);
		}

		private void OnPerformedStep(IUpdater updater)
		{
			this.PerformedStep?.Invoke(this, updater);
		}
	}
}
