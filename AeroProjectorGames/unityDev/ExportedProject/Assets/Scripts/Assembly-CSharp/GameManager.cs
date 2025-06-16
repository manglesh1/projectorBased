using System.Collections;
using Games;
using Games.GameState;
using Games.Models;
using Stats;
using Timer;
using UI;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
	private const int AMOUNT_TO_MOVE_GAME_IN_X_TO_HIDE = 1000;

	private const int TARGET_FRAMERATE = 60;

	private const int RENDER_FRAME_INTERVAL = 2;

	private GameObject _currentGame;

	[Header("End of Time Actions Assets")]
	[SerializeField]
	private GameStateSO gameState;

	[SerializeField]
	private TimerEventsSO timerEvents;

	[Header("Game Loading Assets")]
	[SerializeField]
	private GameEventsSO gameEvents;

	[SerializeField]
	private GameObject parentForGame;

	[SerializeField]
	private UIManager uiManager;

	[Header("Telemetry")]
	[SerializeField]
	private TelemetryEventsSO telemetryEvents;

	private void Awake()
	{
		Application.targetFrameRate = 60;
		OnDemandRendering.renderFrameInterval = 2;
		gameEvents.OnMainMenu += UnloadGame;
		timerEvents.OnTimerStateChange += EndOfTimerActions;
	}

	private void OnDestroy()
	{
		gameEvents.OnMainMenu -= UnloadGame;
		timerEvents.OnTimerStateChange -= EndOfTimerActions;
	}

	public void LoadGame(GameObject gamePrefab, GameSO game)
	{
		if ((bool)_currentGame)
		{
			UnloadGame();
		}
		gameEvents.RaiseCheckAndDestroySubMenu();
		uiManager.ShowGameView();
		gameState.SetLoadedGame(game);
		gameEvents.RaiseGameLoading();
		if (gamePrefab.tag.Contains("2D-Game"))
		{
			_currentGame = Object.Instantiate(gamePrefab, parentForGame.transform, worldPositionStays: true);
			_currentGame.transform.localScale = new Vector3(1f, 1f, _currentGame.transform.localScale.z);
			_currentGame.transform.localPosition = new Vector3(0f, 0f);
		}
		else
		{
			_currentGame = Object.Instantiate(gamePrefab, parentForGame.transform);
		}
		telemetryEvents.RaiseGameStartedTelemetry(game.GameId);
		gameEvents.RaiseGameLoaded();
	}

	public void UnloadGame()
	{
		if (_currentGame != null)
		{
			_currentGame.transform.Translate(-1000f, 0f, 0f);
			Object.Destroy(_currentGame);
		}
		_currentGame = null;
		gameState.SetLoadedGame(null);
	}

	private void EndOfTimerActions(TimerStateEnum timerStateEnum)
	{
		if (timerStateEnum == TimerStateEnum.Stopped && parentForGame.transform.childCount != 0)
		{
			switch (timerEvents.SelectedAction)
			{
			case EndOfTimerActionStates.FinishGame:
				break;
			case EndOfTimerActionStates.FreezeGame:
				StartCoroutine(WaitToDisableGame());
				break;
			case EndOfTimerActionStates.HideGameboard:
				StartCoroutine(WaitToDisableGame());
				if (_currentGame != null)
				{
					_currentGame.transform.Translate(1000f, 0f, 0f);
				}
				break;
			}
		}
		else if (timerStateEnum == TimerStateEnum.Running && parentForGame.transform.childCount != 0)
		{
			if (_currentGame != null && timerEvents.SelectedAction == EndOfTimerActionStates.HideGameboard)
			{
				_currentGame.transform.Translate(-1000f, 0f, 0f);
			}
			gameState.EnableTarget();
		}
	}

	private IEnumerator WaitToDisableGame()
	{
		StartCoroutine(WaitToStopCoroutine());
		yield return new WaitUntil(() => !gameState.IsTargetDisabled);
		gameState.DisableTarget();
	}

	private IEnumerator WaitToStopCoroutine()
	{
		yield return new WaitForSecondsRealtime(10f);
		StopCoroutine(WaitToDisableGame());
	}
}
