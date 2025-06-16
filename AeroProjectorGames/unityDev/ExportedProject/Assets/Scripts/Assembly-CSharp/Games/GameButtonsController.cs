using Timer;
using UnityEngine;

namespace Games
{
	public class GameButtonsController : MonoBehaviour
	{
		private bool _timerStopped;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private TimerEventsSO timerEvents;

		private void OnEnable()
		{
			_timerStopped = false;
			timerEvents.OnTimerStateChange += TimerStateChanged;
		}

		private void OnDisable()
		{
			timerEvents.OnTimerStateChange -= TimerStateChanged;
		}

		public void RaiseMiss()
		{
			gameEvents.RaiseMiss();
		}

		public void RaiseGameOver()
		{
			gameEvents.RaiseGameOver();
		}

		public void RaiseNewGame()
		{
			if (!_timerStopped)
			{
				gameEvents.RaiseNewGame();
			}
		}

		public void RaiseMainMenu()
		{
			gameEvents.RaiseMainMenu();
		}

		public void RaiseUndo()
		{
			gameEvents.RaiseUndo();
		}

		private void TimerStateChanged(TimerStateEnum timerStateEnum)
		{
			_timerStopped = timerStateEnum == TimerStateEnum.Stopped;
		}
	}
}
