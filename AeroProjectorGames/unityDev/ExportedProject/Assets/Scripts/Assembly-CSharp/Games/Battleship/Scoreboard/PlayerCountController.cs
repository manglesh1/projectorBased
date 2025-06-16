using System.Collections.Generic;
using Games.GameState;
using Players;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Battleship.Scoreboard
{
	public class PlayerCountController : MonoBehaviour
	{
		private int currentPlayerCount;

		[SerializeField]
		private BattleshipScoreboardEventsSO battleshipScoreboardEvents;

		[SerializeField]
		private LayoutElement emptyCellLayoutElement;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private List<GameObject> playerCellList;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private int scoreboardPlayerCount;

		private void OnEnable()
		{
			battleshipScoreboardEvents.OnRemoveEliminatedPlayers += HandleRemoveEliminatedPlayer;
			battleshipScoreboardEvents.OnUpdateEliminatedPlayers += HandleElimatedPlayer;
			gameEvents.OnNewGame += HandleNewGame;
			currentPlayerCount = scoreboardPlayerCount;
		}

		private void OnDisable()
		{
			battleshipScoreboardEvents.OnRemoveEliminatedPlayers -= HandleRemoveEliminatedPlayer;
			battleshipScoreboardEvents.OnUpdateEliminatedPlayers -= HandleElimatedPlayer;
			gameEvents.OnNewGame -= HandleNewGame;
		}

		private bool AddPlayer(int playerIndex)
		{
			if (playerCellList[playerIndex].activeSelf)
			{
				return false;
			}
			playerCellList[playerIndex].SetActive(value: true);
			return true;
		}

		private void HandleRemoveEliminatedPlayer(string playerName)
		{
			int playerIndex = GetPlayerIndex(playerName);
			if (playerIndex == -1)
			{
				Debug.Log("WARNING: Invalid player index");
			}
			else if (RemovePlayer(playerIndex))
			{
				currentPlayerCount--;
			}
		}

		private int GetPlayerIndex(string playerName)
		{
			for (int i = 0; i < playerState.players.Count; i++)
			{
				if (playerState.players[i].PlayerName == playerName)
				{
					return i;
				}
			}
			return -1;
		}

		private void HandleElimatedPlayer(string playerName, bool removePlayerFromElimination = false, bool isUndo = false)
		{
			if ((currentPlayerCount == scoreboardPlayerCount && removePlayerFromElimination) || !isUndo)
			{
				return;
			}
			int playerIndex = GetPlayerIndex(playerName);
			if (playerIndex == -1)
			{
				Debug.Log("WARNING: Invalid player index");
				return;
			}
			if (removePlayerFromElimination)
			{
				if (AddPlayer(playerIndex))
				{
					currentPlayerCount++;
				}
			}
			else if (RemovePlayer(playerIndex))
			{
				currentPlayerCount--;
			}
			SetEmptyCell();
		}

		private void HandleNewGame()
		{
			if (!gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				currentPlayerCount = scoreboardPlayerCount;
				ResetPlayers();
				SetEmptyCell();
			}
		}

		private bool RemovePlayer(int playerIndex)
		{
			if (playerCellList[playerIndex].activeSelf)
			{
				playerCellList[playerIndex].SetActive(value: false);
				return true;
			}
			return false;
		}

		private void ResetPlayers()
		{
			foreach (GameObject playerCell in playerCellList)
			{
				playerCell.SetActive(value: true);
			}
		}

		private void SetEmptyCell()
		{
			switch (currentPlayerCount)
			{
			case 1:
				emptyCellLayoutElement.gameObject.SetActive(value: true);
				emptyCellLayoutElement.flexibleHeight = 4f;
				break;
			case 2:
				emptyCellLayoutElement.gameObject.SetActive(value: true);
				emptyCellLayoutElement.flexibleHeight = 3f;
				break;
			case 3:
				emptyCellLayoutElement.gameObject.SetActive(value: true);
				emptyCellLayoutElement.flexibleHeight = 2f;
				break;
			case 4:
				emptyCellLayoutElement.gameObject.SetActive(value: true);
				emptyCellLayoutElement.flexibleHeight = 1f;
				break;
			case 5:
				emptyCellLayoutElement.gameObject.SetActive(value: false);
				break;
			case 6:
				emptyCellLayoutElement.gameObject.SetActive(value: false);
				break;
			}
		}
	}
}
