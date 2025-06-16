using System.Collections.Generic;
using Extensions;
using Games.Battleship.Scoreboard;
using Games.Battleship.ScoringLogic;
using Games.GameState;
using Players;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Battleship
{
	public class BattleshipGameboardManager : MonoBehaviour
	{
		private const int GAMEBOARD_CELL_COUNT = 25;

		private Stack<Dictionary<string, PlayerGameboardTracker>> _playersHitTrackerHistory = new Stack<Dictionary<string, PlayerGameboardTracker>>();

		private Dictionary<string, PlayerGameboardTracker> _playersHitTracker = new Dictionary<string, PlayerGameboardTracker>();

		[Header("Multi-Display Scoring Panel")]
		[SerializeField]
		private GameObject multiDisplayScoringPanel;

		[Header("Game Managers")]
		[SerializeField]
		private GameboardShipPlacement gameboardShipPlacement;

		[Header("Gameboard Cell Elements")]
		[SerializeField]
		private List<CellManager> cellManagers = new List<CellManager>();

		[SerializeField]
		private List<MultiDisplayCellManager> multiDisplayCellManager = new List<MultiDisplayCellManager>();

		[Header("Scriptable Objects")]
		[SerializeField]
		private BattleshipScoreboardEventsSO battleshipScoreboardEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		public string PlayerToAttacked
		{
			get
			{
				PlayerGameboardTracker playerGameboardTracker = _playersHitTracker[gameState.CurrentPlayer];
				return playerState.players[playerGameboardTracker.PlayerToAttackIndex].PlayerName;
			}
		}

		private void OnEnable()
		{
			InitializeGameboard();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayScoringPanel);
			}
			else
			{
				multiDisplayScoringPanel.SetActive(value: false);
			}
			gameEvents.OnCustomAfterUndo += HandleUndo;
			gameEvents.OnMiss += SaveGameboardState;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnUpdatePlayerTurn += HandleUpdatePlayerTurn;
		}

		private void OnDisable()
		{
			gameEvents.OnCustomAfterUndo -= HandleUndo;
			gameEvents.OnMiss -= SaveGameboardState;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnUpdatePlayerTurn -= HandleUpdatePlayerTurn;
		}

		public BattleshipScoreModel GetScoreWithoutRecording(int cellPosition)
		{
			PlayerGameboardTracker playerGameboardTracker = _playersHitTracker[gameState.CurrentPlayer];
			if (playerGameboardTracker.CellsClicked.Contains(cellPosition))
			{
				return new BattleshipScoreModel(string.Empty, -1);
			}
			return gameboardShipPlacement.CheckForHit(cellPosition, playerGameboardTracker.PlayerToAttackIndex);
		}

		public BattleshipScoreModel HandleActiveCell(int cellPosition)
		{
			PlayerGameboardTracker playerGameboardTracker = _playersHitTracker[gameState.CurrentPlayer];
			if (playerGameboardTracker.CellsClicked.Contains(cellPosition))
			{
				return new BattleshipScoreModel(string.Empty, -1);
			}
			SaveGameboardState();
			BattleshipScoreModel battleshipScoreModel = gameboardShipPlacement.CheckForHit(cellPosition, playerGameboardTracker.PlayerToAttackIndex);
			playerGameboardTracker.CellsClicked.Add(cellPosition);
			if (battleshipScoreModel.Score == 0)
			{
				playerGameboardTracker.CurrentPlayerCellStates[cellPosition] = GameboardCellStates.Miss;
				return battleshipScoreModel;
			}
			playerGameboardTracker.CurrentPlayerCellStates[cellPosition] = GameboardCellStates.Hit;
			return battleshipScoreModel;
		}

		public void HandleScoreChange(int score, string attackedPlayerName)
		{
			battleshipScoreboardEvents.RaiseScoreChange(score, attackedPlayerName);
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			SaveGameboardState();
		}

		private void HandleNewGame()
		{
			if (!gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				InitializeGameboard();
				battleshipScoreboardEvents.RaiseCustomAfterNewGame();
			}
		}

		private void HandleUndo()
		{
			if (_playersHitTrackerHistory.Count != 0)
			{
				Dictionary<string, PlayerGameboardTracker> playersHitTracker = _playersHitTrackerHistory.Pop();
				_playersHitTracker = playersHitTracker;
				gameEvents.RaiseUpdatePlayerTurn();
			}
		}

		private void HandleUpdatePlayerTurn()
		{
			PlayerGameboardTracker playerGameboardTracker = _playersHitTracker[gameState.CurrentPlayer];
			int count = playerGameboardTracker.CurrentPlayerCellStates.Count;
			for (int i = 1; i < count; i++)
			{
				switch (playerGameboardTracker.CurrentPlayerCellStates[i])
				{
				case GameboardCellStates.Hit:
					cellManagers[i].SetCellAsHit();
					if (SettingsStore.Interaction.MultiDisplayEnabled)
					{
						multiDisplayCellManager[i].SetCellAsHit();
					}
					break;
				case GameboardCellStates.Miss:
					cellManagers[i].SetCellAsMiss();
					if (SettingsStore.Interaction.MultiDisplayEnabled)
					{
						multiDisplayCellManager[i].SetCellAsHit();
					}
					break;
				default:
					cellManagers[i].SetCellAsEmpty();
					break;
				}
				SetMultiDisplayCellManager(i, playerGameboardTracker.CurrentPlayerCellStates[i]);
			}
		}

		private void IsCellAlreadyHit(int cellPosition)
		{
		}

		private void SetMultiDisplayCellManager(int index, GameboardCellStates state)
		{
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayCellManager[index].SetCellState(state);
			}
		}

		private void InitializeGameboard()
		{
			_playersHitTrackerHistory.Clear();
			ResetPlayerShipPositions();
			HandleUpdatePlayerTurn();
		}

		private void ResetPlayerShipPositions()
		{
			_playersHitTracker.Clear();
			for (int i = 0; i < playerState.players.Count; i++)
			{
				int playerToAttackIndex = gameboardShipPlacement.GetPlayerToAttackIndex(i);
				if (playerToAttackIndex != -1)
				{
					_playersHitTracker.Add(playerState.players[i].PlayerName, new PlayerGameboardTracker(25, playerToAttackIndex));
				}
				else
				{
					Debug.Log("WARNING: Player index (" + i + ") is an invalid player index");
				}
			}
		}

		private void SaveGameboardState()
		{
			_playersHitTrackerHistory.Push(new Dictionary<string, PlayerGameboardTracker>(_playersHitTracker.SimpleJsonClone()));
		}

		public bool TransferPlayerHitTracker(string attackedPlayerName)
		{
			_playersHitTracker[gameState.CurrentPlayer] = _playersHitTracker[attackedPlayerName];
			return true;
		}
	}
}
