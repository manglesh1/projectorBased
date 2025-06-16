using System.Collections.Generic;
using Admin.Demo;
using Detection;
using Games;
using Games.Models;
using Settings;
using Timer;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
	public class MainMenuController : MonoBehaviour
	{
		private Dictionary<string, GameObject> _gameButtonsMap = new Dictionary<string, GameObject>();

		private GameObject _currentSubMenu;

		private List<GameSO> _viewableGames = new List<GameSO>();

		[SerializeField]
		private GameObject buttonPrefab;

		[SerializeField]
		private DemoSO demoState;

		[SerializeField]
		private DetectionUIStateSO detectionUiState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameManager gameManager;

		[SerializeField]
		private GameObject menuScrollViewObject;

		[SerializeField]
		private TimerEventsSO timerEvents;

		[SerializeField]
		private TimerStateSO timerState;

		[SerializeField]
		private ViewableGamesSO viewableGames;

		[Header("Multi-Display")]
		[SerializeField]
		private bool isMultiDisplayMenu;

		[SerializeField]
		private MainMenuController primaryDisplayController;

		private void Awake()
		{
			ResetMenuList();
		}

		private void OnEnable()
		{
			gameEvents.OnCheckAndDestroySubMenu += CheckAndDestroySubMenu;
			gameEvents.OnViewableGamesUpdated += ResetMenuList;
			gameEvents.OnMainMenu += CheckAndDestroySubMenu;
			CheckAndDestroySubMenu();
			detectionUiState.SetClosed();
		}

		private void OnDisable()
		{
			gameEvents.OnCheckAndDestroySubMenu -= CheckAndDestroySubMenu;
			gameEvents.OnViewableGamesUpdated -= ResetMenuList;
			gameEvents.OnMainMenu -= CheckAndDestroySubMenu;
		}

		public void ResetMenuList()
		{
			foreach (KeyValuePair<string, GameObject> item in _gameButtonsMap)
			{
				Object.DestroyImmediate(item.Value);
			}
			_gameButtonsMap.Clear();
			LoadGamesFromViewableGames();
		}

		private void LoadGamesFromViewableGames()
		{
			_viewableGames = viewableGames.GetViewableGames();
			foreach (GameSO game in _viewableGames)
			{
				GameObject gameObject = Object.Instantiate(buttonPrefab, base.transform);
				gameObject.name = game.GameName;
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<GameSelectionButton>().SetupGameComponents(game);
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					CheckTimerThenLoadGameOrMenu(game);
				});
				_gameButtonsMap.Add(game.GameName, gameObject);
				gameObject.SetActive(value: true);
			}
		}

		private void CheckAndDestroySubMenu()
		{
			if (_currentSubMenu != null)
			{
				Object.Destroy(_currentSubMenu);
			}
		}

		private void CheckTimerThenLoadGameOrMenu(GameSO game)
		{
			if (!demoState.DemoIsRunning && timerState.CurrentState != TimerStateEnum.Disabled && timerState.CurrentState != TimerStateEnum.Running)
			{
				timerEvents.RaiseStartTimerWarning();
			}
			else if (game.SubGameSelectionPrefab == null)
			{
				gameManager.LoadGame(game.GamePrefab, game);
			}
			else if (SettingsStore.Interaction.MultiDisplayEnabled && isMultiDisplayMenu)
			{
				primaryDisplayController.LoadSubMenu(game.SubGameSelectionPrefab);
				LoadSubMenu(game.SubGameSelectionPrefab);
			}
			else
			{
				LoadSubMenu(game.SubGameSelectionPrefab);
			}
		}

		public void LoadSubMenu(GameObject prefab)
		{
			_currentSubMenu = Object.Instantiate(prefab, menuScrollViewObject.transform);
		}
	}
}
