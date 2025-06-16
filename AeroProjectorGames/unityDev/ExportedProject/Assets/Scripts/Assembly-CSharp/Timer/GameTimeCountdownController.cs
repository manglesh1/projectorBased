using UnityEngine;

namespace Timer
{
	public class GameTimeCountdownController : MonoBehaviour
	{
		private float _secondsRemaining;

		[SerializeField]
		private TimerStateSO timerState;

		private void OnEnable()
		{
			timerState.StopTimer();
		}

		private void Update()
		{
			if (timerState.CurrentState == TimerStateEnum.Running)
			{
				timerState.SubtractTimeRemaining(Time.deltaTime);
				if (timerState.TimeRemaining <= 0f)
				{
					timerState.StopTimer();
				}
			}
		}
	}
}
