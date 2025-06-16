using API;
using Games.HitCustomPhotoController.ScriptableObjects;
using Timer;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class HitCustomPhotoGameSessionManager : MonoBehaviour
	{
		[SerializeField]
		private AxcitementApiHandler api;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		[SerializeField]
		private HitCustomPhotoGameSessionSO hitCustomPhotoGameSession;

		[SerializeField]
		private TimerEventsSO timerEvents;

		private void OnEnable()
		{
			timerEvents.OnTimerStateChange += HandleTimerChangeState;
			hitCustomPhotoEvents.OnEndGameSession += EndGameSession;
		}

		private void OnDisable()
		{
			timerEvents.OnTimerStateChange -= HandleTimerChangeState;
			hitCustomPhotoEvents.OnEndGameSession -= EndGameSession;
		}

		private void HandleTimerChangeState(TimerStateEnum timerStateEnum)
		{
			if (timerStateEnum == TimerStateEnum.Running)
			{
				EndGameSession();
			}
		}

		private void EndGameSession()
		{
			string gameSessionID = hitCustomPhotoGameSession.GetGameSessionID();
			hitCustomPhotoGameSession.EndGameSession();
			if (!string.IsNullOrEmpty(gameSessionID))
			{
				StartCoroutine(api.DeleteGameSession(gameSessionID, delegate
				{
				}));
			}
		}
	}
}
