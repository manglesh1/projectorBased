using System.Collections.Generic;
using Games;
using Games.GameState;
using Settings;
using Target;
using UnityEngine;

namespace WinAnimations
{
	public class WinAnimationManager : MonoBehaviour
	{
		private const string WIN_TEXT_SINGLE_WINNER = "WINNER!";

		private const string WIN_TEXT_TIE = "WE HAVE A TIE!";

		private List<string> _winnersName = new List<string>();

		private List<string> _winningTextToDisplay = new List<string>();

		[SerializeField]
		private List<GameObject> winAnimationPrefabs = new List<GameObject>();

		[SerializeField]
		private WinAnimationSO winAnimationSettings;

		[Header("External References")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[Header("Settings Elements")]
		private TargetSettings _targetSettings;

		private void OnEnable()
		{
			gameEvents.OnNewGame += ResetWindAnimation;
			gameEvents.OnPlayWinAnimation += PlayWinningAnimation;
			gameEvents.OnPlayWinAnimationForPlayer += PlayAnimationWithWinner;
			gameEvents.OnPlayWinAnimationForPlayers += PlayAnimationForWinners;
			winAnimationPrefabs.RemoveAll((GameObject g) => g == null);
			ResetWindAnimation();
		}

		private void OnDisable()
		{
			gameEvents.OnNewGame -= ResetWindAnimation;
			gameEvents.OnPlayWinAnimation -= PlayWinningAnimation;
			gameEvents.OnPlayWinAnimationForPlayer -= PlayAnimationWithWinner;
			gameEvents.OnPlayWinAnimationForPlayers -= PlayAnimationForWinners;
		}

		private void DetermineWinner()
		{
			_winnersName.Clear();
			if (gameState.RoundScores.Count > 0)
			{
				int num = -99;
				int num2 = 0;
				{
					foreach (KeyValuePair<string, List<int?>> roundScore in gameState.RoundScores)
					{
						num2 = 0;
						foreach (int? item in roundScore.Value)
						{
							int value = item.Value;
							num2 += value;
						}
						if (num2 > num)
						{
							num = num2;
							_winnersName.Clear();
							_winnersName.Add(roundScore.Key);
						}
						else if (num2 == num)
						{
							_winnersName.Add(roundScore.Key);
						}
					}
					return;
				}
			}
			_winnersName.Add(gameState.CurrentPlayer);
		}

		private int RadonmNumber()
		{
			int count = winAnimationPrefabs.Count;
			return Random.Range(0, count);
		}

		private void PlayAnimationWithWinner(string winnerName)
		{
			_targetSettings = SettingsStore.Target;
			if (!_targetSettings.PlayWinAnimations)
			{
				return;
			}
			_winnersName.Add(winnerName);
			_winningTextToDisplay.Clear();
			if (_winnersName.Count == 1)
			{
				_winningTextToDisplay.Add("WINNER!");
			}
			else
			{
				_winningTextToDisplay.Add("WE HAVE A TIE!");
			}
			foreach (string item in _winnersName)
			{
				_winningTextToDisplay.Add(item);
			}
			winAnimationSettings.TextToShow = _winningTextToDisplay;
			Object.Instantiate(winAnimationPrefabs[RadonmNumber()], base.gameObject.transform);
		}

		private void PlayAnimationForWinners(List<string> winnerNames)
		{
			_targetSettings = SettingsStore.Target;
			if (_targetSettings.PlayWinAnimations)
			{
				_winnersName.AddRange(winnerNames);
				_winningTextToDisplay.Clear();
				if (_winnersName.Count == 1)
				{
					_winningTextToDisplay.Add("WINNER!");
				}
				else
				{
					_winningTextToDisplay.Add("WE HAVE A TIE!");
				}
				_winningTextToDisplay.AddRange(_winnersName);
				winAnimationSettings.TextToShow = _winningTextToDisplay;
				Object.Instantiate(winAnimationPrefabs[RadonmNumber()], base.gameObject.transform);
			}
		}

		private void PlayWinningAnimation()
		{
			_targetSettings = SettingsStore.Target;
			if (!_targetSettings.PlayWinAnimations)
			{
				return;
			}
			DetermineWinner();
			_winningTextToDisplay.Clear();
			if (_winnersName.Count == 1)
			{
				_winningTextToDisplay.Add("WINNER!");
			}
			else
			{
				_winningTextToDisplay.Add("WE HAVE A TIE!");
			}
			foreach (string item in _winnersName)
			{
				_winningTextToDisplay.Add(item);
			}
			winAnimationSettings.TextToShow = _winningTextToDisplay;
			Object.Instantiate(winAnimationPrefabs[RadonmNumber()], base.gameObject.transform);
		}

		private void ResetWindAnimation()
		{
			_winnersName.Clear();
		}
	}
}
