using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using Games.SharedScoringLogic.UnscoredTwoPlayer;
using HitEffects;
using Scoreboard;
using Scoreboard.Messaging;
using Scoreboard.PrefabScripts;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Lineup_4
{
	public class Lineup4GameController : MonoBehaviour
	{
		private Dictionary<string, List<int>> _columnIndexes = new Dictionary<string, List<int>>
		{
			{
				"Column1",
				new List<int> { 20, 15, 10, 5, 0 }
			},
			{
				"Column2",
				new List<int> { 21, 16, 11, 6, 1 }
			},
			{
				"Column3",
				new List<int> { 22, 17, 12, 7, 2 }
			},
			{
				"Column4",
				new List<int> { 23, 18, 13, 8, 3 }
			},
			{
				"Column5",
				new List<int> { 24, 19, 14, 9, 4 }
			}
		};

		private List<GameObject> _cells = new List<GameObject>(25);

		private List<GameObject> _multiDisplayCells = new List<GameObject>(25);

		private bool _missRoutineRunning;

		private Dictionary<string, List<int>> _playerScores;

		private Stack<Dictionary<int, bool>> _previousCellState = new Stack<Dictionary<int, bool>>();

		private ScoreboardController _scoreboardController;

		private List<int[]> _winningCombinations = new List<int[]>
		{
			new int[4] { 0, 1, 2, 3 },
			new int[4] { 1, 2, 3, 4 },
			new int[4] { 5, 6, 7, 8 },
			new int[4] { 6, 7, 8, 9 },
			new int[4] { 10, 11, 12, 13 },
			new int[4] { 11, 12, 13, 14 },
			new int[4] { 15, 16, 17, 18 },
			new int[4] { 16, 17, 18, 19 },
			new int[4] { 20, 21, 22, 23 },
			new int[4] { 21, 22, 23, 24 },
			new int[4] { 0, 5, 10, 15 },
			new int[4] { 5, 10, 15, 20 },
			new int[4] { 1, 6, 11, 16 },
			new int[4] { 6, 11, 16, 21 },
			new int[4] { 2, 7, 12, 17 },
			new int[4] { 7, 12, 17, 22 },
			new int[4] { 3, 8, 13, 18 },
			new int[4] { 8, 13, 18, 23 },
			new int[4] { 4, 9, 14, 19 },
			new int[4] { 9, 14, 19, 24 },
			new int[4] { 0, 6, 12, 18 },
			new int[4] { 6, 12, 18, 24 },
			new int[4] { 1, 7, 13, 19 },
			new int[4] { 5, 11, 17, 23 },
			new int[4] { 4, 8, 12, 16 },
			new int[4] { 8, 12, 16, 20 },
			new int[4] { 3, 7, 11, 15 },
			new int[4] { 9, 13, 17, 21 }
		};

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

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

		[Header("Multi Display Settings")]
		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[SerializeField]
		private GameObject multiDisplayScoringPanel;

		private void OnEnable()
		{
			gameState.NumberOfRounds = NumberOfRounds.Infinite;
			gameState.Player1Icon = Player1Token;
			gameState.Player2Icon = Player2Token;
			gameEvents.OnGameObjectClicked += Score;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			gameEvents.OnUndo += HandleUndo;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayScoringPanel);
			}
			else
			{
				multiDisplayScoringPanel.SetActive(value: false);
			}
		}

		private void OnDisable()
		{
			gameEvents.OnGameObjectClicked -= Score;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			gameEvents.OnUndo -= HandleUndo;
		}

		private void Start()
		{
			HandleNewGame();
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.UnscoredTwoPlayer);
		}

		private void HandleMiss()
		{
			scoringStrategy.RecordScore(null);
			SaveGameState();
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

		private void SaveGameState()
		{
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			for (int i = 0; i < _cells.Count; i++)
			{
				GameObject gameObject = _cells[i];
				dictionary.Add(i, gameObject != null);
			}
			_previousCellState.Push(dictionary);
		}

		private void Score(GameObject column)
		{
			SaveGameState();
			for (int i = 0; i < _columnIndexes[column.name].Count; i++)
			{
				int num = _columnIndexes[column.name][i];
				if (_cells[num] == null)
				{
					_cells[num] = column;
					_playerScores[gameState.CurrentPlayer].Add(num);
					break;
				}
			}
			List<int> list = CheckForWin();
			if (list.Count > 0)
			{
				foreach (int item in list)
				{
					_cells[item].GetComponent<SpriteRenderer>().color = Color.green;
					if (SettingsStore.Interaction.MultiDisplayEnabled)
					{
						_multiDisplayCells[item].GetComponent<SpriteRenderer>().color = Color.green;
					}
				}
				gameEvents.RaiseGameOver();
				gameEvents.RaiseWinAnimation();
			}
			else
			{
				scoringStrategy.RecordScore(null);
			}
		}

		public void RegisterMultiDisplayTokenInColumn(GameObject column)
		{
			for (int i = 0; i < _columnIndexes[column.name].Count; i++)
			{
				int index = _columnIndexes[column.name][i];
				if (_multiDisplayCells[index] == null)
				{
					_multiDisplayCells[index] = column;
					break;
				}
			}
		}

		private List<int> CheckForWin()
		{
			List<int> list = new List<int>();
			foreach (int[] winningCombination in _winningCombinations)
			{
				int[] array = winningCombination;
				foreach (int item in array)
				{
					list.Add(item);
					if (!_playerScores[gameState.CurrentPlayer].Contains(item))
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

		private void HandleNewGame()
		{
			foreach (GameObject cell in _cells)
			{
				Object.Destroy(cell);
			}
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				_multiDisplayCells.ForEach(Object.Destroy);
			}
			for (int i = 0; i < 25; i++)
			{
				_cells.Add(null);
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					_multiDisplayCells.Add(null);
				}
			}
			_playerScores = new Dictionary<string, List<int>>
			{
				{
					gameState.Player1Name,
					new List<int>()
				},
				{
					gameState.Player2Name,
					new List<int>()
				}
			};
		}

		private void HandleUndo()
		{
			if (_previousCellState.Count == 0 || gameState.IsTargetDisabled)
			{
				return;
			}
			Dictionary<int, bool> dictionary = _previousCellState.Pop();
			for (int i = 0; i < dictionary.Count; i++)
			{
				if (!dictionary[i] && _cells[i] != null)
				{
					Object.DestroyImmediate(_cells[i]);
					_cells[i] = null;
					if (SettingsStore.Interaction.MultiDisplayEnabled && _multiDisplayCells[i] != null)
					{
						Object.DestroyImmediate(_multiDisplayCells[i]);
						_multiDisplayCells[i] = null;
					}
					if (_playerScores[gameState.Player1Name].Contains(i))
					{
						_playerScores[gameState.Player1Name].Remove(i);
					}
					if (_playerScores[gameState.Player2Name].Contains(i))
					{
						_playerScores[gameState.Player2Name].Remove(i);
					}
				}
			}
		}
	}
}
