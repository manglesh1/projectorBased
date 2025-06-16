using System;
using System.Collections.Generic;
using Games.Battleship.ScoringLogic;
using UnityEngine;
using UnityEngine.Events;

namespace Games.Battleship.Scoreboard
{
	[CreateAssetMenu(menuName = "Scoreboards/Battleship/Battleship Scoreboard Events")]
	public class BattleshipScoreboardEventsSO : ScriptableObject
	{
		public event UnityAction OnAfterDisplayMessage;

		public event UnityAction OnCheckForWinCondition;

		public event UnityAction OnCustomAfterNewGame;

		public event UnityAction<Dictionary<string, BattleshipPlayerScoreboardStateHolder>> OnCustomAfterUndo;

		public event UnityAction<bool> OnIsGameboardVisible;

		public event UnityAction<int, bool, string, Action> OnPlayerSunkShip;

		public event UnityAction<string> OnRemoveEliminatedPlayers;

		public event UnityAction<string, int, bool> OnSetSunkenShipFlameImage;

		public event UnityAction<int?, string> OnScoreChange;

		public event UnityAction OnShowNextPlayerMessage;

		public event UnityAction<string, bool, bool> OnUpdateEliminatedPlayers;

		public void RaiseAfterDisplayMessage()
		{
			this.OnAfterDisplayMessage?.Invoke();
		}

		public void RaiseCheckForWinCondition()
		{
			this.OnCheckForWinCondition?.Invoke();
		}

		public void RaiseCustomAfterNewGame()
		{
			this.OnCustomAfterNewGame?.Invoke();
		}

		public void RaiseCustomAfterUndo(Dictionary<string, BattleshipPlayerScoreboardStateHolder> playerScorebardStateHolder)
		{
			this.OnCustomAfterUndo?.Invoke(playerScorebardStateHolder);
		}

		public void RaiseIsGameboardVisible(bool isVisible)
		{
			this.OnIsGameboardVisible?.Invoke(isVisible);
		}

		public void RaisePlayerSunkShip(int shipSunk, bool isAllShipsSunk, string attackedPlayerName, Action nextPlayerCallback)
		{
			this.OnPlayerSunkShip?.Invoke(shipSunk, isAllShipsSunk, attackedPlayerName, nextPlayerCallback);
		}

		public void RaiseRemoveEliminatedPlayers(string playerToDisable)
		{
			this.OnRemoveEliminatedPlayers?.Invoke(playerToDisable);
		}

		public void RaiseSetSunkenShipFlameImage(string playerName, int shipScore, bool showFlameImage)
		{
			this.OnSetSunkenShipFlameImage?.Invoke(playerName, shipScore, showFlameImage);
		}

		public void RaiseShowNextPlayerMessage()
		{
			this.OnShowNextPlayerMessage?.Invoke();
		}

		public void RaiseScoreChange(int? score, string attackedPlayerName)
		{
			this.OnScoreChange?.Invoke(score, attackedPlayerName);
		}

		public void RaiseUpdateEliminatedPlayers(string playerToDisable, bool removePlayerFromElimination = false, bool isUndo = false)
		{
			this.OnUpdateEliminatedPlayers?.Invoke(playerToDisable, removePlayerFromElimination, isUndo);
		}
	}
}
