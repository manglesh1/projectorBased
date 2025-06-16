using System;
using Assets.Games.Norse.ScoreBoard;
using Extensions;
using Games;
using Games.GameState;
using Players;
using Scoreboard.BasicUnscoredScoreboard;
using Scoreboard.BattleshipScoreboard;
using Scoreboard.Concentration;
using Scoreboard.CricketScoreboards;
using Scoreboard.Messaging;
using Scoreboard.StandardScoreboard;
using Scoreboard.TwentyOneScoreboard;
using Scoreboard.UnscoredTwoPlayerScoreboard;
using Scoreboard.WordWhackScoreboard;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Scoreboard.PrefabScripts
{
	public class ScoreboardController : MonoBehaviour
	{
		private const int DEFAULT_THROWS_PER_TURN = 1;

		private GameObject _editScoreCanvas;

		private GameObject _scoreboardControlCanvas;

		private GameObject initializedPrefab;

		[Header("Scoreboard Parent Container")]
		[SerializeField]
		private GameObject scoreboardParent;

		[Header("Scoreboard Events")]
		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoaderEvents;

		[Header("Scoreboard State Needed")]
		[SerializeField]
		private GameEventsSO gameEvent;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[Space]
		[Header("Scoreboard Controllers")]
		[SerializeField]
		private BasicUnscoredScoreboardFactoryController basicUnscoredScoreboardFactory;

		[SerializeField]
		private BattleshipScoreboardFactoryController battleshipScoredScoreboardFactory;

		[SerializeField]
		private ConcentrationScoreboardFactoryController concentrationScoreboardFactory;

		[SerializeField]
		private CricketScoredScoreboardFactoryController cricketScoredScoreboardFactory;

		[SerializeField]
		private CricketUnscoredScoreboardFactoryController cricketUnscoredScoreboardFactory;

		[SerializeField]
		private StandardScoreboardFactoryController standardScoreboardFactory;

		[SerializeField]
		private UnscoredTwoPlayerScoreboardFactoryController unscoredTwoPlayerScoreboardFactory;

		[SerializeField]
		private NorseScoreboardFactoryController norseScoreboardFactory;

		[SerializeField]
		private TwentyOneScoreboardFactoryController twentyOneScoreboardFactory;

		[SerializeField]
		private WordWhackScoreboardFactoryController wordWhackScoreboard;

		public GameObject Current => initializedPrefab;

		private void OnEnable()
		{
			gameEvent.OnBeginScoreEdit += ShowEditScoreMenu;
			gameEvent.OnConfirmScoreEdit += ConfirmEditScore;
			gameEvent.OnCancelScoreEdit += ShowDefaultMenu;
			gameEvent.OnMainMenu += CleanUpChildren;
			scoreboardLoaderEvents.OnLoadScoreboardRequest += Load;
			scoreboardLoaderEvents.OnUnloadScoreboardRequest += CleanUpChildren;
			scoreboardLoaderEvents.OnScoreboardMessageRequest += HideScoreboard;
			scoreboardLoaderEvents.OnScoreboardMessageFinished += ShowScoreboard;
		}

		private void OnDisable()
		{
			gameEvent.OnBeginScoreEdit -= ShowEditScoreMenu;
			gameEvent.OnConfirmScoreEdit -= ConfirmEditScore;
			gameEvent.OnCancelScoreEdit -= ShowDefaultMenu;
			gameEvent.OnMainMenu -= CleanUpChildren;
			scoreboardLoaderEvents.OnLoadScoreboardRequest -= Load;
			scoreboardLoaderEvents.OnUnloadScoreboardRequest -= CleanUpChildren;
			scoreboardLoaderEvents.OnScoreboardMessageRequest -= HideScoreboard;
			scoreboardLoaderEvents.OnScoreboardMessageFinished -= ShowScoreboard;
			CleanUpChildren();
		}

		private void CleanUpChildren()
		{
			if (initializedPrefab != null)
			{
				UnityEngine.Object.Destroy(initializedPrefab);
				initializedPrefab = null;
			}
			for (int i = 0; i < scoreboardParent.transform.childCount; i++)
			{
				UnityEngine.Object.DestroyImmediate(scoreboardParent.transform.GetChild(i).gameObject);
			}
		}

		private void ConfirmEditScore((string playerName, int frameIndex, int score) frameInfo)
		{
			ShowDefaultMenu();
		}

		private void HideScoreboard(ScoreboardMessageRequest request)
		{
			if (!(initializedPrefab == null) && !initializedPrefab.IsHidden())
			{
				initializedPrefab.Hide();
			}
		}

		private void Load(ScoreboardType scoreboardType)
		{
			CleanUpChildren();
			LoadScoreboard(scoreboardType);
			_scoreboardControlCanvas = GameObject.Find("ScoreboardButtons");
			_scoreboardControlCanvas.SetActive(value: true);
			if (gameState.NumberOfRounds == NumberOfRounds.FiveFrames || gameState.NumberOfRounds == NumberOfRounds.TenFrames)
			{
				_editScoreCanvas = GameObject.Find("EditScoreMenu");
				_editScoreCanvas.SetActive(value: false);
			}
			if (!SettingsStore.Target.ThrowsPerTurnEnabled)
			{
				gameState.ThrowsPerTurn = 1;
				gameState.ThrowsRemaining = 1;
			}
		}

		private void LoadScoreboard(ScoreboardType type)
		{
			GameObject scoreboard = GetScoreboard(type);
			initializedPrefab = UnityEngine.Object.Instantiate(scoreboard, scoreboardParent.transform);
			GameObject[] array = GameObject.FindGameObjectsWithTag("ScoreboardDimmerArea");
			if (array.Length != 0)
			{
				GameObject[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].AddComponent<Image>().color = new Color(0f, 0f, 0f, SettingsStore.Backgrounds.TargetDimmer.Alpha);
				}
			}
		}

		private GameObject GetScoreboard(ScoreboardType type)
		{
			switch (type)
			{
			case ScoreboardType.BasicUnscored:
				return basicUnscoredScoreboardFactory.GetScoreboard();
			case ScoreboardType.Battleship:
				return battleshipScoredScoreboardFactory.GetScoreboard(gameState.NumberOfRounds, playerState);
			case ScoreboardType.Concentration:
				return concentrationScoreboardFactory.GetScoreboard(playerState);
			case ScoreboardType.CricketScored:
				return cricketScoredScoreboardFactory.GetScoreboard(playerState);
			case ScoreboardType.CricketUnscored:
				return cricketUnscoredScoreboardFactory.GetScoreboard(playerState);
			case ScoreboardType.Standard:
				return standardScoreboardFactory.GetScoreboard(gameState.NumberOfRounds, playerState);
			case ScoreboardType.Norse:
				return norseScoreboardFactory.GetScoreboard();
			case ScoreboardType.TwentyOne:
				return twentyOneScoreboardFactory.GetScoreboard(gameState.NumberOfRounds, playerState);
			case ScoreboardType.UnscoredTwoPlayer:
				return unscoredTwoPlayerScoreboardFactory.GetScoreboard();
			case ScoreboardType.WordWhack:
				return wordWhackScoreboard.GetScoreboard();
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
		}

		private void ShowDefaultMenu()
		{
			_editScoreCanvas.SetActive(value: false);
			_scoreboardControlCanvas.SetActive(value: true);
		}

		private void ShowEditScoreMenu((string playerName, int frameIndex) frameInfo)
		{
			_scoreboardControlCanvas.SetActive(value: false);
			_editScoreCanvas.SetActive(value: true);
			EditScoresController component = _editScoreCanvas.GetComponent<EditScoresController>();
			component.FrameIndex = frameInfo.frameIndex;
			(component.PlayerName, _) = frameInfo;
		}

		private void ShowScoreboard()
		{
			if (!(initializedPrefab == null))
			{
				initializedPrefab.Show();
			}
		}
	}
}
