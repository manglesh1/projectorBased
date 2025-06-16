using System;
using System.Collections.Generic;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Games
{
	[CreateAssetMenu(menuName = "Games/Game Events")]
	public class GameEventsSO : ScriptableObject
	{
		public event UnityAction<GameObject> OnGameObjectClicked;

		public event UnityAction<bool> OnChangeGameboardVisibility;

		public event UnityAction<int?> OnScoreChange;

		public event UnityAction<ScoreToken, int?> OnScoreChangeWithObject;

		public event UnityAction<(string playerName, int frameIndex, int score)> OnConfirmScoreEdit;

		public event UnityAction<(string playerName, int frameIndex)> OnBeginScoreEdit;

		public event UnityAction<MoveToken> OnMoveToken;

		public event UnityAction<string, string, int?> OnRemovePlayerToken;

		public event UnityAction<string, string> OnRemoveScoreboardPlayerToken;

		public event UnityAction<string> OnResetPlayerScoreScoreboard;

		public event UnityAction<string> OnResetPlayerScore;

		public event UnityAction<string, string, string, int?> OnUpdateTwoPlayersScoresScoreboard;

		public event UnityAction<string, string, string, int?> OnUpdateTwoPlayersScores;

		public event UnityAction<GameObject> OnAddObjectToGameFlexSpace;

		public event UnityAction<GameObject> OnRemoveObjectFromGameFlexSpace;

		public event UnityAction OnRemoveAllFromGameFlexSpace;

		public event UnityAction OnUpdatePlayerTurn;

		public event UnityAction OnCancelScoreEdit;

		public event UnityAction OnCheckAndDestroySubMenu;

		public event UnityAction OnCustomAfterUndo;

		public event UnityAction OnGameLicensedListUpdated;

		public event UnityAction OnGameLoaded;

		public event UnityAction OnGameLoading;

		public event UnityAction OnGameOver;

		public event UnityAction OnNewGame;

		public event UnityAction OnNewRound;

		public event UnityAction OnMainMenu;

		public event UnityAction OnMiss;

		public event UnityAction<PointerEventData, Vector2?> OnMissDetected;

		public event UnityAction OnPlayWinAnimation;

		public event UnityAction<string> OnPlayWinAnimationForPlayer;

		public event UnityAction<List<string>> OnPlayWinAnimationForPlayers;

		public event UnityAction<string> OnResetTokenSize;

		public event UnityAction OnShowHelp;

		public event UnityAction OnTargetDisabled;

		public event UnityAction OnTargetEnabled;

		public event UnityAction OnThrowsPerTurnUpdated;

		public event UnityAction OnUndo;

		public event UnityAction OnUpdateScoreboard;

		public event UnityAction<string, Action> OnUpdateScoreboardWithCallback;

		public event UnityAction<string> OnUpdatePlayersScoreboard;

		public event UnityAction OnViewableGamesUpdated;

		public void RaiseBeginScoreEdit(string playerName, int frameIndex)
		{
			this.OnBeginScoreEdit?.Invoke((playerName, frameIndex));
		}

		public void RaiseCancelScoreEdit()
		{
			this.OnCancelScoreEdit?.Invoke();
		}

		public void RaiseChangeGameboardVisibility(bool isVisible)
		{
			this.OnChangeGameboardVisibility?.Invoke(isVisible);
		}

		public void RaiseCheckAndDestroySubMenu()
		{
			this.OnCheckAndDestroySubMenu?.Invoke();
		}

		public void RaiseConfirmScoreEdit(string playerName, int frameIndex, int score)
		{
			this.OnConfirmScoreEdit?.Invoke((playerName, frameIndex, score));
		}

		public void RaiseGameLicensedListUpdated()
		{
			this.OnGameLicensedListUpdated?.Invoke();
		}

		public void RaiseGameLoaded()
		{
			this.OnGameLoaded?.Invoke();
		}

		public void RaiseGameLoading()
		{
			this.OnGameLoading?.Invoke();
		}

		public void RaiseGameObjectClicked(GameObject gameObject)
		{
			this.OnGameObjectClicked?.Invoke(gameObject);
		}

		public void RaiseMoveToken(MoveToken moveToken)
		{
			this.OnMoveToken?.Invoke(moveToken);
		}

		public void RaiseRemovePlayerToken(string playerName, string tokenName, int? scoreValue)
		{
			this.OnRemovePlayerToken?.Invoke(playerName, tokenName, scoreValue);
		}

		public void RaiseRemoveScoreboardPlayerToken(string playerName, string tokenName)
		{
			this.OnRemoveScoreboardPlayerToken?.Invoke(playerName, tokenName);
		}

		public void RaiseResetPlayerScore(string playerName)
		{
			this.OnResetPlayerScoreScoreboard?.Invoke(playerName);
			this.OnResetPlayerScore?.Invoke(playerName);
		}

		public void RaiseUpdateTwoPlayersScores(string resetPlayerName, string updatePlayerName, string removeTokenName, int? removePlayerNewScore)
		{
			this.OnUpdateTwoPlayersScoresScoreboard?.Invoke(resetPlayerName, updatePlayerName, removeTokenName, removePlayerNewScore);
			this.OnUpdateTwoPlayersScores?.Invoke(resetPlayerName, updatePlayerName, removeTokenName, removePlayerNewScore);
		}

		public void RaiseScoreChange(int? score)
		{
			this.OnScoreChange?.Invoke(score);
		}

		public void RaiseScoreChange(ScoreToken scoreToken, int? scoreValue)
		{
			this.OnScoreChangeWithObject?.Invoke(scoreToken, scoreValue);
		}

		public void RaiseUpdatePlayerTurn()
		{
			this.OnUpdatePlayerTurn?.Invoke();
		}

		public void RaiseAddToGameFlexSpace(GameObject objectToAdd)
		{
			this.OnAddObjectToGameFlexSpace?.Invoke(objectToAdd);
		}

		public void RaiseRemoveObjectFromGameFlexSpace(GameObject objectToRemove)
		{
			this.OnRemoveObjectFromGameFlexSpace?.Invoke(objectToRemove);
		}

		public void RaiseRemoveAllFromGameFlexSpace()
		{
			this.OnRemoveAllFromGameFlexSpace?.Invoke();
		}

		public void RaiseGameOver()
		{
			this.OnGameOver?.Invoke();
		}

		public void RaiseNewGame()
		{
			this.OnNewGame?.Invoke();
		}

		public void RaiseNewRound()
		{
			this.OnNewRound?.Invoke();
		}

		public void RaiseMainMenu()
		{
			this.OnMainMenu?.Invoke();
		}

		public void RaiseMiss()
		{
			this.OnMiss?.Invoke();
		}

		public void RaiseMissDetected(PointerEventData pointerEventData = null, Vector2? screenPosition = null)
		{
			this.OnMissDetected?.Invoke(pointerEventData, screenPosition);
		}

		public void RaiseWinAnimation()
		{
			this.OnPlayWinAnimation?.Invoke();
		}

		public void RaiseWinAnimationForPlayer(string winnerName)
		{
			this.OnPlayWinAnimationForPlayer?.Invoke(winnerName);
		}

		public void RaiseWinAnimationForPlayers(List<string> playerNames)
		{
			this.OnPlayWinAnimationForPlayers?.Invoke(playerNames);
		}

		public void RaiseOnResetTokenSize(string tokenName)
		{
			this.OnResetTokenSize?.Invoke(tokenName);
		}

		public void RaiseShowHelp()
		{
			this.OnShowHelp?.Invoke();
		}

		public void RaiseThrowsPerTurnUpdated()
		{
			this.OnThrowsPerTurnUpdated?.Invoke();
		}

		public void RaiseTargetDisabled()
		{
			this.OnTargetDisabled?.Invoke();
		}

		public void RaiseTargetEnabled()
		{
			this.OnTargetEnabled?.Invoke();
		}

		public void RaiseCustomAfterUndo()
		{
			this.OnCustomAfterUndo?.Invoke();
		}

		public void RaiseUndo()
		{
			this.OnUndo?.Invoke();
		}

		public void RaiseUpdateScoreboard()
		{
			this.OnUpdateScoreboard?.Invoke();
		}

		public void RaiseUpdateScoreboardWithCallback(string playerName, Action callback)
		{
			this.OnUpdateScoreboardWithCallback?.Invoke(playerName, callback);
		}

		public void RaiseUpdatePlayersScoreboard(string playerName)
		{
			this.OnUpdatePlayersScoreboard?.Invoke(playerName);
		}

		public void RaiseViewableGamesUpdated()
		{
			this.OnViewableGamesUpdated?.Invoke();
		}
	}
}
