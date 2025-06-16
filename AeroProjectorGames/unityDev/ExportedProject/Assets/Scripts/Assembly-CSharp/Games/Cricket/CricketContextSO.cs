using System.Collections.Generic;
using Games.Cricket.Logic.Scoring;
using Players;
using UnityEngine;

namespace Games.Cricket
{
	[CreateAssetMenu(menuName = "Context/Cricket/Cricket Context")]
	public class CricketContextSO : ScriptableObject
	{
		private const int THROWS_PER_TURN_START = 1;

		private const int DEFAULT_THROWS_PER_TURN = 3;

		private int _currentThrow;

		private CricketGameState _cricketGameState;

		private int _currentPlayerIndex;

		private PlayerScoreCollection _playerScoreCollection;

		private int _throwsPerTurn;

		private Stack<CricketContextHistory> _history;

		[Header("States")]
		[SerializeField]
		private PlayerStateSO playerState;

		public CricketGameState CurrentGameState => _cricketGameState;

		public PlayerData CurrentPlayer => playerState.players[_currentPlayerIndex];

		public int CurrentPlayerIndex => _currentPlayerIndex;

		public int CurrentThrow => _currentThrow;

		public PlayerScoreCollection PlayerScoreCollection => _playerScoreCollection;

		public bool IsSinglePlayer => playerState.players.Count == 1;

		public int ThrowsPerTurn => _throwsPerTurn;

		public int ThrowsRemaining => ThrowsPerTurn - (_currentThrow - 1);

		public void Clean()
		{
			_history = new Stack<CricketContextHistory>();
			_currentPlayerIndex = 0;
			_currentThrow = 1;
			_playerScoreCollection = null;
			_throwsPerTurn = 3;
		}

		public void NextPlayer()
		{
			if (_currentPlayerIndex >= playerState.players.Count - 1)
			{
				_currentPlayerIndex = 0;
			}
			else
			{
				_currentPlayerIndex++;
			}
		}

		public bool NextThrow()
		{
			_currentThrow++;
			bool result = true;
			if (_currentThrow > ThrowsPerTurn)
			{
				_currentThrow = 1;
				result = false;
			}
			return result;
		}

		public void SaveState()
		{
			_history.Push(new CricketContextHistory(_currentThrow, _currentPlayerIndex, _cricketGameState, _playerScoreCollection.Clone()));
		}

		public void SetGameState(CricketGameState newState)
		{
			_cricketGameState = newState;
		}

		public void Setup(int throwsPerTurn = 3)
		{
			_history = new Stack<CricketContextHistory>();
			_cricketGameState = CricketGameState.Setup;
			_currentPlayerIndex = 0;
			_currentThrow = 1;
			_playerScoreCollection = new PlayerScoreCollection(playerState.players);
			_throwsPerTurn = throwsPerTurn;
			SaveState();
		}

		public void Undo()
		{
			if (_history.Count > 1)
			{
				CricketContextHistory cricketContextHistory = _history.Pop();
				_currentThrow = cricketContextHistory.CurrentThrow;
				_cricketGameState = cricketContextHistory.CricketGameState;
				_currentPlayerIndex = cricketContextHistory.CurrentPlayerIndex;
				_playerScoreCollection = cricketContextHistory.PlayerScoreCollection;
			}
		}
	}
}
