using Games.GameState;
using Players;
using UnityEngine;

namespace Games.SharedScoringLogic.UnscoredTwoPlayer
{
	public class TwoPlayerUnscoredScoringStrategy : ScoringStrategyBase<TwoPlayerUnscoredModel>
	{
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		private int _undoCount;

		public void OnEnable()
		{
			if (gameState.NumberOfRounds != NumberOfRounds.Infinite)
			{
				gameState.NumberOfRounds = NumberOfRounds.Infinite;
			}
			Reset();
			gameEvents.OnGameOver += gameState.DisableTarget;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnMainMenu += HandleMainMenu;
			gameEvents.OnUndo += Undo;
			Setup();
		}

		public void OnDisable()
		{
			gameEvents.OnGameOver -= gameState.DisableTarget;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnMainMenu -= HandleMainMenu;
			gameEvents.OnUndo -= Undo;
		}

		public override void RecordScore(TwoPlayerUnscoredModel score)
		{
			SaveGameState();
			if (gameState.GameStatus != GameStatus.Finished && !gameState.IsTargetDisabled)
			{
				ToggleActivePlayer();
				gameEvents.RaiseUpdateScoreboard();
			}
		}

		private void HandleMainMenu()
		{
			Reset();
		}

		private void HandleNewGame()
		{
			Reset();
			Setup();
		}

		private void Reset()
		{
			_undoCount = 0;
			gameState.RoundScores.Clear();
			gameState.CurrentPlayer = null;
			gameState.CurrentRound = 0;
			gameState.GameStatus = GameStatus.Setup;
			gameState.EnableTarget();
		}

		private void SaveGameState()
		{
			_undoCount++;
		}

		private void Setup()
		{
			if (playerState.players.Count > 1)
			{
				gameState.Player1Name = playerState.players[0].PlayerName;
				gameState.Player2Name = playerState.players[1].PlayerName;
			}
			else
			{
				gameState.Player1Name = playerState.AvailableTeams[0];
				gameState.Player2Name = playerState.AvailableTeams[1];
			}
			gameState.CurrentPlayer = gameState.Player1Name;
			gameEvents.RaiseUpdateScoreboard();
		}

		private void ToggleActivePlayer()
		{
			gameState.CurrentPlayer = ((gameState.CurrentPlayer == gameState.Player1Name) ? gameState.Player2Name : gameState.Player1Name);
		}

		private void Undo()
		{
			if (_undoCount > 0 && !gameState.IsTargetDisabled)
			{
				ToggleActivePlayer();
				gameEvents.RaiseUpdateScoreboard();
				_undoCount--;
			}
		}
	}
}
