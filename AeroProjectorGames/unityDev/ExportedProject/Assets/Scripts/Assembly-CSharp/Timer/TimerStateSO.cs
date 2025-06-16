using UnityEngine;

namespace Timer
{
	[CreateAssetMenu(menuName = "Timer/Timer State")]
	public class TimerStateSO : ScriptableObject
	{
		private const int HOUR_IN_MINUTES = 60;

		private const int MAX_TIMER_VALUE_IN_MINUTES = 240;

		private const int MIN_TIMER_VALUE_IN_MINUTES = 1;

		private const int SECONDS_IN_MINUTES = 60;

		private int _maxTimeInSeconds = 14400;

		[SerializeField]
		private TimerEventsSO timerEvents;

		public TimerStateEnum CurrentState { get; private set; } = TimerStateEnum.Stopped;

		public int MaxTimerValue { get; private set; } = 240;

		public float TimeRemaining { get; private set; }

		public void AddMinutes(int minutesToAdd)
		{
			float num = TimeRemaining / 60f + (float)minutesToAdd;
			if (num < (float)MaxTimerValue)
			{
				TimeRemaining = num * 60f;
			}
			else
			{
				TimeRemaining = _maxTimeInSeconds;
			}
		}

		public void DisableTimer()
		{
			ResetTimer();
			CurrentState = TimerStateEnum.Disabled;
			timerEvents.RaiseDisabledTimer(isDisabled: true);
		}

		public void EnableTimer()
		{
			ResetTimer();
			CurrentState = TimerStateEnum.Stopped;
			timerEvents.RaiseDisabledTimer(isDisabled: false);
		}

		public void PauseTimer()
		{
			if (CurrentState != TimerStateEnum.Disabled)
			{
				CurrentState = TimerStateEnum.Paused;
				timerEvents.RaiseTimerStateChange(CurrentState);
			}
		}

		private void ResetTimer()
		{
			CurrentState = TimerStateEnum.Stopped;
			TimeRemaining = 0f;
		}

		public void SetMaximumMinutes(int timerUpperBoundsInMinutes)
		{
			MaxTimerValue = timerUpperBoundsInMinutes;
			_maxTimeInSeconds = MaxTimerValue * 60;
		}

		public void StarTimer()
		{
			if (CurrentState != TimerStateEnum.Disabled && !(TimeRemaining <= 0f))
			{
				CurrentState = TimerStateEnum.Running;
				timerEvents.RaiseTimerStateChange(CurrentState);
			}
		}

		public void StarTimer(int minutes)
		{
			if (CurrentState != TimerStateEnum.Disabled)
			{
				CurrentState = TimerStateEnum.Running;
				AddMinutes(minutes);
				timerEvents.RaiseTimerStateChange(CurrentState);
			}
		}

		public void StopTimer()
		{
			if (CurrentState != TimerStateEnum.Disabled)
			{
				CurrentState = TimerStateEnum.Stopped;
				TimeRemaining = 0f;
				timerEvents.RaiseTimerStateChange(CurrentState);
			}
		}

		public void SubtractTimeRemaining(float deltaTime)
		{
			if (CurrentState != TimerStateEnum.Disabled)
			{
				TimeRemaining -= deltaTime;
			}
		}
	}
}
