using System;
using System.Collections.Generic;
using Extensions;
using Games.GameState;
using Games.HitCustomPhotoController.ScriptableObjects;
using Players;
using Settings;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Games.SharedScoringLogic.Standard
{
	[RenamedFrom("StandardRoundBasedScoringStrategy")]
	public class StandardRoundTwoEventCallsBasedScoringLogic : ScoringStrategyBase<StandardScoreModel>
	{
		private Dictionary<string, List<int?>>.Enumerator _currentPlayer;

		private readonly Stack<GameStateHistory> _gameStateHistory = new Stack<GameStateHistory>();

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		protected NumberOfRounds NumberOfRounds
		{
			get
			{
				if (!SettingsStore.Target.FiveFrameGame)
				{
					return NumberOfRounds.TenFrames;
				}
				return NumberOfRounds.FiveFrames;
			}
		}

		protected GameEventsSO GameEvents => gameEvents;

		protected GameStateSO GameState => gameState;

		protected PlayerStateSO PlayerState => playerState;

		public event UnityAction OnInitBegin;

		public event UnityAction OnInitComplete;

		public event UnityAction OnDisableBegin;

		public event UnityAction OnDisableComplete;

		public event UnityAction OnSaveGameStateBegin;

		public event UnityAction OnSaveGameStateComplete;

		public event UnityAction OnFrameUpdateBegin;

		public event UnityAction OnFrameUpdateComplete;

		public event UnityAction OnUndoComplete;

		private void OnEnable()
		{
			if (NumberOfRounds != NumberOfRounds.FiveFrames && NumberOfRounds != NumberOfRounds.TenFrames)
			{
				throw new Exception("This scoring strategy only supports Five or Ten Frame rounds");
			}
			RaiseInitBegin();
			Reset();
			GameEvents.OnBeginScoreEdit += HandleBeingScoreEdit;
			GameEvents.OnCancelScoreEdit += HandleCancelScoreEdit;
			GameEvents.OnConfirmScoreEdit += HandleScoreEdit;
			GameEvents.OnNewGame += HandleNewGame;
			GameEvents.OnMainMenu += HandleMainMenu;
			GameEvents.OnGameOver += GameState.DisableTarget;
			GameEvents.OnUndo += Undo;
			SetupRounds();
			RaiseInitComplete();
			SaveGameState();
		}

		private void OnDisable()
		{
			RaiseDisableBegin();
			GameEvents.OnBeginScoreEdit -= HandleBeingScoreEdit;
			GameEvents.OnCancelScoreEdit -= HandleCancelScoreEdit;
			GameEvents.OnNewGame -= HandleNewGame;
			GameEvents.OnMainMenu -= HandleMainMenu;
			GameEvents.OnGameOver -= GameState.DisableTarget;
			GameEvents.OnUndo -= Undo;
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

		private void RaiseFrameUpdateBegin()
		{
			this.OnFrameUpdateBegin?.Invoke();
		}

		private void RaiseFrameUpdateComplete()
		{
			this.OnFrameUpdateComplete?.Invoke();
		}

		private void RaiseOnUndoComplete()
		{
			this.OnUndoComplete?.Invoke();
		}

		private void SetupRounds()
		{
			gameState.NumberOfRounds = NumberOfRounds;
			foreach (PlayerData player in playerState.players)
			{
				int numberOfRounds = (int)gameState.NumberOfRounds;
				gameState.RoundScores.Add(player.PlayerName, new List<int?>(numberOfRounds));
				for (int i = 1; i <= numberOfRounds; i++)
				{
					gameState.RoundScores[player.PlayerName].Add(null);
				}
			}
			_currentPlayer = gameState.RoundScores.GetEnumerator();
			_currentPlayer.MoveNext();
			GameStateSO gameStateSO = gameState;
			gameStateSO.CurrentPlayer = _currentPlayer.Current.Key;
			GameEvents.RaiseUpdateScoreboard();
		}

		private void HandleCancelScoreEdit()
		{
			if (gameState.GameStatus != GameStatus.Finished)
			{
				GameState.EnableTarget();
			}
		}

		private void HandleNewGame()
		{
			Reset();
			SetupRounds();
		}

		private void HandleMainMenu()
		{
			Reset();
		}

		private void HandleBeingScoreEdit((string playerName, int frameIndex) frameInfo)
		{
			GameState.DisableTarget();
		}

		public void HandleUpdateNextFrame()
		{
			if (gameState.ThrowsRemaining == 0)
			{
				PlayerTurnOver();
			}
			else
			{
				RaiseFrameUpdateBegin();
				gameState.CurrentFrame++;
				RaiseFrameUpdateComplete();
			}
			if (gameState.GameStatus != GameStatus.Finished)
			{
				gameEvents.RaiseUpdateScoreboard();
			}
		}

		public override void RecordScore(StandardScoreModel scoreModel)
		{
			if (gameState.GameStatus != GameStatus.Finished && !GameState.IsTargetDisabled)
			{
				SaveGameState();
				Dictionary<string, List<int?>> roundScores = gameState.RoundScores;
				roundScores[_currentPlayer.Current.Key][gameState.CurrentFrame] = scoreModel.Score;
				gameState.ThrowsRemaining--;
				if (gameState.GameStatus != GameStatus.Finished)
				{
					gameEvents.RaiseUpdateScoreboard();
				}
			}
		}

		private void HandleScoreEdit((string playerName, int frameIndex, int score) editedFrame)
		{
			gameState.RoundScores[editedFrame.playerName][editedFrame.frameIndex] = editedFrame.score;
			foreach (GameStateHistory item in _gameStateHistory)
			{
				if (item.RoundScores[editedFrame.playerName][editedFrame.frameIndex] > 0)
				{
					item.RoundScores[editedFrame.playerName][editedFrame.frameIndex] = editedFrame.score;
				}
			}
			GameEvents.RaiseUpdateScoreboard();
			if (gameState.GameStatus != GameStatus.Finished)
			{
				GameState.EnableTarget();
			}
		}

		private void PlayerTurnOver()
		{
			if (_currentPlayer.MoveNext())
			{
				RaiseFrameUpdateBegin();
				GameStateSO gameStateSO = gameState;
				gameStateSO.CurrentPlayer = _currentPlayer.Current.Key;
				gameState.CurrentFrame = gameState.CurrentRoundIndex;
				RaiseFrameUpdateComplete();
			}
			else
			{
				_currentPlayer = gameState.RoundScores.GetEnumerator();
				_currentPlayer.MoveNext();
				GameStateSO gameStateSO2 = gameState;
				gameStateSO2.CurrentPlayer = _currentPlayer.Current.Key;
				gameState.CurrentRound += gameState.ThrowsPerTurn;
				gameState.CurrentFrame = gameState.CurrentRoundIndex;
				if (gameState.NumberOfRounds - gameState.CurrentRound < NumberOfRounds.Infinite)
				{
					gameState.GameStatus = GameStatus.Finished;
					gameEvents.RaiseGameOver();
					gameEvents.RaiseUpdateScoreboard();
					gameEvents.RaiseWinAnimation();
					return;
				}
				gameEvents.RaiseNewRound();
			}
			hitCustomPhotoEvents.RaiseEndPlayerTurn();
			if (gameState.CurrentRound + gameState.ThrowsPerTurn > (int)gameState.NumberOfRounds)
			{
				gameState.ThrowsRemaining = (int)(gameState.NumberOfRounds - gameState.CurrentFrame);
			}
			else
			{
				gameState.ThrowsRemaining = gameState.ThrowsPerTurn;
			}
		}

		protected void Reset()
		{
			_gameStateHistory.Clear();
			gameState.RoundScores.Clear();
			gameState.GameStatus = GameStatus.Setup;
			gameState.CurrentPlayer = null;
			gameState.CurrentFrame = 0;
			gameState.CurrentRound = 1;
			gameState.ThrowsRemaining = gameState.ThrowsPerTurn;
			GameState.EnableTarget();
		}

		private void SaveGameState()
		{
			RaiseSaveGameStateBegin();
			_gameStateHistory.Push(new GameStateHistory
			{
				CurrentFrame = gameState.CurrentFrame,
				CurrentPlayer = gameState.CurrentPlayer,
				CurrentRound = gameState.CurrentRound,
				ThrowsRemaining = gameState.ThrowsRemaining,
				RoundScores = gameState.RoundScores.SimpleJsonClone()
			});
			RaiseSaveGameStateComplete();
		}

		private void Undo()
		{
			if (_gameStateHistory.Count == 0 || GameState.IsTargetDisabled)
			{
				return;
			}
			GameStateHistory gameStateHistory = _gameStateHistory.Pop();
			gameState.CurrentFrame = gameStateHistory.CurrentFrame;
			gameState.CurrentPlayer = gameStateHistory.CurrentPlayer;
			gameState.CurrentRound = gameStateHistory.CurrentRound;
			gameState.RoundScores = gameStateHistory.RoundScores;
			gameState.ThrowsRemaining = gameStateHistory.ThrowsRemaining;
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
			gameEvents.RaiseUpdateScoreboard();
			RaiseOnUndoComplete();
		}
	}
}
