using System;
using System.Collections;
using System.Linq;
using Games.Cricket.Logic.Gameboard;
using Games.Cricket.Logic.Gameboard._18_Segment_Gameboard;
using Games.Cricket.Logic.Scoring;
using Games.Cricket.Scoreboard;
using Games.GameState;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using Scoreboard.PrefabScripts;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Cricket
{
	public class CricketGame : MonoBehaviour
	{
		private bool _missRoutineRunning;

		private ScoreboardController _scoreboardController;

		[Header("Multi Display")]
		[SerializeField]
		private GameObject multiDisplayPanel;

		[SerializeField]
		private GameObject multiDisplayGamePanel;

		[SerializeField]
		private GameObject multiDisplaySetupPanel;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[SerializeField]
		private EighteenSegmentGameboardController multiDisplayGameboardController;

		[Header("Scoreboard Loader")]
		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Hit Effect Events")]
		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[Header("Scoring Logic")]
		[SerializeField]
		private CricketScoringLogic scoringLogic;

		[Header("Context")]
		[SerializeField]
		private CricketContextSO context;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private CricketScoreboardEventsSO scoreboardEvents;

		[Header("Game Panels")]
		[SerializeField]
		private GameObject gameSetupPanel;

		[SerializeField]
		private GameObject gameboardPanel;

		[Header("Gameboard")]
		[SerializeField]
		private EighteenSegmentGameboardController gameboardController;

		private void OnEnable()
		{
			gameState.NumberOfRounds = NumberOfRounds.Infinite;
			gameState.EnableTarget();
			context.Setup();
			ChangeState(CricketGameState.Setup);
			gameboardController.OnScore += HandleScore;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnUndo += HandleUndo;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayGameboardController.OnScore += HandleScore;
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayPanel);
			}
		}

		protected void OnDisable()
		{
			context.Clean();
			gameboardController.OnScore -= HandleScore;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnUndo -= HandleUndo;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayGameboardController.OnScore -= HandleScore;
			}
		}

		private void Start()
		{
			if (context.IsSinglePlayer)
			{
				InitializeGame(CricketGameState.Unscored);
			}
		}

		public void SelectScoredGame()
		{
			InitializeGame(CricketGameState.Scored);
		}

		public void SelectUnscoredGame()
		{
			InitializeGame(CricketGameState.Unscored);
		}

		private void ChangeState(CricketGameState newState)
		{
			context.SetGameState(newState);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplaySetupPanel.SetActive(value: false);
				multiDisplayGamePanel.SetActive(value: false);
			}
			switch (context.CurrentGameState)
			{
			case CricketGameState.Setup:
				gameboardPanel.SetActive(value: false);
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplaySetupPanel.SetActive(value: true);
				}
				gameSetupPanel.SetActive(value: true);
				break;
			case CricketGameState.Loading:
				gameboardPanel.SetActive(value: false);
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplaySetupPanel.SetActive(value: false);
					multiDisplayGamePanel.SetActive(value: false);
				}
				gameSetupPanel.SetActive(value: false);
				break;
			case CricketGameState.Scored:
			case CricketGameState.Unscored:
				gameSetupPanel.SetActive(value: false);
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayGamePanel.SetActive(value: true);
				}
				gameboardPanel.SetActive(value: true);
				break;
			}
		}

		private void HandleMiss()
		{
			if (!gameState.IsTargetDisabled)
			{
				scoringLogic.Miss();
				if (context.CurrentThrow == 1 && !context.IsSinglePlayer)
				{
					scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, context.CurrentPlayer.PlayerName + " you're up!", ScoreboardMessageStyle.Normal));
				}
				scoreboardEvents.RaiseUpdateScoreboard();
			}
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			StartCoroutine(MissDetectedRoutine(pointerEventData, screenPoint));
		}

		private IEnumerator MissDetectedRoutine(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			_missRoutineRunning = true;
			gameState.DisableTarget();
			if (screenPoint.HasValue)
			{
				hitEffectEvents.RaiseHitEffect(screenPoint.Value);
			}
			scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, "Miss!", ScoreboardMessageStyle.Normal));
			yield return new WaitForSeconds(3f);
			gameState.EnableTarget();
			HandleMiss();
			_missRoutineRunning = false;
		}

		private void HandleNewGame()
		{
			if (!gameState.IsTargetDisabled)
			{
				ChangeState(CricketGameState.Setup);
				scoreboardLoader.RaiseUnloadScoreboardRequest();
				context.Setup();
			}
		}

		private void HandleScore(CricketGameboardHitModel hitModel)
		{
			if (gameState.GameStatus != GameStatus.Finished && !gameState.IsTargetDisabled)
			{
				switch (context.CurrentGameState)
				{
				case CricketGameState.Scored:
					HandleScoredGameScoring(hitModel);
					break;
				case CricketGameState.Unscored:
					HandleUnscoredGameScoring(hitModel);
					break;
				}
			}
		}

		private void HandleScoredGameScoring(CricketGameboardHitModel hitModel)
		{
			bool scoreRegistered = scoringLogic.AddScore(hitModel.BucketKey, hitModel.GetScoringModifier());
			hitModel.ScoreRegistered = scoreRegistered;
			StartCoroutine(ProcessScoredGameHit(hitModel));
		}

		private IEnumerator ProcessScoredGameHit(CricketGameboardHitModel hitModel)
		{
			gameState.DisableTarget();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				StartCoroutine(multiDisplayGameboardController.AnimateHit(hitModel));
			}
			yield return gameboardController.AnimateHit(hitModel);
			gameState.EnableTarget();
			ShowNextPlayerTurn();
			scoreboardEvents.RaiseUpdateScoreboard();
			if (scoringLogic.IsGameOver())
			{
				gameEvents.RaiseWinAnimationForPlayers((from p in scoringLogic.GetCurrentScoredWinners()
					select p.PlayerName).ToList());
				gameEvents.RaiseGameOver();
				gameState.GameStatus = GameStatus.Finished;
			}
		}

		private void HandleUndo()
		{
			if (!gameState.IsTargetDisabled)
			{
				context.Undo();
				scoreboardEvents.RaiseUpdateScoreboard();
			}
		}

		private void HandleUnscoredGameScoring(CricketGameboardHitModel hitModel)
		{
			bool scoreRegistered = scoringLogic.AddScore(hitModel.BucketKey, hitModel.GetScoringModifier());
			hitModel.ScoreRegistered = scoreRegistered;
			StartCoroutine(ProcessUnscoredGameHit(hitModel));
		}

		private IEnumerator ProcessUnscoredGameHit(CricketGameboardHitModel hitModel)
		{
			gameState.DisableTarget();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				StartCoroutine(multiDisplayGameboardController.AnimateHit(hitModel));
			}
			yield return gameboardController.AnimateHit(hitModel);
			gameState.EnableTarget();
			PlayerData firstUnscoredWinner = scoringLogic.GetFirstUnscoredWinner();
			if (firstUnscoredWinner != null)
			{
				gameEvents.RaiseWinAnimationForPlayer(firstUnscoredWinner.PlayerName);
				gameEvents.RaiseGameOver();
				gameState.GameStatus = GameStatus.Finished;
			}
			else
			{
				ShowNextPlayerTurn();
			}
			scoreboardEvents.RaiseUpdateScoreboard();
		}

		private void InitializeGame(CricketGameState newState)
		{
			gameState.GameStatus = GameStatus.Setup;
			if (newState == CricketGameState.Setup)
			{
				throw new ArgumentException("GameState in setup, no scoreboard will load for this state");
			}
			ChangeState(CricketGameState.Loading);
			Action messageCompleteAction = delegate
			{
				ChangeState(newState);
				scoreboardLoader.RaiseLoadScoreboardRequest((context.CurrentGameState == CricketGameState.Scored) ? ScoreboardType.CricketScored : ScoreboardType.CricketUnscored);
				scoreboardEvents.RaiseUpdateScoreboard();
			};
			scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(messageCompleteAction, "Every player will have \n 3 throws per turn!", ScoreboardMessageStyle.Normal));
		}

		private void ShowNextPlayerTurn()
		{
			if (context.CurrentThrow == 1 && !context.IsSinglePlayer)
			{
				scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, context.CurrentPlayer.PlayerName + " you're up!", ScoreboardMessageStyle.Normal));
			}
		}
	}
}
