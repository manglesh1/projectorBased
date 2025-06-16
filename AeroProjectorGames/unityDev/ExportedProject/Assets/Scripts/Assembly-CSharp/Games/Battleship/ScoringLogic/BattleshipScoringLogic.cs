using System.Collections;
using System.Collections.Generic;
using Extensions;
using Games.Battleship.Scoreboard;
using Games.GameState;
using Games.SharedScoringLogic;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Games.Battleship.ScoringLogic
{
	public class BattleshipScoringLogic : ScoringStrategyBase<BattleshipScoreModel>
	{
		private const int THROWS_PER_TURN = 1;

		private Dictionary<string, List<int?>>.Enumerator _currentPlayer;

		private List<string> _eliminatedPlayers = new List<string>();

		private Stack<BattleshipScoringStateHistory> _gameStateHistory = new Stack<BattleshipScoringStateHistory>();

		private bool _missRoutineRunning;

		private Dictionary<string, BattleshipPlayerScoreboardStateHolder> _playerScoreboardStateholder = new Dictionary<string, BattleshipPlayerScoreboardStateHolder>();

		private string _playerToRemove;

		[SerializeField]
		private BattleshipScoreboardEventsSO battleshipScoreboardEvents;

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
			battleshipScoreboardEvents.OnCheckForWinCondition += CheckWinCondition;
			battleshipScoreboardEvents.OnCustomAfterNewGame += HandleNewGame;
			battleshipScoreboardEvents.OnCustomAfterUndo += HandleCustomUndo;
			battleshipScoreboardEvents.OnUpdateEliminatedPlayers += HandleUpdateEliminatedPlayers;
			gameEvents.OnMainMenu += HandleMainMenu;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnUndo += Undo;
			Reset();
			Initialize();
			RaiseInitComplete();
		}

		protected void OnDisable()
		{
			RaiseDisableBegin();
			battleshipScoreboardEvents.OnCheckForWinCondition -= CheckWinCondition;
			battleshipScoreboardEvents.OnCustomAfterNewGame -= HandleNewGame;
			battleshipScoreboardEvents.OnCustomAfterUndo -= HandleCustomUndo;
			battleshipScoreboardEvents.OnUpdateEliminatedPlayers -= HandleUpdateEliminatedPlayers;
			gameEvents.OnMainMenu -= HandleMainMenu;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnUndo -= Undo;
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

		private bool CheckIfShipIsSunk(string attackedPlayerName)
		{
			BattleshipPlayerScoreboardStateHolder battleshipPlayerScoreboardStateHolder = _playerScoreboardStateholder[attackedPlayerName];
			if (battleshipPlayerScoreboardStateHolder.TwoHitShipHitCount >= 2 && !battleshipPlayerScoreboardStateHolder.IsTwoHitShipSunk)
			{
				battleshipScoreboardEvents.RaiseSetSunkenShipFlameImage(attackedPlayerName, 2, showFlameImage: true);
				battleshipPlayerScoreboardStateHolder.IsTwoHitShipSunk = true;
				return true;
			}
			if (battleshipPlayerScoreboardStateHolder.ThreeHitShipHitCount >= 3 && !battleshipPlayerScoreboardStateHolder.IsThreeHitShipSunk)
			{
				battleshipScoreboardEvents.RaiseSetSunkenShipFlameImage(attackedPlayerName, 3, showFlameImage: true);
				battleshipPlayerScoreboardStateHolder.IsThreeHitShipSunk = true;
				return true;
			}
			if (battleshipPlayerScoreboardStateHolder.FourHitShipHitCount >= 4 && !battleshipPlayerScoreboardStateHolder.IsFourHitShipSunk)
			{
				battleshipScoreboardEvents.RaiseSetSunkenShipFlameImage(attackedPlayerName, 4, showFlameImage: true);
				battleshipPlayerScoreboardStateHolder.IsFourHitShipSunk = true;
				return true;
			}
			return false;
		}

		private void CheckWinCondition()
		{
			int count = _eliminatedPlayers.Count;
			int count2 = playerState.players.Count;
			if (count + 1 == count2)
			{
				gameEvents.RaiseWinAnimationForPlayer(gameState.CurrentPlayer);
				RemoveEliminatedPlayer();
				HandleGameOver();
			}
			else
			{
				HandleAfterScoring();
			}
		}

		private void HandleAfterScoring()
		{
			gameState.ThrowsRemaining--;
			if (gameState.ThrowsRemaining == 0)
			{
				PlayerTurnOver();
			}
			else
			{
				gameState.CurrentFrame++;
			}
			gameEvents.RaiseUpdatePlayerTurn();
			if (gameState.GameStatus != GameStatus.Finished)
			{
				battleshipScoreboardEvents.RaiseShowNextPlayerMessage();
			}
		}

		private void HandleCustomUndo(Dictionary<string, BattleshipPlayerScoreboardStateHolder> playerScoreboardStateHolder)
		{
			foreach (KeyValuePair<string, BattleshipPlayerScoreboardStateHolder> item in playerScoreboardStateHolder)
			{
				if (_eliminatedPlayers.Contains(item.Key))
				{
					battleshipScoreboardEvents.RaiseUpdateEliminatedPlayers(item.Key, removePlayerFromElimination: false, isUndo: true);
				}
				else
				{
					battleshipScoreboardEvents.RaiseUpdateEliminatedPlayers(item.Key, removePlayerFromElimination: true, isUndo: true);
				}
				battleshipScoreboardEvents.RaiseSetSunkenShipFlameImage(item.Key, 2, item.Value.IsTwoHitShipSunk);
				battleshipScoreboardEvents.RaiseSetSunkenShipFlameImage(item.Key, 3, item.Value.IsThreeHitShipSunk);
				battleshipScoreboardEvents.RaiseSetSunkenShipFlameImage(item.Key, 4, item.Value.IsFourHitShipSunk);
			}
		}

		private void HandleGameOver()
		{
			gameState.DisableTarget();
			gameState.GameStatus = GameStatus.Finished;
			gameEvents.RaiseGameOver();
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

		private void HandleNewGame()
		{
			if (!gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				Reset();
				Initialize();
			}
		}

		private void HandleUpdateEliminatedPlayers(string playerToEliminate, bool removePlayerFromElimination = false, bool isUpdate = false)
		{
			if (removePlayerFromElimination)
			{
				if (_eliminatedPlayers.Contains(playerToEliminate))
				{
					_eliminatedPlayers.Remove(playerToEliminate);
				}
			}
			else if (!_eliminatedPlayers.Contains(playerToEliminate))
			{
				_eliminatedPlayers.Add(playerToEliminate);
			}
		}

		private void Initialize()
		{
			foreach (PlayerData player in playerState.players)
			{
				int capacity = 0;
				gameState.RoundScores.Add(player.PlayerName, new List<int?>(capacity));
				gameState.InfiniteScoredGameScores.Add(player.PlayerName, new List<ScoreToken>());
				gameState.TotalScores.Add(player.PlayerName, 0);
				_playerScoreboardStateholder.Add(player.PlayerName, new BattleshipPlayerScoreboardStateHolder());
			}
			_currentPlayer = gameState.RoundScores.GetEnumerator();
			_currentPlayer.MoveNext();
			GameStateSO gameStateSO = gameState;
			gameStateSO.CurrentPlayer = _currentPlayer.Current.Key;
			gameEvents.RaiseUpdateScoreboard();
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

		private void PlayerTurnOver()
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			bool flag3;
			do
			{
				flag3 = _currentPlayer.MoveNext();
				if (flag3)
				{
					List<string> eliminatedPlayers = _eliminatedPlayers;
					if (!eliminatedPlayers.Contains(_currentPlayer.Current.Key))
					{
						flag = true;
					}
				}
				else
				{
					_currentPlayer = gameState.RoundScores.GetEnumerator();
					flag2 = true;
				}
				num++;
			}
			while (!flag && num <= playerState.players.Count);
			if (flag3 && !flag2)
			{
				GameStateSO gameStateSO = gameState;
				gameStateSO.CurrentPlayer = _currentPlayer.Current.Key;
				gameState.CurrentFrame = gameState.CurrentRoundIndex;
			}
			else
			{
				GameStateSO gameStateSO2 = gameState;
				gameStateSO2.CurrentPlayer = _currentPlayer.Current.Key;
				gameState.CurrentRound += gameState.ThrowsPerTurn;
				gameState.CurrentFrame = gameState.CurrentRoundIndex;
			}
			gameState.ThrowsRemaining = 1;
		}

		public override void RecordScore(BattleshipScoreModel scoreModel)
		{
			SavegameState();
			UpdatePlayerState(scoreModel);
			if (scoreModel.Score == 0)
			{
				HandleAfterScoring();
				return;
			}
			if (!CheckIfShipIsSunk(scoreModel.AttackedPlayerName))
			{
				HandleAfterScoring();
				return;
			}
			BattleshipPlayerScoreboardStateHolder battleshipPlayerScoreboardStateHolder = _playerScoreboardStateholder[scoreModel.AttackedPlayerName];
			if (battleshipPlayerScoreboardStateHolder.IsTwoHitShipSunk && battleshipPlayerScoreboardStateHolder.IsThreeHitShipSunk && battleshipPlayerScoreboardStateHolder.IsFourHitShipSunk)
			{
				_playerToRemove = scoreModel.AttackedPlayerName;
				battleshipScoreboardEvents.RaiseUpdateEliminatedPlayers(_playerToRemove);
				battleshipScoreboardEvents.RaisePlayerSunkShip(scoreModel.Score, isAllShipsSunk: true, scoreModel.AttackedPlayerName, HandleAfterScoring);
			}
			else
			{
				battleshipScoreboardEvents.RaisePlayerSunkShip(scoreModel.Score, isAllShipsSunk: false, scoreModel.AttackedPlayerName, HandleAfterScoring);
			}
		}

		protected void Reset()
		{
			_eliminatedPlayers.Clear();
			_playerScoreboardStateholder.Clear();
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

		public bool RemoveEliminatedPlayer()
		{
			if (!string.IsNullOrEmpty(_playerToRemove))
			{
				battleshipScoreboardEvents.RaiseRemoveEliminatedPlayers(_playerToRemove);
				_playerToRemove = string.Empty;
			}
			return true;
		}

		private void SavegameState()
		{
			RaiseSaveGameStateBegin();
			_gameStateHistory.Push(new BattleshipScoringStateHistory
			{
				CurrentFrame = gameState.CurrentFrame,
				CurrentPlayer = gameState.CurrentPlayer,
				CurrentRound = gameState.CurrentRound,
				EliminatedPlayers = new List<string>(_eliminatedPlayers),
				PlayerScoreboardStateholder = _playerScoreboardStateholder.SimpleJsonClone(),
				RoundScores = gameState.RoundScores.SimpleJsonClone(),
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
			gameState.DisableTarget();
			BattleshipScoringStateHistory battleshipScoringStateHistory = _gameStateHistory.Pop();
			gameState.CurrentFrame = battleshipScoringStateHistory.CurrentFrame;
			gameState.CurrentPlayer = battleshipScoringStateHistory.CurrentPlayer;
			gameState.CurrentRound = battleshipScoringStateHistory.CurrentRound;
			_eliminatedPlayers = new List<string>(battleshipScoringStateHistory.EliminatedPlayers);
			_playerScoreboardStateholder = battleshipScoringStateHistory.PlayerScoreboardStateholder;
			gameState.RoundScores = battleshipScoringStateHistory.RoundScores;
			gameState.TotalScores = battleshipScoringStateHistory.TotalScores;
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
			battleshipScoreboardEvents.RaiseCustomAfterUndo(_playerScoreboardStateholder);
			gameEvents.RaiseCustomAfterUndo();
			gameState.EnableTarget();
		}

		private void UpdatePlayerState(BattleshipScoreModel scoreModel)
		{
			switch (scoreModel.Score)
			{
			case 2:
				_playerScoreboardStateholder[scoreModel.AttackedPlayerName].TwoHitShipHitCount++;
				break;
			case 3:
				_playerScoreboardStateholder[scoreModel.AttackedPlayerName].ThreeHitShipHitCount++;
				break;
			case 4:
				_playerScoreboardStateholder[scoreModel.AttackedPlayerName].FourHitShipHitCount++;
				break;
			}
		}
	}
}
