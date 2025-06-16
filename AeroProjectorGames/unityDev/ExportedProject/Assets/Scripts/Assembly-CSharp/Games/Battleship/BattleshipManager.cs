using System;
using System.Collections;
using Games.Battleship.Scoreboard;
using Games.Battleship.ScoringLogic;
using Games.GameState;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using UnityEngine;

namespace Games.Battleship
{
	[RequireComponent(typeof(BattleshipObjectsManager))]
	public class BattleshipManager : MonoBehaviour
	{
		private BattleshipObjectsManager battleshipObjectManager;

		[SerializeField]
		private BattleshipGameboardManager battleshipGameboardManager;

		[SerializeField]
		private ShipSinkingAnimationManager shipSinkingAnimationManager;

		[Header("Scriptable Objects")]
		[SerializeField]
		private BattleshipScoreboardEventsSO battleshipScoreboardEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Scoring Strategy")]
		[SerializeField]
		private BattleshipScoringLogic scoringLogic;

		private void OnEnable()
		{
			battleshipObjectManager = base.gameObject.GetComponent<BattleshipObjectsManager>();
			gameState.NumberOfRounds = NumberOfRounds.InfiniteScored;
			StartCoroutine(GameFirstLoad());
			battleshipScoreboardEvents.OnCustomAfterNewGame += HandleNewGame;
			battleshipScoreboardEvents.OnPlayerSunkShip += HandlePlayerShipSunk;
			battleshipScoreboardEvents.OnScoreChange += HandleScoreChange;
			battleshipScoreboardEvents.OnShowNextPlayerMessage += HandleNextPlayerMessage;
		}

		protected void OnDisable()
		{
			battleshipScoreboardEvents.OnCustomAfterNewGame -= HandleNewGame;
			battleshipScoreboardEvents.OnPlayerSunkShip -= HandlePlayerShipSunk;
			battleshipScoreboardEvents.OnScoreChange -= HandleScoreChange;
			battleshipScoreboardEvents.OnShowNextPlayerMessage -= HandleNextPlayerMessage;
		}

		private IEnumerator GameFirstLoad()
		{
			yield return battleshipObjectManager.InitializeGame();
			InitializeGame();
			gameState.DisableTarget();
			yield return StartCoroutine(ShowMessageOfPlayersTurn(2.6f));
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Battleship);
		}

		private void HandleNewGame()
		{
			if (!gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				InitializeGame();
				StartCoroutine(NewGameEnumerator());
			}
		}

		private void HandleNextPlayerMessage()
		{
			StartCoroutine(ShowMessageOfPlayersTurn(2.05f));
		}

		private void HandlePlayerShipSunk(int shipScore, bool isAllShipsSunk, string attackedPlayerName, Action nextPlayerCallback)
		{
			StartCoroutine(PlayerShipSunk(shipScore, isAllShipsSunk, attackedPlayerName, nextPlayerCallback));
		}

		private void HandleScoreChange(int? score, string attackedPlayerName)
		{
			scoringLogic.RecordScore(new BattleshipScoreModel(attackedPlayerName, score.GetValueOrDefault()));
		}

		private void InitializeGame()
		{
			gameState.EnableTarget();
			gameState.GameStatus = GameStatus.InProgress;
			gameState.CurrentFrame = 0;
		}

		private IEnumerator NewGameEnumerator()
		{
			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(ShowMessageOfPlayersTurn(2.05f));
		}

		private IEnumerator PlayerShipSunk(int shipScore, bool isAllShipsSunk, string attackedPlayerName, Action nextPlayerCallback)
		{
			yield return StartCoroutine(shipSinkingAnimationManager.PlaySinkingAnimation(shipScore));
			if (isAllShipsSunk)
			{
				yield return battleshipGameboardManager.TransferPlayerHitTracker(attackedPlayerName);
				battleshipScoreboardEvents.RaiseCheckForWinCondition();
			}
			else
			{
				nextPlayerCallback?.Invoke();
			}
		}

		private IEnumerator ShowMessageOfPlayersTurn(float secondsToWait)
		{
			battleshipScoreboardEvents.RaiseIsGameboardVisible(isVisible: false);
			yield return scoringLogic.RemoveEliminatedPlayer();
			scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(battleshipScoreboardEvents.RaiseAfterDisplayMessage, gameState.CurrentPlayer + " is up, \nshooting at " + battleshipGameboardManager.PlayerToAttacked + "'s Navy", ScoreboardMessageStyle.Normal));
			yield return new WaitForSeconds(secondsToWait);
			battleshipScoreboardEvents.RaiseIsGameboardVisible(isVisible: true);
			gameState.EnableTarget();
		}
	}
}
