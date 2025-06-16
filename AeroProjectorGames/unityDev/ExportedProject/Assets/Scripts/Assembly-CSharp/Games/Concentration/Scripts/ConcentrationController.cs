using System.Collections;
using System.Collections.Generic;
using Games.Concentration.SO;
using Games.Concentration.Scoreboard;
using Games.Concentration.Scripts.Enums;
using Games.Concentration.Scripts.Gameboard;
using Games.Concentration.Scripts.PraiseList;
using Games.Concentration.Scripts.Themes;
using Games.GameState;
using HitEffects;
using Scoreboard;
using Scoreboard.Messaging;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Concentration.Scripts
{
	public class ConcentrationController : MonoBehaviour
	{
		private const ConcentrationGameStates GAME_FIRST_STATE = ConcentrationGameStates.DifficultySelection;

		private IEnumerator _autoFlipfirstCardEnumerator;

		private IEnumerator _showPlayerTurnMessageEnumerator;

		private GameObject _autoSelectedGameToken;

		private int _lastPlayerPraiseIndexUsed;

		private bool _missRoutineRunning;

		private GameObject _playerSelectedGameToken;

		private bool _playerGotMatch;

		[Header("Multi Display - Core")]
		[SerializeField]
		private GameObject multiDisplayPanel;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[Header("Multi Display - Views")]
		[SerializeField]
		private GameObject multiDisplayTokenContainer;

		[SerializeField]
		private GameObject multiDisplayDifficultySelectionMenuGroup;

		[SerializeField]
		private GameObject multiDisplayThemeSelectionMenuGroup;

		[Header("Game Main Groups")]
		[SerializeField]
		private GameObject difficultySelectionMenuGroup;

		[SerializeField]
		private GameObject gamePlayGroup;

		[SerializeField]
		private GameObject themeSelectionMenuGroup;

		[Header("Scriptable Objects")]
		[SerializeField]
		private ConcentrationGameEventsSO concentrationGameEvents;

		[SerializeField]
		private ConcentrationScoreboardEventsSO concentrationScoreboardEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private ConcentrationGameSettingsSO gameSettings;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("External References")]
		[SerializeField]
		private GameboardController gameboardController;

		[SerializeField]
		private ThemeManager themeManager;

		private void OnEnable()
		{
			gameState.NumberOfRounds = NumberOfRounds.InfiniteScored;
			HandleResetGame();
			AtEnableAddListeners();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayPanel);
			}
			else
			{
				multiDisplayPanel.SetActive(value: false);
			}
		}

		private void OnDisable()
		{
			AtDisableRemoveListeners();
			AtDisableStopAllCoroutines();
		}

		private void AtEnableAddListeners()
		{
			concentrationGameEvents.OnFinishedFailedMatchAnimation += HandleEndOfPlayerTurn;
			concentrationGameEvents.OnFinishedGameboardUndo += gameState.EnableTarget;
			concentrationGameEvents.OnFinishedHitAutoSelctedtokenAnimation += HandleEndOfPlayerTurn;
			concentrationGameEvents.OnFinishedMatchTwoTokensAnimation += HandleEndOfPlayerTurn;
			concentrationGameEvents.OnFinishedMatchWithWildAnimation += HandleEndOfPlayerTurn;
			concentrationGameEvents.OnNoStandardTargetsToFlip += HandleNoStandardCardsLeftToFlip;
			concentrationGameEvents.OnChangeGameState += HandleGameStateChange;
			concentrationGameEvents.OnFinishedFlippngAutoSelecedCardFaceUp += gameState.EnableTarget;
			concentrationGameEvents.OnFinishedFlippngPlayerSelecedCardFaceUp += CompareSelectedGameTokens;
			concentrationGameEvents.OnFinishedGameboardSetup += HandleStartPlayerTurn;
			concentrationGameEvents.OnFinishedSavingGameboardState += HandleTargetClicked;
			concentrationGameEvents.OnStoreAutoSelectedTokenValue += HandleStoringAutoSelectedToken;
			concentrationGameEvents.OnTargetClicked += HandleSavingGameboardState;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnUpdatePlayerTurn += HandleStartPlayerTurn;
		}

		private void AtDisableRemoveListeners()
		{
			concentrationGameEvents.OnFinishedFailedMatchAnimation -= HandleEndOfPlayerTurn;
			concentrationGameEvents.OnFinishedGameboardUndo -= gameState.EnableTarget;
			concentrationGameEvents.OnFinishedHitAutoSelctedtokenAnimation -= HandleEndOfPlayerTurn;
			concentrationGameEvents.OnFinishedMatchTwoTokensAnimation -= HandleEndOfPlayerTurn;
			concentrationGameEvents.OnFinishedMatchWithWildAnimation -= HandleEndOfPlayerTurn;
			concentrationGameEvents.OnNoStandardTargetsToFlip -= HandleNoStandardCardsLeftToFlip;
			concentrationGameEvents.OnChangeGameState -= HandleGameStateChange;
			concentrationGameEvents.OnFinishedFlippngAutoSelecedCardFaceUp -= gameState.EnableTarget;
			concentrationGameEvents.OnFinishedFlippngPlayerSelecedCardFaceUp -= CompareSelectedGameTokens;
			concentrationGameEvents.OnFinishedGameboardSetup -= HandleStartPlayerTurn;
			concentrationGameEvents.OnFinishedSavingGameboardState -= HandleTargetClicked;
			concentrationGameEvents.OnStoreAutoSelectedTokenValue -= HandleStoringAutoSelectedToken;
			concentrationGameEvents.OnTargetClicked -= HandleSavingGameboardState;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnUpdatePlayerTurn -= HandleStartPlayerTurn;
		}

		private void AtDisableStopAllCoroutines()
		{
			if (_autoFlipfirstCardEnumerator != null)
			{
				StopCoroutine(_autoFlipfirstCardEnumerator);
			}
			if (_showPlayerTurnMessageEnumerator != null)
			{
				StopCoroutine(_showPlayerTurnMessageEnumerator);
			}
		}

		private bool CheckForWinCondition(bool noCardsToFlip = false)
		{
			int gameTokenMatchesLeftInPlay = gameboardController.GameTokenMatchesLeftInPlay;
			List<string> playersWithHighestScore = GetPlayersWithHighestScore();
			if (gameTokenMatchesLeftInPlay == 0)
			{
				HandleWinCondition(playersWithHighestScore);
				return true;
			}
			if (playersWithHighestScore.Count == 1)
			{
				int? highestScoreValue = GetHighestScoreValue();
				if (gameTokenMatchesLeftInPlay < highestScoreValue - GetSecondPlaceScoreValue(highestScoreValue))
				{
					HandleWinCondition(playersWithHighestScore);
					return true;
				}
			}
			else if (noCardsToFlip)
			{
				HandleWinCondition(playersWithHighestScore);
				return true;
			}
			return false;
		}

		private void CompareSelectedGameTokens()
		{
			string text = _autoSelectedGameToken.GetComponent<Renderer>().materials[gameboardController.GameTokenFaceMaterialIndex].name;
			string text2 = _playerSelectedGameToken.GetComponent<Renderer>().materials[gameboardController.GameTokenFaceMaterialIndex].name;
			bool flag = text2.Contains(themeManager.SelectedWildCardMaterial.name);
			bool flag2 = text.Contains(themeManager.SelectedStealCardMaterial.name);
			bool flag3 = text2.Contains(themeManager.SelectedStealCardMaterial.name);
			if (text == text2)
			{
				_playerGotMatch = true;
				if (flag3)
				{
					List<string> playersWithHighestScore = GetPlayersWithHighestScore();
					playersWithHighestScore = RemoveCurrentPlayerFromList(playersWithHighestScore);
					string randomPlayerFromList = GetRandomPlayerFromList(playersWithHighestScore);
					concentrationScoreboardEvents.RaiseRecordRemoveStolenTokenScore(randomPlayerFromList, themeManager.TokenMatchScoreValue);
					concentrationGameEvents.RaisePlayStealTokenAnimation(_autoSelectedGameToken, _playerSelectedGameToken, randomPlayerFromList);
				}
				else
				{
					concentrationScoreboardEvents.RaiseRecordStandardScore(_playerSelectedGameToken, themeManager.TokenMatchScoreValue);
					concentrationGameEvents.RaisePlayMatchTwoTokensAnimation(_autoSelectedGameToken, _playerSelectedGameToken);
				}
			}
			else if (!flag)
			{
				_playerGotMatch = false;
				concentrationScoreboardEvents.RaiseRecordMissedScore();
				concentrationGameEvents.RaisePlayFailedMatchAnimation(_autoSelectedGameToken, _playerSelectedGameToken);
			}
			else
			{
				_playerGotMatch = true;
				if (flag2)
				{
					List<string> playersWithHighestScore2 = GetPlayersWithHighestScore();
					playersWithHighestScore2 = RemoveCurrentPlayerFromList(playersWithHighestScore2);
					string randomPlayerFromList2 = GetRandomPlayerFromList(playersWithHighestScore2);
					concentrationScoreboardEvents.RaiseRecordRemoveStolenTokenScore(randomPlayerFromList2, themeManager.TokenMatchScoreValue);
					concentrationGameEvents.RaisePlayStealTokenWithWildAnimation(_autoSelectedGameToken, _playerSelectedGameToken, randomPlayerFromList2);
				}
				else
				{
					concentrationScoreboardEvents.RaiseRecordWildScore(_autoSelectedGameToken, themeManager.TokenMatchScoreValue);
					concentrationGameEvents.RaisePlayMatchWithWildAnimation(_autoSelectedGameToken, _playerSelectedGameToken);
				}
			}
		}

		public PlayerScoringStates DetermineIfCurrentPlayerHasHighestScore()
		{
			List<string> playersWithHighestScore = GetPlayersWithHighestScore();
			if (playersWithHighestScore.Count == 0)
			{
				return PlayerScoringStates.HighestScore;
			}
			if (playersWithHighestScore.Count == gameState.TotalScores.Count)
			{
				return PlayerScoringStates.AllPlayersTied;
			}
			foreach (string item in playersWithHighestScore)
			{
				if (item == gameState.CurrentPlayer)
				{
					return PlayerScoringStates.HighestScore;
				}
			}
			return PlayerScoringStates.LowerScore;
		}

		private void DisableAllMainGameObjectGroups()
		{
			difficultySelectionMenuGroup.SetActive(value: false);
			gamePlayGroup.SetActive(value: false);
			themeSelectionMenuGroup.SetActive(value: false);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayTokenContainer.SetActive(value: false);
				multiDisplayDifficultySelectionMenuGroup.SetActive(value: false);
				multiDisplayThemeSelectionMenuGroup.SetActive(value: false);
			}
		}

		private void EnablingGameplaySetup()
		{
			gameState.DisableTarget();
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Concentration);
			gamePlayGroup.SetActive(value: true);
			concentrationGameEvents.RaiseSetupGameboard();
		}

		private int? GetHighestScoreValue()
		{
			int? num = -1;
			foreach (KeyValuePair<string, int?> totalScore in gameState.TotalScores)
			{
				if (num < totalScore.Value)
				{
					num = totalScore.Value;
				}
			}
			return num;
		}

		private List<string> GetPlayersWithHighestScore()
		{
			List<string> list = new List<string>();
			int? num = -1;
			foreach (KeyValuePair<string, int?> totalScore in gameState.TotalScores)
			{
				if (num < totalScore.Value)
				{
					num = totalScore.Value;
					list = new List<string>();
					list.Add(totalScore.Key);
				}
				else if (num == totalScore.Value)
				{
					list.Add(totalScore.Key);
				}
			}
			if (num <= 0)
			{
				return new List<string>();
			}
			return list;
		}

		private string GetRandomPlayerFromList(List<string> playersWithHighestScore)
		{
			return playersWithHighestScore[Random.Range(0, playersWithHighestScore.Count)];
		}

		private int? GetSecondPlaceScoreValue(int? highestScore)
		{
			int? num = -1;
			foreach (KeyValuePair<string, int?> totalScore in gameState.TotalScores)
			{
				if (highestScore != totalScore.Value && num < totalScore.Value)
				{
					num = totalScore.Value;
				}
			}
			return num;
		}

		private void HandleEndOfPlayerTurn()
		{
			if (!CheckForWinCondition())
			{
				if (gameSettings.SamePlayerAfterMatch && _playerGotMatch)
				{
					HandleStartPlayerTurn();
				}
				else
				{
					concentrationGameEvents.RaiseEndPlayerTurn();
				}
			}
		}

		private void HandleSavingGameboardState(GameObject clickedTarget)
		{
			_playerSelectedGameToken = clickedTarget;
			concentrationGameEvents.RaiseSaveGameboardState();
		}

		private void HandleGameStateChange(ConcentrationGameStates newGameState)
		{
			DisableAllMainGameObjectGroups();
			UnloadAllScoreboardObjects();
			switch (newGameState)
			{
			case ConcentrationGameStates.Playing:
				EnablingGameplaySetup();
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayTokenContainer.SetActive(value: true);
				}
				break;
			case ConcentrationGameStates.ThemeSelection:
				gameEvents.RaiseAddToGameFlexSpace(Object.Instantiate(themeSelectionMenuGroup));
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayThemeSelectionMenuGroup.SetActive(value: true);
				}
				break;
			default:
				gameEvents.RaiseAddToGameFlexSpace(Object.Instantiate(difficultySelectionMenuGroup));
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayDifficultySelectionMenuGroup.SetActive(value: true);
				}
				break;
			}
		}

		private void HandleMiss()
		{
			if (!gameState.IsTargetDisabled && gameState.GameStatus != GameStatus.Finished)
			{
				gameState.DisableTarget();
				HandleSavingGameboardState(null);
			}
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			StartCoroutine(MissDetectedRoutine(pointerEventData, screenPoint));
		}

		private void HandleNewGame()
		{
			HandleGameStateChange(ConcentrationGameStates.DifficultySelection);
		}

		private void HandleNoStandardCardsLeftToFlip()
		{
			CheckForWinCondition(noCardsToFlip: true);
		}

		private void HandleResetGame()
		{
			HandleGameStateChange(ConcentrationGameStates.DifficultySelection);
		}

		private void HandleStartPlayerTurn()
		{
			gameState.DisableTarget();
			if (_playerGotMatch && gameSettings.SamePlayerAfterMatch)
			{
				_showPlayerTurnMessageEnumerator = ShowPlayerTurnMessage();
				StartCoroutine(_showPlayerTurnMessageEnumerator);
			}
			else
			{
				_autoFlipfirstCardEnumerator = AutoFlipfirstCard();
				StartCoroutine(_autoFlipfirstCardEnumerator);
			}
			_playerGotMatch = false;
		}

		private void HandleStoringAutoSelectedToken(GameObject autoSelectedToken)
		{
			_autoSelectedGameToken = autoSelectedToken;
		}

		private void HandleTargetClicked()
		{
			if (!(_playerSelectedGameToken != null))
			{
				concentrationScoreboardEvents.RaiseRecordMissedScore();
				concentrationGameEvents.RaisePlayMissAnimation(_autoSelectedGameToken);
			}
			else if (_autoSelectedGameToken.name == _playerSelectedGameToken.name)
			{
				concentrationScoreboardEvents.RaiseRecordMissedScore();
				concentrationGameEvents.RaisePlayHitAutoSelctedtokenAnimation(_autoSelectedGameToken);
			}
			else
			{
				concentrationGameEvents.RaiseFlipPlayerSelectedGameToken(_playerSelectedGameToken);
			}
		}

		private void HandleWinCondition(List<string> winningNamesList)
		{
			gameEvents.RaiseGameOver();
			if (winningNamesList.Count == 1)
			{
				gameEvents.RaiseWinAnimationForPlayer(winningNamesList[0]);
			}
			else
			{
				gameEvents.RaiseWinAnimationForPlayers(winningNamesList);
			}
		}

		private List<string> RemoveCurrentPlayerFromList(List<string> playerList)
		{
			playerList.Remove(gameState.CurrentPlayer);
			return playerList;
		}

		private void UnloadAllScoreboardObjects()
		{
			gameEvents.RaiseRemoveAllFromGameFlexSpace();
			scoreboardLoader.RaiseUnloadScoreboardRequest();
		}

		private IEnumerator AutoFlipfirstCard()
		{
			yield return null;
			concentrationGameEvents.RaiseAutomaticallySelectFirstCard();
		}

		private IEnumerator MissDetectedRoutine(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			_missRoutineRunning = true;
			gameState.DisableTarget();
			if (screenPoint.HasValue)
			{
				hitEffectEvents.RaiseHitEffect(screenPoint.Value);
			}
			scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, "Miss!", ScoreboardMessageStyle.Normal));
			yield return new WaitForSeconds(3f);
			gameState.EnableTarget();
			HandleMiss();
			_missRoutineRunning = false;
		}

		private IEnumerator ShowPlayerTurnMessage()
		{
			float secondsToWait = 2f;
			yield return null;
			PlayerPraiseList playerPraiseList = new PlayerPraiseList();
			int count = playerPraiseList.PraiseList.Count;
			int num = Random.Range(0, count);
			if (_lastPlayerPraiseIndexUsed == num)
			{
				num++;
				if (num >= count)
				{
					num = 0;
				}
			}
			_lastPlayerPraiseIndexUsed = num;
			scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, playerPraiseList.PraiseList[num], ScoreboardMessageStyle.Normal));
			yield return new WaitForSeconds(secondsToWait);
			concentrationGameEvents.RaiseAutomaticallySelectFirstCard();
		}
	}
}
