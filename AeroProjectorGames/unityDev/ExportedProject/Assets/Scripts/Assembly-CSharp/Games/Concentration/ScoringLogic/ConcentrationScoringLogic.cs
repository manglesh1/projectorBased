using System.Collections.Generic;
using Extensions;
using Games.Concentration.SO;
using Games.Concentration.Scoreboard;
using Games.GameState;
using Players;
using UnityEngine;

namespace Games.Concentration.ScoringLogic
{
	public class ConcentrationScoringLogic : MonoBehaviour
	{
		private Dictionary<string, List<int?>>.Enumerator _currentPlayer;

		private Stack<GameStateHistory> _scoreboardStateHistory = new Stack<GameStateHistory>();

		[SerializeField]
		private ConcentrationGameEventsSO concentrationGameEvents;

		[SerializeField]
		private ConcentrationScoreboardEventsSO concentrationScoreboardEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		protected void OnEnable()
		{
			if (gameState.NumberOfRounds != NumberOfRounds.InfiniteScored)
			{
				gameState.NumberOfRounds = NumberOfRounds.InfiniteScored;
			}
			HandleNewGame();
			AtEnableAddListeners();
		}

		protected void OnDisable()
		{
			Reset();
			AtDisableRemoveListeners();
		}

		private void AtEnableAddListeners()
		{
			concentrationGameEvents.OnEndPlayerTurn += AfterScoring;
			concentrationScoreboardEvents.OnRecordMissedScore += HandleMiss;
			concentrationScoreboardEvents.OnRecordStandardScore += RecordStandardScore;
			concentrationScoreboardEvents.OnRecordScoreWithStolenToken += RecordScoreWithStolenCard;
			concentrationScoreboardEvents.OnRecordWildScore += RecordStandardScore;
			concentrationScoreboardEvents.OnRecordRemoveStolenTokenScore += HandleStealTokenFromPlayer;
			gameEvents.OnGameOver += HandleGameOver;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnUndo += Undo;
		}

		private void AtDisableRemoveListeners()
		{
			concentrationGameEvents.OnEndPlayerTurn -= AfterScoring;
			concentrationScoreboardEvents.OnRecordMissedScore -= HandleMiss;
			concentrationScoreboardEvents.OnRecordStandardScore -= RecordStandardScore;
			concentrationScoreboardEvents.OnRecordScoreWithStolenToken -= RecordScoreWithStolenCard;
			concentrationScoreboardEvents.OnRecordWildScore -= RecordStandardScore;
			concentrationScoreboardEvents.OnRecordRemoveStolenTokenScore -= HandleStealTokenFromPlayer;
			gameEvents.OnGameOver -= HandleGameOver;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnUndo -= Undo;
		}

		private void AfterScoring()
		{
			PlayerTurnOver();
			gameEvents.RaiseUpdatePlayerTurn();
		}

		private void Initialize()
		{
			foreach (PlayerData player in playerState.players)
			{
				int capacity = 0;
				gameState.RoundScores.Add(player.PlayerName, new List<int?>(capacity));
				gameState.InfiniteScoredGameScores.Add(player.PlayerName, new List<ScoreToken>());
				gameState.TotalScores.Add(player.PlayerName, 0);
			}
			_currentPlayer = gameState.RoundScores.GetEnumerator();
			_currentPlayer.MoveNext();
			GameStateSO gameStateSO = gameState;
			gameStateSO.CurrentPlayer = _currentPlayer.Current.Key;
		}

		private void HandleGameOver()
		{
			gameState.DisableTarget();
			gameState.GameStatus = GameStatus.Finished;
		}

		private void HandleMiss()
		{
			RecordStandardScore(null, 0);
		}

		private void HandleNewGame()
		{
			Reset();
			Initialize();
		}

		private void HandleStealTokenFromPlayer(string playerToStealFrom, int tokenScoreValue)
		{
			SaveScoreboardState();
			gameState.TotalScores[playerToStealFrom] -= tokenScoreValue;
			concentrationScoreboardEvents.RaiseUpdateScoreboardRemovingStolenTokenFromPlayer(playerToStealFrom, tokenScoreValue);
		}

		private void HandleUpdatingScoreboardWithStandardScore(GameObject gameToken, int tokenScoreValue)
		{
			gameState.TotalScores[_currentPlayer.Current.Key] += tokenScoreValue;
			concentrationScoreboardEvents.RaiseUpdateScoreboardWithStandardScore(gameState.CurrentPlayer, gameToken);
		}

		private void HandleUpdatingScoreboardWithStealScore(GameObject stolenGameToken, int tokenScoreValue)
		{
			gameState.TotalScores[_currentPlayer.Current.Key] += tokenScoreValue;
			concentrationScoreboardEvents.RaiseUpdateScoreboardWithStealScore(gameState.CurrentPlayer, stolenGameToken);
		}

		private void PlayerTurnOver()
		{
			if (_currentPlayer.MoveNext())
			{
				GameStateSO gameStateSO = gameState;
				gameStateSO.CurrentPlayer = _currentPlayer.Current.Key;
				gameState.CurrentFrame = gameState.CurrentRoundIndex;
				return;
			}
			_currentPlayer = gameState.RoundScores.GetEnumerator();
			_currentPlayer.MoveNext();
			GameStateSO gameStateSO2 = gameState;
			gameStateSO2.CurrentPlayer = _currentPlayer.Current.Key;
			gameState.CurrentRound += gameState.ThrowsPerTurn;
			gameState.CurrentFrame = gameState.CurrentRoundIndex;
			gameEvents.RaiseNewRound();
		}

		private void RecordScoreWithStolenCard(GameObject stolenGameToken, int tokenScoreValue)
		{
			HandleUpdatingScoreboardWithStealScore(stolenGameToken, tokenScoreValue);
		}

		private void RecordStandardScore(GameObject gameToken, int tokenScoreValue)
		{
			SaveScoreboardState();
			ScoreToken scoreToken = new ScoreToken();
			scoreToken.ScoreValue = tokenScoreValue;
			scoreToken.TokenName = ((gameToken == null) ? string.Empty : gameToken.name);
			scoreToken.Token = gameToken;
			Dictionary<string, List<ScoreToken>> infiniteScoredGameScores = gameState.InfiniteScoredGameScores;
			infiniteScoredGameScores[_currentPlayer.Current.Key].Add(scoreToken);
			HandleUpdatingScoreboardWithStandardScore(gameToken, tokenScoreValue);
		}

		protected void Reset()
		{
			_scoreboardStateHistory.Clear();
			gameState.RoundScores.Clear();
			gameState.InfiniteScoredGameScores.Clear();
			gameState.TotalScores.Clear();
			gameState.GameStatus = GameStatus.Setup;
			gameState.CurrentPlayer = null;
			gameState.CurrentFrame = 0;
			gameState.CurrentRound = 1;
		}

		private void SaveScoreboardState()
		{
			_scoreboardStateHistory.Push(new GameStateHistory
			{
				CurrentFrame = gameState.CurrentFrame,
				CurrentPlayer = gameState.CurrentPlayer,
				CurrentRound = gameState.CurrentRound,
				RoundScores = gameState.RoundScores.SimpleJsonClone(),
				InfiniteScoredGameScores = gameState.InfiniteScoredGameScores.SimpleJsonClone(),
				TotalScores = gameState.TotalScores.SimpleJsonClone()
			});
		}

		private void Undo()
		{
			if (_scoreboardStateHistory.Count == 0 || gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				return;
			}
			gameState.DisableTarget();
			GameStateHistory gameStateHistory = _scoreboardStateHistory.Pop();
			gameState.CurrentFrame = gameStateHistory.CurrentFrame;
			gameState.CurrentPlayer = gameStateHistory.CurrentPlayer;
			gameState.CurrentRound = gameStateHistory.CurrentRound;
			gameState.RoundScores = gameStateHistory.RoundScores;
			gameState.InfiniteScoredGameScores = gameStateHistory.InfiniteScoredGameScores;
			gameState.TotalScores = gameStateHistory.TotalScores;
			_currentPlayer = gameState.RoundScores.GetEnumerator();
			_currentPlayer.MoveNext();
			foreach (string key in gameState.RoundScores.Keys)
			{
				if (_currentPlayer.Current.Key == gameState.CurrentPlayer)
				{
					break;
				}
				_currentPlayer.MoveNext();
			}
			gameEvents.RaiseCustomAfterUndo();
			gameState.EnableTarget();
		}
	}
}
