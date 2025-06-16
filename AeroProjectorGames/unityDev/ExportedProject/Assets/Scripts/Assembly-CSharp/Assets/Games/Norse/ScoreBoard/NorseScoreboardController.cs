using System;
using System.Collections.Generic;
using Assets.Games.Norse.Scripts;
using Games;
using Games.GameState;
using Players;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Games.Norse.ScoreBoard
{
	public class NorseScoreboardController : MonoBehaviour
	{
		[Header("Scriptable Objects")]
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private NorseStateSO norseState;

		[Header("Player Prefabs")]
		[SerializeField]
		private List<NorseScoreboardPlayerController> playerControllers;

		[Header("Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private NorseEventsSO norseEvents;

		[Header("UI Views")]
		[SerializeField]
		private GameObject helpBoard;

		[SerializeField]
		private GameObject introBoard;

		[SerializeField]
		private GameObject menuButtons;

		[SerializeField]
		private GameObject scoreBoard;

		[Header("UI Elements")]
		[SerializeField]
		private Button helpButton;

		private const int MAX_PLAYERS = 6;

		private Dictionary<string, NorseScoreboardPlayerController> playerControllerDictionary;

		private Vector3 grandParentLocation;

		private bool viewHelp;

		private void Initialize()
		{
			if (playerControllers.Count != 6)
			{
				throw new ArgumentOutOfRangeException($"PlayerControllers does not have {6} players");
			}
			playerControllerDictionary = new Dictionary<string, NorseScoreboardPlayerController>(6);
			for (int i = 0; i < 6; i++)
			{
				if (playerState.players.Count >= i + 1)
				{
					InitializePlayerScoreboardController(playerState.players[i].PlayerName, playerControllers[i]);
					playerControllerDictionary.Add(playerState.players[i].PlayerName, playerControllers[i]);
				}
				else
				{
					playerControllers[i].SetVisible(visible: false);
				}
			}
		}

		private void InitializePlayerScoreboardController(string playerName, NorseScoreboardPlayerController controller)
		{
			controller.SetLeader(leader: false);
			controller.SetMissCount(0);
			controller.SetPlayerName(playerName);
			controller.SetProtected(isProtected: false);
			controller.SetVisible(visible: true);
		}

		private void OnGameStarted()
		{
			helpBoard.SetActive(value: false);
			introBoard.SetActive(value: false);
			menuButtons.SetActive(value: true);
			scoreBoard.SetActive(value: true);
			helpButton.gameObject.SetActive(value: true);
		}

		private void OnHelpClosed()
		{
			helpBoard.SetActive(value: false);
			scoreBoard.SetActive(value: true);
		}

		private void OnHelpOpened()
		{
			helpBoard.SetActive(value: true);
			scoreBoard.SetActive(value: false);
		}

		private void OnHelpClicked()
		{
			viewHelp = !viewHelp;
			if (viewHelp)
			{
				norseEvents.RaiseOnHelpOpened();
			}
			else
			{
				norseEvents.RaiseOnHelpClosed();
			}
		}

		private void OnIntroStarted()
		{
			helpBoard.SetActive(value: false);
			introBoard.SetActive(value: true);
			menuButtons.SetActive(value: false);
			scoreBoard.SetActive(value: false);
			helpButton.gameObject.SetActive(value: false);
		}

		private void OnWordSelection()
		{
			helpBoard.SetActive(value: false);
			introBoard.SetActive(value: false);
			menuButtons.SetActive(value: false);
			scoreBoard.SetActive(value: false);
			helpButton.gameObject.SetActive(value: false);
		}

		private void UpdateScoreboard()
		{
			for (int i = 0; i < playerState.players.Count; i++)
			{
				NorseScoreboardPlayerController norseScoreboardPlayerController = playerControllers[i];
				NorsePlayerState norsePlayerState = norseState.Players[playerState.players[i].PlayerName];
				bool leader = norseState.CommandingPlayer.Equals(playerState.players[i].PlayerName);
				norseScoreboardPlayerController.SetMissCount(norsePlayerState.NumberOfMisses);
				norseScoreboardPlayerController.SetLeader(leader);
				norseScoreboardPlayerController.SetProtected(norsePlayerState.NumberOfShields > 0);
				norseScoreboardPlayerController.UpdateUI();
			}
		}

		private void OnDisable()
		{
			gameEvents.OnUpdateScoreboard -= UpdateScoreboard;
			gameEvents.OnUpdatePlayerTurn -= UpdateScoreboard;
			helpButton.onClick.RemoveAllListeners();
			norseEvents.OnGameStarted -= OnGameStarted;
			norseEvents.OnHelpClosed -= OnHelpClosed;
			norseEvents.OnHelpOpened -= OnHelpOpened;
			norseEvents.OnIntroStarted -= OnIntroStarted;
			norseEvents.OnWordSelection -= OnWordSelection;
		}

		private void OnEnable()
		{
			Initialize();
			gameEvents.OnUpdateScoreboard += UpdateScoreboard;
			gameEvents.OnUpdatePlayerTurn += UpdateScoreboard;
			helpButton.onClick.AddListener(OnHelpClicked);
			norseEvents.OnGameStarted += OnGameStarted;
			norseEvents.OnHelpClosed += OnHelpClosed;
			norseEvents.OnHelpOpened += OnHelpOpened;
			norseEvents.OnIntroStarted += OnIntroStarted;
			norseEvents.OnWordSelection += OnWordSelection;
			helpBoard.SetActive(value: false);
			scoreBoard.SetActive(value: true);
		}
	}
}
