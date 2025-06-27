using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Games.GameState;
using Players;
using UnityEngine;

namespace Assets.Games.Norse.Scripts
{
	public class NorseScoringLogic : MonoBehaviour
	{
		private int _currentPlayerIndex;

		private Stack<NorseStateSO> _history;

		private int _previousPowerUp = -1;

		[Header("State")]
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private NorseStateSO norseState;

		[Header("Events")]
		[SerializeField]
		private NorseEventsSO norseEvents;

		public void AddHistory()
		{
			_history.Push(norseState.SimpleJsonClone());
		}

		public void ExecutePowerUp()
		{
			switch (norseState.SelectedPowerUp)
			{
			case NorsePowerUpEnum.Attack:
				ExecutePowerUpAttack();
				break;
			case NorsePowerUpEnum.ExtraLife:
				ExecutePowerUpExtraLife();
				break;
			case NorsePowerUpEnum.Protect:
				ExecutePowerUpProtect();
				break;
			case NorsePowerUpEnum.Reverse:
				ExecutePowerUpReverse();
				break;
			case NorsePowerUpEnum.Steal:
				ExecutePowerUpSteal();
				break;
			}
		}

		private void ExecutePowerUpAttack()
		{
			RecordMiss(norseState.CommandingPlayer);
		}

		private void ExecutePowerUpExtraLife()
		{
			norseState.Players[gameState.CurrentPlayer].NumberOfMisses--;
			if (norseState.Players[gameState.CurrentPlayer].NumberOfMisses < 0)
			{
				norseState.Players[gameState.CurrentPlayer].NumberOfMisses = 0;
			}
		}

		private void ExecutePowerUpProtect()
		{
			if (norseState.Players.TryGetValue(gameState.CurrentPlayer, out var value))
			{
				int num = norseState.GameWord.Length - value.NumberOfMisses;
				if (num > 0 && value.NumberOfShields < num)
				{
					norseState.Players[gameState.CurrentPlayer].NumberOfShields++;
				}
			}
		}

		private void ExecutePowerUpReverse()
		{
			norseState.DirectionIsReversed = !norseState.DirectionIsReversed;
		}

		private void ExecutePowerUpSteal()
		{
			SetCommander(gameState.CurrentPlayer);
		}

		public NorsePowerUpEnum GetRandomPowerUp()
		{
			NorsePowerUpEnum norsePowerUpEnum = NorsePowerUpEnum.Attack;
			bool flag = false;
			int num;
			do
			{
				num = UnityEngine.Random.Range(0, Enum.GetValues(typeof(NorsePowerUpEnum)).Length);
				if (num != _previousPowerUp)
				{
					norsePowerUpEnum = (NorsePowerUpEnum)num;
					flag = norsePowerUpEnum switch
					{
						NorsePowerUpEnum.ExtraLife => norseState.Players[gameState.CurrentPlayer].NumberOfMisses > 0, 
						NorsePowerUpEnum.Protect => norseState.Players[gameState.CurrentPlayer].NumberOfShields == 0, 
						NorsePowerUpEnum.Reverse => norseState.Players.Count > 2, 
						_ => true, 
					};
				}
			}
			while (!flag);
			_previousPowerUp = num;
			return norsePowerUpEnum;
		}

		public bool HasPlayerWon()
		{
			return norseState.Players.Count((KeyValuePair<string, NorsePlayerState> p) => !p.Value.IsOutOfMisses) == 1;
		}

		public void Initialize()
		{
			_currentPlayerIndex = 0;
			if (_history == null)
			{
				_history = new Stack<NorseStateSO>();
			}
			_history.Clear();
			SetCurrentPlayer();
		}

		public void NextPlayer()
		{
			do
			{
				if (norseState.DirectionIsReversed)
				{
					if (_currentPlayerIndex <= 0)
					{
						_currentPlayerIndex = playerState.CurrentPlayerNames.Count - 1;
					}
					else
					{
						_currentPlayerIndex--;
					}
				}
				else if (_currentPlayerIndex >= playerState.CurrentPlayerNames.Count - 1)
				{
					_currentPlayerIndex = 0;
				}
				else
				{
					_currentPlayerIndex++;
				}
				SetCurrentPlayer();
				if (norseState.CommandingPlayer == gameState.CurrentPlayer && norseState.Players[gameState.CurrentPlayer].IsOutOfMisses)
				{
					norseState.CommandingPlayer = string.Empty;
					norseState.IsCommandSet = false;
					norseEvents.RaiseOnHideGamePieces();
				}
			}
			while (norseState.Players[gameState.CurrentPlayer].IsOutOfMisses);
		}

		public void RecordMiss(string playerName)
		{
			if (norseState.Players.TryGetValue(playerName, out var value))
			{
				if (value.NumberOfShields > 0)
				{
					value.NumberOfShields--;
				}
				else
				{
					value.NumberOfMisses++;
				}
				value.IsOutOfMisses = value.NumberOfMisses >= norseState.GameWord.Length;
				norseState.Players[playerName] = value;
			}
		}

		public void SetCommander(string playerName)
		{
			norseState.CommandingPlayer = playerName;
			norseState.IsCommandSet = true;
		}

		private void SetCurrentPlayer()
		{
			gameState.CurrentPlayer = playerState.CurrentPlayerNames[_currentPlayerIndex];
			norseState.PlayerName = gameState.CurrentPlayer;
		}

		public void UndoHistory()
		{
			if (_history.Count > 0)
			{
				NorseStateSO norseStateSO = _history.Pop();
				norseState.CommandingPlayer = norseStateSO.CommandingPlayer;
				norseState.GameWord = norseStateSO.GameWord;
				norseState.IsCommandSet = norseStateSO.IsCommandSet;
				norseState.PlayerName = norseStateSO.PlayerName;
				norseState.Players = norseStateSO.Players;
				norseState.PowerUpLocationX = norseStateSO.PowerUpLocationX;
				norseState.PowerUpLocationY = norseStateSO.PowerUpLocationY;
				norseState.SelectedPowerUp = norseStateSO.SelectedPowerUp;
				norseState.TargetLocationX = norseStateSO.TargetLocationX;
				norseState.TargetLocationY = norseStateSO.TargetLocationY;
				_currentPlayerIndex = playerState.CurrentPlayerNames.IndexOf(norseStateSO.PlayerName);
				gameState.CurrentPlayer = norseStateSO.PlayerName;
			}
		}

		private void OnDisable()
		{
		}

		private void OnEnable()
		{
			_currentPlayerIndex = 0;
		}
	}
}
