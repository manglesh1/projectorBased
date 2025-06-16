using Games;
using Games.GameState;
using Timer;
using UnityEngine;

namespace UI.MultiDisplay
{
	public class TimerOverlayController : MonoBehaviour
	{
		[Header("Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private TimerEventsSO timerEvents;

		[Header("Game State")]
		[SerializeField]
		private GameStateSO gameState;

		[Header("Timer")]
		[SerializeField]
		private GameObject[] timerOverlays;

		[SerializeField]
		private TimerStateSO timerState;

		private void OnDisable()
		{
			gameEvents.OnGameLoaded -= DetermineTimerOverlayVisibility;
			gameEvents.OnMainMenu -= DetermineTimerOverlayVisibility;
			timerEvents.OnTimerStateChange -= HandleTimerStateChange;
			timerEvents.OnDisabledTimer -= HandleTimerDisabled;
		}

		private void OnEnable()
		{
			gameEvents.OnGameLoaded += DetermineTimerOverlayVisibility;
			gameEvents.OnMainMenu += DetermineTimerOverlayVisibility;
			timerEvents.OnTimerStateChange += HandleTimerStateChange;
			timerEvents.OnDisabledTimer += HandleTimerDisabled;
		}

		private void DetermineTimerOverlayVisibility()
		{
			if (timerState.CurrentState == TimerStateEnum.Disabled)
			{
				SetTimerOverlaysActive(active: false);
			}
			else
			{
				SetTimerOverlaysActive(gameState.LoadedGame != null && timerState.CurrentState != TimerStateEnum.Running);
			}
		}

		private void HandleTimerDisabled(bool _)
		{
			DetermineTimerOverlayVisibility();
		}

		private void HandleTimerStateChange(TimerStateEnum _)
		{
			DetermineTimerOverlayVisibility();
		}

		private void SetTimerOverlaysActive(bool active)
		{
			GameObject[] array = timerOverlays;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(active);
			}
		}
	}
}
