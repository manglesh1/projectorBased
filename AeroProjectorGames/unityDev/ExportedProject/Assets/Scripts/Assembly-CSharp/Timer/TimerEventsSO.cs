using UnityEngine;
using UnityEngine.Events;

namespace Timer
{
	[CreateAssetMenu(menuName = "Timer/Timer Events")]
	public class TimerEventsSO : ScriptableObject
	{
		public EndOfTimerActionStates SelectedAction { get; set; }

		public event UnityAction OnDisplayTimerSettings;

		public event UnityAction<bool> OnDisabledTimer;

		public event UnityAction OnStartTimerWarningRequest;

		public event UnityAction OnTimeIncrementsUpdated;

		public event UnityAction<TimerStateEnum> OnTimerStateChange;

		public void RaiseDisplayTimerSettings()
		{
			this.OnDisplayTimerSettings?.Invoke();
		}

		public void RaiseDisabledTimer(bool isDisabled)
		{
			this.OnDisabledTimer?.Invoke(isDisabled);
		}

		public void RaiseStartTimerWarning()
		{
			this.OnStartTimerWarningRequest?.Invoke();
		}

		public void RaiseTimeIncrementsUpdated()
		{
			this.OnTimeIncrementsUpdated?.Invoke();
		}

		public void RaiseTimerStateChange(TimerStateEnum newStateEnum)
		{
			this.OnTimerStateChange?.Invoke(newStateEnum);
		}
	}
}
