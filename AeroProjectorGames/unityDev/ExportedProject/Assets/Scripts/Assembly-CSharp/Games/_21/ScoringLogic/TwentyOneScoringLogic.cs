using System.Collections;
using System.Collections.Generic;
using Extensions;
using Games.GameState;
using Games.SharedScoringLogic;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Games._21.ScoringLogic
{
	public class TwentyOneScoringLogic : ScoringStrategyBase<TwentyOneScoreModel>
	{
		private const int THROWS_PER_TURN = 1;

		private Dictionary<string, List<int?>>.Enumerator _currentPlayer;

		private Stack<GameStateHistory> _gameStateHistory = new Stack<GameStateHistory>();

		private bool _missRoutineRunning;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardEvents;

		public event UnityAction OnInitBegin;

		public event UnityAction OnInitComplete;

		public event UnityAction OnDisableBegin;

		public event UnityAction OnDisableComplete;

		public event UnityAction OnSaveGameStateBegin;

		public event UnityAction OnSaveGameStateComplete;

		public event UnityAction OnUndoBegin;

		public event UnityAction OnUndoComplete;

		protected void OnEnable()
		{
			RaiseInitBegin();
			if (gameState.NumberOfRounds != NumberOfRounds.InfiniteScored)
			{
				gameState.NumberOfRounds = NumberOfRounds.InfiniteScored;
			}
			gameEvents.OnGameOver += HandleGameOver;
			gameEvents.OnMainMenu += HandleMainMenu;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnMoveToken += HandleMoveToken;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnRemovePlayerToken += HandleRemovePlayerToken;
			gameEvents.OnResetPlayerScoreScoreboard += HandleResetPlayerScore;
			gameEvents.OnUndo += Undo;
			gameEvents.OnUpdateTwoPlayersScoresScoreboard += HandleUpdateTwoPlayersScores;
			Reset();
			Initialize();
			RaiseInitComplete();
		}

		protected void OnDisable()
		{
			RaiseDisableBegin();
			gameEvents.OnGameOver -= HandleGameOver;
			gameEvents.OnMainMenu -= HandleMainMenu;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnMoveToken -= HandleMoveToken;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnRemovePlayerToken -= HandleRemovePlayerToken;
			gameEvents.OnResetPlayerScoreScoreboard -= HandleResetPlayerScore;
			gameEvents.OnUndo -= Undo;
			gameEvents.OnUpdateTwoPlayersScoresScoreboard -= HandleUpdateTwoPlayersScores;
			Reset();
			RaiseDisableComplete();
		}

		private void RaiseInitBegin()
		{
			this.OnInitBegin?.Invoke();
		}

		private void RaiseInitComplete()
		{
			this.OnInitComplete?.Invoke();
		}

		private void RaiseDisableBegin()
		{
			this.OnDisableBegin?.Invoke();
		}

		private void RaiseDisableComplete()
		{
			this.OnDisableComplete?.Invoke();
		}

		private void RaiseSaveGameStateBegin()
		{
			this.OnSaveGameStateBegin?.Invoke();
		}

		private void RaiseSaveGameStateComplete()
		{
			this.OnSaveGameStateComplete?.Invoke();
		}

		private void RaiseUndoBegin()
		{
			this.OnUndoBegin?.Invoke();
		}

		private void RaiseUndoComplete()
		{
			this.OnUndoComplete?.Invoke();
		}

		private void HandleAfterScoring()
		{
			gameState.ThrowsRemaining--;
			gameState.EnableTarget();
			if (gameState.ThrowsRemaining == 0)
			{
				PlayerTurnOver();
			}
			else
			{
				gameState.CurrentFrame++;
			}
			gameEvents.RaiseUpdatePlayerTurn();
		}

		private void HandleGameOver()
		{
			gameState.DisableTarget();
			gameState.GameStatus = GameStatus.Finished;
		}

		private void HandleMainMenu()
		{
			Reset();
		}

		private void HandleMiss()
		{
			if (gameState.GameStatus != GameStatus.Finished && !gameState.IsTargetDisabled)
			{
				SavegameState();
				HandleAfterScoring();
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
			scoreboardEvents.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, "Miss!", ScoreboardMessageStyle.Normal));
			yield return new WaitForSeconds(3f);
			gameState.EnableTarget();
			HandleMiss();
			_missRoutineRunning = false;
		}

		private void HandleMoveToken(MoveToken moveToken)
		{
			SavegameState();
			int index = gameState.InfiniteScoredGameScores[moveToken.FromPlayer].FindIndex((ScoreToken g) => g.TokenName == moveToken.TokenName);
			gameState.InfiniteScoredGameScores[moveToken.ToPlayer].Add(gameState.InfiniteScoredGameScores[moveToken.FromPlayer][index]);
			gameState.TotalScores[moveToken.ToPlayer] = moveToken.ToPlayerScore;
			gameState.InfiniteScoredGameScores[moveToken.FromPlayer].RemoveAt(index);
			gameState.TotalScores[moveToken.FromPlayer] = moveToken.FromPlayerScore;
			gameEvents.RaiseUpdateScoreboardWithCallback(gameState.CurrentPlayer, HandleAfterScoring);
		}

		private void HandleNewGame()
		{
			Reset();
			Initialize();
		}

		private void HandleRemovePlayerToken(string removePlayerName, string removeTokenName, int? playerScore)
		{
			SavegameState();
			RemovePlayerToken(removePlayerName, removeTokenName, playerScore);
			HandleAfterScoring();
		}

		private void HandleResetPlayerScore(string playerName)
		{
			SavegameState();
			ResetPlayerScore(playerName);
			HandleAfterScoring();
		}

		private void HandleUpdateTwoPlayersScores(string restPlayerName, string updatePlayerName, string removeTokenName, int? updatePlayerScore)
		{
			SavegameState();
			ResetPlayerScore(restPlayerName);
			RemovePlayerToken(updatePlayerName, removeTokenName, updatePlayerScore);
			HandleAfterScoring();
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
			gameEvents.RaiseUpdateScoreboard();
		}

		private void PlayerTurnOver()
		{
			if (_currentPlayer.MoveNext())
			{
				GameStateSO gameStateSO = gameState;
				gameStateSO.CurrentPlayer = _currentPlayer.Current.Key;
				gameState.CurrentFrame = gameState.CurrentRoundIndex;
			}
			else
			{
				_currentPlayer = gameState.RoundScores.GetEnumerator();
				_currentPlayer.MoveNext();
				GameStateSO gameStateSO2 = gameState;
				gameStateSO2.CurrentPlayer = _currentPlayer.Current.Key;
				gameState.CurrentRound += gameState.ThrowsPerTurn;
				gameState.CurrentFrame = gameState.CurrentRoundIndex;
				gameState.EnableTarget();
				gameEvents.RaiseNewRound();
			}
			gameState.ThrowsRemaining = 1;
		}

		public override void RecordScore(TwentyOneScoreModel score)
		{
			SavegameState();
			Dictionary<string, List<ScoreToken>> infiniteScoredGameScores = gameState.InfiniteScoredGameScores;
			infiniteScoredGameScores[_currentPlayer.Current.Key].Add(score.ScoreToken);
			Dictionary<string, int?> totalScores = gameState.TotalScores;
			totalScores[_currentPlayer.Current.Key] = score.TotalScore;
			gameEvents.RaiseUpdateScoreboardWithCallback(gameState.CurrentPlayer, HandleAfterScoring);
		}

		private void RemovePlayerToken(string removePlayerName, string removeTokenName, int? playerScore)
		{
			int index = gameState.InfiniteScoredGameScores[removePlayerName].FindIndex((ScoreToken g) => g.TokenName == removeTokenName);
			gameState.InfiniteScoredGameScores[removePlayerName].RemoveAt(index);
			gameState.TotalScores[removePlayerName] = playerScore;
			gameEvents.RaiseRemoveScoreboardPlayerToken(removePlayerName, removeTokenName);
			gameEvents.RaiseUpdatePlayersScoreboard(removePlayerName);
		}

		protected void Reset()
		{
			_gameStateHistory.Clear();
			gameState.RoundScores.Clear();
			gameState.InfiniteScoredGameScores.Clear();
			gameState.TotalScores.Clear();
			gameState.GameStatus = GameStatus.Setup;
			gameState.CurrentPlayer = null;
			gameState.CurrentFrame = 0;
			gameState.CurrentRound = 1;
			gameState.ThrowsRemaining = 1;
		}

		private void ResetPlayerScore(string playerName)
		{
			gameState.InfiniteScoredGameScores[playerName].Clear();
			gameState.TotalScores[playerName] = 0;
		}

		private void SavegameState()
		{
			RaiseSaveGameStateBegin();
			_gameStateHistory.Push(new GameStateHistory
			{
				CurrentFrame = gameState.CurrentFrame,
				CurrentPlayer = gameState.CurrentPlayer,
				CurrentRound = gameState.CurrentRound,
				RoundScores = gameState.RoundScores.SimpleJsonClone(),
				InfiniteScoredGameScores = gameState.InfiniteScoredGameScores.SimpleJsonClone(),
				TotalScores = gameState.TotalScores.SimpleJsonClone()
			});
			RaiseSaveGameStateComplete();
		}

		private void Undo()
		{
			if (_gameStateHistory.Count == 0 || gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				return;
			}
			RaiseUndoBegin();
			gameState.DisableTarget();
			GameStateHistory gameStateHistory = _gameStateHistory.Pop();
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
			RaiseUndoComplete();
			gameEvents.RaiseCustomAfterUndo();
			gameEvents.RaiseUpdatePlayerTurn();
			gameState.EnableTarget();
		}
	}
}
