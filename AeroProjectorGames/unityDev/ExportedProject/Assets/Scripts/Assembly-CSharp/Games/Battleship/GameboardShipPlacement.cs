using System.Collections.Generic;
using Games.Battleship.Scoreboard;
using Games.Battleship.ScoringLogic;
using Games.GameState;
using Players;
using UnityEngine;

namespace Games.Battleship
{
	public class GameboardShipPlacement : MonoBehaviour
	{
		private List<List<int>> _twoHitShipPositions = new List<List<int>>
		{
			new List<int> { 13, 14 },
			new List<int> { 9, 10 },
			new List<int> { 23, 24 },
			new List<int> { 10, 15 },
			new List<int> { 24, 25 },
			new List<int> { 2, 7 },
			new List<int> { 1, 2 },
			new List<int> { 1, 6 },
			new List<int> { 20, 25 },
			new List<int> { 5, 10 },
			new List<int> { 15, 20 },
			new List<int> { 16, 17 },
			new List<int> { 21, 22 },
			new List<int> { 23, 24 },
			new List<int> { 14, 15 },
			new List<int> { 11, 12 },
			new List<int> { 17, 22 },
			new List<int> { 2, 7 },
			new List<int> { 4, 9 },
			new List<int> { 20, 25 },
			new List<int> { 10, 15 }
		};

		private List<List<int>> _threeHitShipPositions = new List<List<int>>
		{
			new List<int> { 21, 22, 23 },
			new List<int> { 18, 19, 20 },
			new List<int> { 7, 12, 17 },
			new List<int> { 8, 13, 18 },
			new List<int> { 4, 9, 14 },
			new List<int> { 13, 14, 15 },
			new List<int> { 21, 22, 23 },
			new List<int> { 3, 4, 5 },
			new List<int> { 11, 16, 21 },
			new List<int> { 2, 7, 12 },
			new List<int> { 3, 4, 5 },
			new List<int> { 5, 10, 15 },
			new List<int> { 7, 8, 9 },
			new List<int> { 7, 8, 9 },
			new List<int> { 21, 22, 23 },
			new List<int> { 14, 19, 24 },
			new List<int> { 15, 20, 25 },
			new List<int> { 22, 23, 24 },
			new List<int> { 1, 6, 11 },
			new List<int> { 16, 17, 18 },
			new List<int> { 22, 23, 24 }
		};

		private List<List<int>> _fourHitShipPositions = new List<List<int>>
		{
			new List<int> { 2, 3, 4, 5 },
			new List<int> { 7, 12, 17, 22 },
			new List<int> { 5, 10, 15, 20 },
			new List<int> { 6, 11, 16, 21 },
			new List<int> { 2, 7, 12, 17 },
			new List<int> { 22, 23, 24, 25 },
			new List<int> { 5, 10, 15, 20 },
			new List<int> { 16, 17, 18, 19 },
			new List<int> { 7, 8, 9, 10 },
			new List<int> { 21, 22, 23, 24 },
			new List<int> { 2, 7, 12, 17 },
			new List<int> { 8, 13, 18, 23 },
			new List<int> { 17, 18, 19, 20 },
			new List<int> { 11, 12, 13, 14 },
			new List<int> { 1, 2, 3, 4 },
			new List<int> { 1, 2, 3, 4 },
			new List<int> { 6, 7, 8, 9 },
			new List<int> { 11, 12, 13, 14 },
			new List<int> { 17, 18, 19, 20 },
			new List<int> { 7, 8, 9, 10 },
			new List<int> { 3, 8, 13, 18 }
		};

		private string _attackedPlayer;

		[Header("Game Elements")]
		private Dictionary<string, ShipPlacementEnum> _playersShipPlacement = new Dictionary<string, ShipPlacementEnum>();

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		private void OnEnable()
		{
			SetPlayerShipPlacement();
			gameEvents.OnNewGame += HandleNewGame;
		}

		private void OnDisable()
		{
			gameEvents.OnNewGame -= HandleNewGame;
		}

		public BattleshipScoreModel CheckForHit(int cellPosition, int attackedPlayerIndex)
		{
			_attackedPlayer = string.Empty;
			_attackedPlayer = playerState.players[attackedPlayerIndex].PlayerName;
			ShipPlacementEnum shipPlacementEnum = _playersShipPlacement[_attackedPlayer];
			if (shipPlacementEnum.TwoHitShip.Contains(cellPosition))
			{
				return new BattleshipScoreModel(_attackedPlayer, 2);
			}
			if (shipPlacementEnum.ThreeHitShip.Contains(cellPosition))
			{
				return new BattleshipScoreModel(_attackedPlayer, 3);
			}
			if (shipPlacementEnum.FourHitShip.Contains(cellPosition))
			{
				return new BattleshipScoreModel(_attackedPlayer, 4);
			}
			return new BattleshipScoreModel(_attackedPlayer);
		}

		private int GetLowestPlacementCount()
		{
			int count = _twoHitShipPositions.Count;
			if (_threeHitShipPositions.Count < count)
			{
				count = _threeHitShipPositions.Count;
			}
			if (_fourHitShipPositions.Count < count)
			{
				count = _fourHitShipPositions.Count;
			}
			return count;
		}

		public int GetPlayerToAttackIndex(int currentPlayerIndex)
		{
			if (currentPlayerIndex >= playerState.players.Count)
			{
				return -1;
			}
			if (currentPlayerIndex + 1 < playerState.players.Count)
			{
				return currentPlayerIndex + 1;
			}
			return 0;
		}

		private List<int> GetShipPlacementFromIndex(List<List<int>> shipPlacementList, int listIndex)
		{
			return shipPlacementList[listIndex];
		}

		private void HandleNewGame()
		{
			if (!gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				SetPlayerShipPlacement();
			}
		}

		private void SetPlayerShipPlacement()
		{
			_playersShipPlacement.Clear();
			_playersShipPlacement = new Dictionary<string, ShipPlacementEnum>();
			List<int> list = new List<int>();
			int lowestPlacementCount = GetLowestPlacementCount();
			for (int i = 0; i < playerState.players.Count; i++)
			{
				ShipPlacementEnum shipPlacementEnum = new ShipPlacementEnum();
				int num = 0;
				int num2;
				do
				{
					num2 = Random.Range(0, lowestPlacementCount);
					num++;
				}
				while (list.Contains(num2) || num < lowestPlacementCount);
				list.Add(num2);
				shipPlacementEnum.TwoHitShip = GetShipPlacementFromIndex(_twoHitShipPositions, num2);
				shipPlacementEnum.ThreeHitShip = GetShipPlacementFromIndex(_threeHitShipPositions, num2);
				shipPlacementEnum.FourHitShip = GetShipPlacementFromIndex(_fourHitShipPositions, num2);
				_playersShipPlacement.Add(playerState.players[i].PlayerName, shipPlacementEnum);
			}
		}
	}
}
