using System.Collections;
using System.Collections.Generic;
using Extensions;
using Games.GameState;
using Games.SharedScoringLogic.UnscoredTwoPlayer;
using Games.Tic_Tac_Toe.Scripts;
using HitEffects;
using Scoreboard;
using Scoreboard.Messaging;
using Scoreboard.PrefabScripts;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Tic_Tac_Toe
{
	public class TicTacToeGameController : MonoBehaviour
	{
		private List<int[]> _winningCombinations = new List<int[]>
		{
			new int[3] { 0, 1, 2 },
			new int[3] { 3, 4, 5 },
			new int[3] { 6, 7, 8 },
			new int[3] { 0, 4, 8 },
			new int[3] { 2, 4, 6 },
			new int[3] { 0, 3, 6 },
			new int[3] { 1, 4, 7 },
			new int[3] { 2, 5, 8 }
		};

		private Dictionary<string, int> _gameboardGirdIndex = new Dictionary<string, int>
		{
			{ "Top Left", 0 },
			{ "Top Center", 1 },
			{ "Top Right", 2 },
			{ "Middle Left", 3 },
			{ "Middle Center", 4 },
			{ "Middle Right", 5 },
			{ "Bottom Left", 6 },
			{ "Bottom Center", 7 },
			{ "Bottom Right", 8 }
		};

		private Dictionary<int, TicTacToeBoardSection> _gameboard = new Dictionary<int, TicTacToeBoardSection>();

		private bool _missRoutineRunning;

		private Dictionary<int, string> _playerLocations = new Dictionary<int, string>();

		private Stack<Dictionary<int, string>> _playerLocationHistory = new Stack<Dictionary<int, string>>();

		private ScoreboardController _scoreboardController;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameObject multiDisplayScoringPanel;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Player Tokens")]
		[SerializeField]
		private Sprite Player1Token;

		[SerializeField]
		private Sprite Player2Token;

		[Header("Scoring Strategy")]
		[SerializeField]
		private TwoPlayerUnscoredScoringStrategy scoringStrategy;

		[Header("Gameboard")]
		public TicTacToeBoard board;

		private void OnEnable()
		{
			gameState.NumberOfRounds = NumberOfRounds.Infinite;
			gameState.Player1Icon = Player1Token;
			gameState.Player2Icon = Player2Token;
			gameEvents.OnGameObjectClicked += ProcessClick;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnNewGame += InitializeGame;
			gameEvents.OnUndo += HandleUndo;
			_gameboard.Add(board.TopLeft.BoardIndex, board.TopLeft);
			_gameboard.Add(board.TopCenter.BoardIndex, board.TopCenter);
			_gameboard.Add(board.TopRight.BoardIndex, board.TopRight);
			_gameboard.Add(board.MiddleLeft.BoardIndex, board.MiddleLeft);
			_gameboard.Add(board.MiddleCenter.BoardIndex, board.MiddleCenter);
			_gameboard.Add(board.MiddleRight.BoardIndex, board.MiddleRight);
			_gameboard.Add(board.BottomLeft.BoardIndex, board.BottomLeft);
			_gameboard.Add(board.BottomCenter.BoardIndex, board.BottomCenter);
			_gameboard.Add(board.BottomRight.BoardIndex, board.BottomRight);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayScoringPanel);
			}
		}

		protected void OnDisable()
		{
			gameEvents.OnGameObjectClicked -= ProcessClick;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnNewGame -= InitializeGame;
			gameEvents.OnUndo -= HandleUndo;
		}

		private void Start()
		{
			InitializeGame();
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.UnscoredTwoPlayer);
		}

		private void AddCurrentBoardToHistory()
		{
			_playerLocationHistory.Push(_playerLocations.SimpleJsonClone());
		}

		private void InitializeGame()
		{
			_playerLocations.Clear();
			_playerLocationHistory.Clear();
			foreach (KeyValuePair<int, TicTacToeBoardSection> item in _gameboard)
			{
				item.Value.GameboardSpriteRenderer.sprite = null;
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					item.Value.MultiDisplayScoringPanelSpriteRenderer.sprite = null;
				}
			}
		}

		private void HandleMiss()
		{
			scoringStrategy.RecordScore(null);
			AddCurrentBoardToHistory();
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			StartCoroutine(MissDetectedRoutine(pointerEventData, screenPoint));
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

		private void ProcessClick(GameObject clickedGameObject)
		{
			if (gameState.GameStatus != GameStatus.Finished && !gameState.IsTargetDisabled)
			{
				AddCurrentBoardToHistory();
				int num = _gameboardGirdIndex[clickedGameObject.name];
				if ((_playerLocations.ContainsKey(num) ? _playerLocations[num] : null) == null)
				{
					_playerLocations.Add(num, gameState.CurrentPlayer);
					SetIcon(num, gameState.CurrentPlayer);
				}
				else
				{
					_playerLocations.Remove(num);
					_gameboard[num].GameboardSpriteRenderer.sprite = null;
					_gameboard[num].MultiDisplayScoringPanelSpriteRenderer.sprite = null;
				}
				List<int> winningCombination = GetWinningCombination();
				if (winningCombination.Count > 0)
				{
					gameEvents.RaiseGameOver();
					StartCoroutine(AnimateWinningTokens(winningCombination));
					gameEvents.RaiseWinAnimation();
				}
				else
				{
					scoringStrategy.RecordScore(null);
				}
			}
		}

		private List<int> GetWinningCombination()
		{
			List<int> list = new List<int>();
			foreach (int[] winningCombination in _winningCombinations)
			{
				int[] array = winningCombination;
				foreach (int num in array)
				{
					list.Add(num);
					if (!_playerLocations.ContainsKey(num) || _playerLocations[num] != gameState.CurrentPlayer)
					{
						list.Clear();
						break;
					}
				}
				if (list.Count > 0)
				{
					break;
				}
			}
			return list;
		}

		private void SetIcon(int gameboardIndex, string playerName)
		{
			TicTacToeBoardSection ticTacToeBoardSection = _gameboard[gameboardIndex];
			if (playerName == gameState.Player1Name)
			{
				ticTacToeBoardSection.GameboardSpriteRenderer.sprite = Player1Token;
				ticTacToeBoardSection.GameboardSpriteRenderer.color = Color.yellow;
				ticTacToeBoardSection.MultiDisplayScoringPanelSpriteRenderer.sprite = Player1Token;
				ticTacToeBoardSection.MultiDisplayScoringPanelSpriteRenderer.color = Color.yellow;
			}
			else
			{
				ticTacToeBoardSection.GameboardSpriteRenderer.sprite = Player2Token;
				ticTacToeBoardSection.GameboardSpriteRenderer.color = Color.red;
				ticTacToeBoardSection.MultiDisplayScoringPanelSpriteRenderer.sprite = Player2Token;
				ticTacToeBoardSection.MultiDisplayScoringPanelSpriteRenderer.color = Color.red;
			}
		}

		private IEnumerator AnimateWinningTokens(List<int> winningIndexes)
		{
			TicTacToeBoardSection token = _gameboard[winningIndexes[0]];
			TicTacToeBoardSection token2 = _gameboard[winningIndexes[1]];
			TicTacToeBoardSection token3 = _gameboard[winningIndexes[2]];
			float currentTime = 0f;
			float duration = 0.1f;
			while (currentTime < duration)
			{
				token.GameboardSpriteRenderer.color = Color.Lerp(token.GameboardSpriteRenderer.color, Color.white, currentTime / duration);
				token2.GameboardSpriteRenderer.color = Color.Lerp(token2.GameboardSpriteRenderer.color, Color.white, currentTime / duration);
				token3.GameboardSpriteRenderer.color = Color.Lerp(token3.GameboardSpriteRenderer.color, Color.white, currentTime / duration);
				token.MultiDisplayScoringPanelSpriteRenderer.color = Color.Lerp(token.MultiDisplayScoringPanelSpriteRenderer.color, Color.white, currentTime / duration);
				token2.MultiDisplayScoringPanelSpriteRenderer.color = Color.Lerp(token2.MultiDisplayScoringPanelSpriteRenderer.color, Color.white, currentTime / duration);
				token3.MultiDisplayScoringPanelSpriteRenderer.color = Color.Lerp(token3.MultiDisplayScoringPanelSpriteRenderer.color, Color.white, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
			currentTime = 0f;
			duration = 0.2f;
			while (currentTime < duration)
			{
				token.GameboardSpriteRenderer.color = Color.Lerp(token.GameboardSpriteRenderer.color, Color.green, currentTime / duration);
				token2.GameboardSpriteRenderer.color = Color.Lerp(token2.GameboardSpriteRenderer.color, Color.green, currentTime / duration);
				token3.GameboardSpriteRenderer.color = Color.Lerp(token3.GameboardSpriteRenderer.color, Color.green, currentTime / duration);
				token.MultiDisplayScoringPanelSpriteRenderer.color = Color.Lerp(token.MultiDisplayScoringPanelSpriteRenderer.color, Color.green, currentTime / duration);
				token2.MultiDisplayScoringPanelSpriteRenderer.color = Color.Lerp(token2.MultiDisplayScoringPanelSpriteRenderer.color, Color.green, currentTime / duration);
				token3.MultiDisplayScoringPanelSpriteRenderer.color = Color.Lerp(token3.MultiDisplayScoringPanelSpriteRenderer.color, Color.green, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
			token.GameboardSpriteRenderer.color = Color.green;
			token2.GameboardSpriteRenderer.color = Color.green;
			token3.GameboardSpriteRenderer.color = Color.green;
			token.MultiDisplayScoringPanelSpriteRenderer.color = Color.green;
			token2.MultiDisplayScoringPanelSpriteRenderer.color = Color.green;
			token3.MultiDisplayScoringPanelSpriteRenderer.color = Color.green;
		}

		private void HandleUndo()
		{
			if (_playerLocationHistory.Count == 0 || gameState.IsTargetDisabled)
			{
				return;
			}
			_playerLocations = _playerLocationHistory.Pop();
			for (int i = 0; i < _gameboard.Count; i++)
			{
				if (!_playerLocations.ContainsKey(i))
				{
					_gameboard[i].GameboardSpriteRenderer.sprite = null;
					_gameboard[i].MultiDisplayScoringPanelSpriteRenderer.sprite = null;
				}
			}
			foreach (KeyValuePair<int, string> playerLocation in _playerLocations)
			{
				SetIcon(playerLocation.Key, playerLocation.Value);
			}
		}
	}
}
