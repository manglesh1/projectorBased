using System.Collections;
using System.Collections.Generic;
using Games.Concentration.SO;
using Games.Concentration.Scripts.Enums;
using Games.Concentration.Scripts.Gameboard.Models;
using Games.Concentration.Scripts.Gameboard.SaveState;
using Games.Concentration.Scripts.Themes;
using Games.GameState;
using UnityEngine;

namespace Games.Concentration.Scripts.Gameboard
{
	public class GameboardController : MonoBehaviour
	{
		private const int MATCHING_CARDS_TO_FIND_COUNT = 1;

		private const int GAME_TOKEN_BACK_MATERIAL_INDEX = 1;

		private const int GAME_TOKEN_FACE_MATERIAL_INDEX = 2;

		private IEnumerator _undoFlipFromFaceDownToFaceUpEnumerator;

		private IEnumerator _UndoFlipFromFaceUpToFaceDownEnumerator;

		private List<Renderer> _gameboardSetupTokenRenderers = new List<Renderer>();

		private List<Renderer> _gameTokenRenderersInPlay = new List<Renderer>();

		private Stack<GameboardSaveStateModel> _gameboardStateHistory = new Stack<GameboardSaveStateModel>();

		private string _previousAutoSelectedTokenName;

		private int _stealCardsInPlayCount;

		private int _wildCardsInPlayCount;

		[Header("Gameboard Elements")]
		[SerializeField]
		private GameTokenLayoutModel mainGameboardLayoutModel;

		[Header("Gameboard Main Groups")]
		[SerializeField]
		private GameObject gameboardMainGroup;

		[SerializeField]
		private GameObject gameboardParentGroup;

		[Header("Scriptable Objects")]
		[SerializeField]
		private ConcentrationGameEventsSO concentrationGameEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private ConcentrationGameSettingsSO gameSettings;

		[SerializeField]
		private GameStateSO gameState;

		[Header("External References")]
		[SerializeField]
		private ConcentrationController concentrationController;

		[SerializeField]
		private ThemeManager themeManager;

		public int GameTokenFaceMaterialIndex => 2;

		public int GameTokenMatchesLeftInPlay => _gameTokenRenderersInPlay.Count / 2;

		private void OnEnable()
		{
			gameboardMainGroup.SetActive(value: true);
			gameboardParentGroup.SetActive(value: true);
			AtEnableAddListeners();
		}

		private void OnDisable()
		{
			if (_undoFlipFromFaceDownToFaceUpEnumerator != null)
			{
				StopCoroutine(_undoFlipFromFaceDownToFaceUpEnumerator);
			}
			if (_UndoFlipFromFaceUpToFaceDownEnumerator != null)
			{
				StopCoroutine(_UndoFlipFromFaceUpToFaceDownEnumerator);
			}
			AtDisableRemoveListeners();
		}

		private void AtEnableAddListeners()
		{
			concentrationGameEvents.OnAutomaticallySelectFirstCard += HandleSelectingFirstCardThatFlipsAutomatically;
			concentrationGameEvents.OnRemoveMatchingTargets += HandleRemoveMatchingTargets;
			concentrationGameEvents.OnRemoveMatchingTargetsWithWild += HandleRemoveMatchingTargetsWithWild;
			concentrationGameEvents.OnSaveGameboardState += SaveCurrentGameboardState;
			concentrationGameEvents.OnSetupGameboard += SetupGameboard;
			gameEvents.OnUndo += HandleUndo;
		}

		private void AtDisableRemoveListeners()
		{
			concentrationGameEvents.OnAutomaticallySelectFirstCard -= HandleSelectingFirstCardThatFlipsAutomatically;
			concentrationGameEvents.OnRemoveMatchingTargets -= HandleRemoveMatchingTargets;
			concentrationGameEvents.OnRemoveMatchingTargetsWithWild -= HandleRemoveMatchingTargetsWithWild;
			concentrationGameEvents.OnSaveGameboardState -= SaveCurrentGameboardState;
			concentrationGameEvents.OnSetupGameboard -= SetupGameboard;
			gameEvents.OnUndo -= HandleUndo;
		}

		private void CheckIfStealCardsShouldBeSubtractedFromPlay(GameObject autoSelectedGameToken, GameObject playerSelectedGameToken, List<GameObject> matchingTokensToRemove = null)
		{
			Object obj = autoSelectedGameToken.GetComponent<Renderer>().materials[2];
			Material material = playerSelectedGameToken.GetComponent<Renderer>().materials[2];
			bool flag = obj.name.Contains(themeManager.SelectedStealCardMaterial.name);
			bool flag2 = material.name.Contains(themeManager.SelectedStealCardMaterial.name);
			if (flag)
			{
				_stealCardsInPlayCount--;
			}
			if (flag2)
			{
				_stealCardsInPlayCount--;
			}
			if (matchingTokensToRemove == null)
			{
				return;
			}
			foreach (GameObject item in matchingTokensToRemove)
			{
				if (item.GetComponent<Renderer>().materials[2].name.Contains(themeManager.SelectedStealCardMaterial.name))
				{
					_stealCardsInPlayCount--;
				}
			}
		}

		private void CheckIfWildCardsShouldBeSubtractedFromPlay(GameObject playerSelectedGameToken)
		{
			if (playerSelectedGameToken.GetComponent<Renderer>().materials[2].name.Contains(themeManager.SelectedWildCardMaterial.name))
			{
				_wildCardsInPlayCount--;
			}
		}

		private void DuringUndoResetGameboardTokenStates(List<GameboardLayoutStateModel> gameboardTokenHistory)
		{
			int count = mainGameboardLayoutModel.GameTokens.Count;
			List<int> list = new List<int>();
			PopulateGameTokenRendererList();
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = mainGameboardLayoutModel.GameTokens[i];
				gameObject.SetActive(gameboardTokenHistory[i].isActive);
				if (!gameboardTokenHistory[i].isActive)
				{
					list.Add(i);
				}
				else if (gameboardTokenHistory[i].isFaceDown)
				{
					_UndoFlipFromFaceUpToFaceDownEnumerator = UndoFlipFromFaceUpToFaceDown(gameObject);
					StartCoroutine(_UndoFlipFromFaceUpToFaceDownEnumerator);
				}
				else
				{
					_undoFlipFromFaceDownToFaceUpEnumerator = UndoFlipFromFaceDownToFaceUp(gameObject);
					StartCoroutine(_undoFlipFromFaceDownToFaceUpEnumerator);
					concentrationGameEvents.RaiseStoreAutoSelectedTokenValue(gameObject);
				}
			}
			RemoveFromInPlayList(list);
		}

		private void FlipAutoSelectedGameToken(int gameTokenIndex)
		{
			_previousAutoSelectedTokenName = _gameTokenRenderersInPlay[gameTokenIndex].gameObject.name;
			concentrationGameEvents.RaiseFlipAutoSelectedGameToken(_gameTokenRenderersInPlay[gameTokenIndex].gameObject);
			concentrationGameEvents.RaiseStoreAutoSelectedTokenValue(_gameTokenRenderersInPlay[gameTokenIndex].gameObject);
		}

		public List<GameObject> GetMatchingTokenForWildMatch(GameObject gameTokenToFindItsMatch)
		{
			Material material = gameTokenToFindItsMatch.GetComponent<Renderer>().materials[2];
			int num = 1;
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < _gameTokenRenderersInPlay.Count; i++)
			{
				if (!(gameTokenToFindItsMatch.name == _gameTokenRenderersInPlay[i].gameObject.name))
				{
					if (num <= 0)
					{
						break;
					}
					Material material2 = _gameTokenRenderersInPlay[i].materials[2];
					if (material.name == material2.name)
					{
						list.Add(_gameTokenRenderersInPlay[i].gameObject);
						num--;
					}
				}
			}
			return list;
		}

		private List<GameboardLayoutStateModel> GetCurrentGameTokenState()
		{
			List<GameboardLayoutStateModel> list = new List<GameboardLayoutStateModel>();
			foreach (GameObject gameToken in mainGameboardLayoutModel.GameTokens)
			{
				bool activeSelf = gameToken.activeSelf;
				bool isFaceDown = gameToken.transform.rotation.eulerAngles.y < 100f;
				list.Add(new GameboardLayoutStateModel
				{
					isActive = activeSelf,
					isFaceDown = isFaceDown
				});
			}
			return list;
		}

		private void HandleRemoveMatchingTargets(GameObject autoSelectedGameToken, GameObject playerSelectedGameToken)
		{
			CheckIfStealCardsShouldBeSubtractedFromPlay(autoSelectedGameToken, playerSelectedGameToken);
			List<int> list = new List<int>();
			for (int i = 0; i < _gameTokenRenderersInPlay.Count; i++)
			{
				if (autoSelectedGameToken.name == _gameTokenRenderersInPlay[i].gameObject.name)
				{
					autoSelectedGameToken.SetActive(value: false);
					list.Add(i);
				}
				else if (playerSelectedGameToken.name == _gameTokenRenderersInPlay[i].gameObject.name)
				{
					playerSelectedGameToken.SetActive(value: false);
					list.Add(i);
				}
			}
			RemoveFromInPlayList(list);
		}

		private void HandleRemoveMatchingTargetsWithWild(GameObject autoSelectedGameToken, GameObject playerSelectedGameToken, List<GameObject> matchingTokensToRemove)
		{
			CheckIfStealCardsShouldBeSubtractedFromPlay(autoSelectedGameToken, playerSelectedGameToken, matchingTokensToRemove);
			CheckIfWildCardsShouldBeSubtractedFromPlay(playerSelectedGameToken);
			List<int> list = new List<int>();
			for (int i = 0; i < _gameTokenRenderersInPlay.Count; i++)
			{
				if (autoSelectedGameToken.name == _gameTokenRenderersInPlay[i].gameObject.name)
				{
					autoSelectedGameToken.SetActive(value: false);
					list.Add(i);
					continue;
				}
				if (playerSelectedGameToken.name == _gameTokenRenderersInPlay[i].gameObject.name)
				{
					playerSelectedGameToken.SetActive(value: false);
					list.Add(i);
					continue;
				}
				foreach (GameObject item in matchingTokensToRemove)
				{
					if (item.name == _gameTokenRenderersInPlay[i].gameObject.name)
					{
						item.SetActive(value: false);
						list.Add(i);
					}
				}
			}
			RemoveFromInPlayList(list);
		}

		private void HandleSelectingFirstCardThatFlipsAutomatically()
		{
			bool flag = concentrationController.DetermineIfCurrentPlayerHasHighestScore() != PlayerScoringStates.HighestScore;
			int count = _gameTokenRenderersInPlay.Count;
			int num = Random.Range(0, count);
			for (int i = 0; i < count; i++)
			{
				Material material = _gameTokenRenderersInPlay[num].materials[2];
				string text = _gameTokenRenderersInPlay[num].gameObject.name;
				bool activeSelf = _gameTokenRenderersInPlay[num].gameObject.activeSelf;
				bool flag2 = text == _previousAutoSelectedTokenName;
				bool flag3 = material.name.Contains(themeManager.SelectedStealCardMaterial.name);
				bool flag4 = material.name.Contains(themeManager.SelectedWildCardMaterial.name);
				if (activeSelf && flag3 && flag && !flag2)
				{
					FlipAutoSelectedGameToken(num);
					return;
				}
				if (activeSelf && !flag4 && !flag3 && !flag2)
				{
					FlipAutoSelectedGameToken(num);
					return;
				}
				num++;
				if (num >= count)
				{
					num = 0;
				}
			}
			concentrationGameEvents.RaiseNoStandardTargetsToFlip();
		}

		private void HandleUndo()
		{
			if (!gameState.IsTargetDisabled && gameState.GameStatus != GameStatus.Finished)
			{
				gameState.DisableTarget();
				if (_gameboardStateHistory.Count == 0)
				{
					gameState.EnableTarget();
					return;
				}
				GameboardSaveStateModel gameboardSaveStateModel = _gameboardStateHistory.Pop();
				_stealCardsInPlayCount = gameboardSaveStateModel.StealCardsInPlayCount;
				_wildCardsInPlayCount = gameboardSaveStateModel.WildCardsInPlayCount;
				DuringUndoResetGameboardTokenStates(gameboardSaveStateModel.GameboardLayoutState);
			}
		}

		private void PopulateGameTokenRendererList()
		{
			_gameboardSetupTokenRenderers = new List<Renderer>();
			_gameTokenRenderersInPlay = new List<Renderer>();
			foreach (GameObject gameToken in mainGameboardLayoutModel.GameTokens)
			{
				gameToken.SetActive(value: true);
				_gameboardSetupTokenRenderers.Add(gameToken.GetComponent<Renderer>());
				_gameTokenRenderersInPlay.Add(gameToken.GetComponent<Renderer>());
			}
		}

		private void RemoveFromInPlayList(List<int> gameTokensToRemove)
		{
			for (int num = gameTokensToRemove.Count - 1; num >= 0; num--)
			{
				_gameTokenRenderersInPlay.RemoveAt(gameTokensToRemove[num]);
			}
		}

		private void SaveCurrentGameboardState()
		{
			GameboardSaveStateModel gameboardSaveStateModel = new GameboardSaveStateModel();
			gameboardSaveStateModel.GameboardLayoutState = GetCurrentGameTokenState();
			gameboardSaveStateModel.StealCardsInPlayCount = _stealCardsInPlayCount;
			gameboardSaveStateModel.WildCardsInPlayCount = _wildCardsInPlayCount;
			_gameboardStateHistory.Push(gameboardSaveStateModel);
			concentrationGameEvents.RaiseFinishedSavingGameboardState();
		}

		private void SetGameTokenBackMaterial()
		{
			Material selectedCardBackMatrial = themeManager.SelectedCardBackMatrial;
			foreach (Renderer gameboardSetupTokenRenderer in _gameboardSetupTokenRenderers)
			{
				Material[] materials = gameboardSetupTokenRenderer.materials;
				materials[1] = selectedCardBackMatrial;
				gameboardSetupTokenRenderer.materials = materials;
			}
		}

		private void SetGameTokenFrontSpecialMatchMaterials(int cardCount, Material cardMaterial)
		{
			for (int i = 0; i < cardCount; i++)
			{
				if (_gameboardSetupTokenRenderers.Count > 0)
				{
					int index = Random.Range(0, _gameboardSetupTokenRenderers.Count);
					Material[] materials = _gameboardSetupTokenRenderers[index].materials;
					materials[2] = cardMaterial;
					_gameboardSetupTokenRenderers[index].materials = materials;
					_gameboardSetupTokenRenderers.RemoveAt(index);
				}
			}
		}

		private void SetGameTokenFrontStandardMatchMaterials()
		{
			int standardCardCount = themeManager.StandardCardCount;
			foreach (Material standardCardFrontMaterial in themeManager.GetStandardCardFrontMaterials(gameSettings.GameDifficulty))
			{
				for (int i = 0; i < standardCardCount; i++)
				{
					if (_gameboardSetupTokenRenderers.Count > 0)
					{
						int index = Random.Range(0, _gameboardSetupTokenRenderers.Count);
						Material[] materials = _gameboardSetupTokenRenderers[index].materials;
						materials[2] = standardCardFrontMaterial;
						_gameboardSetupTokenRenderers[index].materials = materials;
						_gameboardSetupTokenRenderers.RemoveAt(index);
					}
				}
			}
		}

		private void SetupGameboard()
		{
			_gameboardStateHistory = new Stack<GameboardSaveStateModel>();
			PopulateGameTokenRendererList();
			SetGameTokenBackMaterial();
			SetGameTokenFrontStandardMatchMaterials();
			_stealCardsInPlayCount = themeManager.StealCardCount;
			SetGameTokenFrontSpecialMatchMaterials(_stealCardsInPlayCount, themeManager.SelectedStealCardMaterial);
			_wildCardsInPlayCount = themeManager.GetWildCardCount(gameSettings.GameDifficulty);
			SetGameTokenFrontSpecialMatchMaterials(_wildCardsInPlayCount, themeManager.SelectedWildCardMaterial);
			concentrationGameEvents.RaiseFinishedGameboardSetup();
		}

		private IEnumerator UndoFlipFromFaceDownToFaceUp(GameObject tokenToFlip)
		{
			yield return null;
			concentrationGameEvents.RaisePlayUndoFaceDownToFaceUpAnimation(tokenToFlip);
		}

		private IEnumerator UndoFlipFromFaceUpToFaceDown(GameObject tokenToFlip)
		{
			yield return null;
			concentrationGameEvents.RaisePlayUndoFaceUpToFaceDownAnimation(tokenToFlip);
		}
	}
}
