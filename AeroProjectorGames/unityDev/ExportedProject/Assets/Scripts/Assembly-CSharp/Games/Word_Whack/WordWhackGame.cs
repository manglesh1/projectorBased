using System.Collections;
using Games.GameState;
using Games.Word_Whack.Scripts;
using Scoreboard;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Word_Whack
{
	public class WordWhackGame : MonoBehaviour
	{
		private Coroutine _enableTargetAfterDelayCoroutine;

		private bool _solving;

		[Header("Scoring Logic")]
		[SerializeField]
		private WordWhackScoringLogic scoringLogic;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[SerializeField]
		private WordWhackEventsSO wordWhackEvents;

		[Header("Multi-Display")]
		[SerializeField]
		private GameObject multiDisplayScoringObject;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		private void OnDisable()
		{
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnUndo -= HandleUndo;
			wordWhackEvents.OnSolveAttemptCompleted -= HandleSolveAttemptCompleted;
			wordWhackEvents.OnCharacterHit -= HandleCharacterHit;
			wordWhackEvents.OnSolveAttemptRequest -= HandleSolveAttemptRequest;
			wordWhackEvents.OnSolveAttemptCancelled -= HandleSolveAttemptCancelled;
		}

		private void OnEnable()
		{
			gameState.NumberOfRounds = NumberOfRounds.Infinite;
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.WordWhack);
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnUndo += HandleUndo;
			wordWhackEvents.OnSolveAttemptCompleted += HandleSolveAttemptCompleted;
			wordWhackEvents.OnCharacterHit += HandleCharacterHit;
			wordWhackEvents.OnSolveAttemptRequest += HandleSolveAttemptRequest;
			wordWhackEvents.OnSolveAttemptCancelled += HandleSolveAttemptCancelled;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayScoringObject);
			}
		}

		private void Start()
		{
			HandleNewGame();
		}

		private IEnumerator EnableTargetAfterDelay()
		{
			yield return new WaitForSeconds(2f);
			gameState.EnableTarget();
		}

		private void HandleGameOver()
		{
			gameState.GameStatus = GameStatus.Finished;
			gameState.DisableTarget();
			wordWhackEvents.RaisePhraseSolved();
			gameEvents.RaiseGameOver();
			gameEvents.RaiseWinAnimation();
		}

		private void HandleCharacterHit(char character)
		{
			gameState.DisableTarget();
			scoringLogic.RecordCharacter(character);
			if (scoringLogic.HasPlayerWon())
			{
				HandleGameOver();
				return;
			}
			if (!scoringLogic.CharacterFound(character))
			{
				wordWhackEvents.RaiseNotFoundAnimation();
				wordWhackEvents.RaiseOnCharacterNotFound(character);
				scoringLogic.NextPlayer();
				gameEvents.RaiseUpdateScoreboard();
			}
			else
			{
				wordWhackEvents.RaiseFoundAnimation();
				wordWhackEvents.RaiseOnCharacterFound(character);
			}
			_enableTargetAfterDelayCoroutine = StartCoroutine(EnableTargetAfterDelay());
		}

		private void HandleMiss()
		{
			gameState.DisableTarget();
			wordWhackEvents.RaiseNotFoundAnimation();
			scoringLogic.Miss();
			gameEvents.RaiseUpdateScoreboard();
			_enableTargetAfterDelayCoroutine = StartCoroutine(EnableTargetAfterDelay());
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? pos)
		{
			HandleMiss();
		}

		private void HandleNewGame()
		{
			wordWhackEvents.RaiseSolveAttemptCancellationRequest();
			wordWhackEvents.RaiseStopGameboardAnimationsRequest();
			StopRoutines();
			gameState.GameStatus = GameStatus.InProgress;
			gameState.EnableTarget();
			scoringLogic.Initialize();
			wordWhackEvents.RaisePhraseChanged(scoringLogic.CurrentPhrase);
			gameEvents.RaiseUpdateScoreboard();
		}

		private void HandleSolveAttemptRequest()
		{
			gameState.DisableTarget();
			string maskedPhrase = scoringLogic.GetMaskedPhrase();
			wordWhackEvents.RaiseSolveAttemptStart(maskedPhrase);
			_solving = true;
		}

		private void HandleSolveAttemptCancelled()
		{
			_solving = false;
			gameState.EnableTarget();
		}

		private void HandleSolveAttemptCompleted(string answer)
		{
			if (scoringLogic.SolveAttempt(answer))
			{
				wordWhackEvents.RaisePhraseSolved();
				HandleGameOver();
			}
			else
			{
				wordWhackEvents.RaiseSolveAttemptCancelled();
				wordWhackEvents.RaiseSolveAttemptFailed();
				scoringLogic.NextPlayer();
				gameEvents.RaiseUpdateScoreboard();
				gameState.EnableTarget();
			}
			_solving = false;
		}

		private void HandleUndo()
		{
			StopRoutines();
			if (!_solving)
			{
				scoringLogic.Undo();
				gameEvents.RaiseUpdateScoreboard();
			}
		}

		private void StopRoutines()
		{
			if (_enableTargetAfterDelayCoroutine != null)
			{
				StopCoroutine(_enableTargetAfterDelayCoroutine);
			}
			if (!_solving)
			{
				gameState.EnableTarget();
			}
		}
	}
}
