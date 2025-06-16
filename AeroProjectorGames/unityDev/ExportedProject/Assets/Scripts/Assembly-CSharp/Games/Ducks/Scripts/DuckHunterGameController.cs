using System.Collections;
using System.Collections.Generic;
using Extensions;
using Games.GameState;
using Games.SharedScoringLogic.Standard;
using Games.Zombies.Scripts;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using Scoreboard.PrefabScripts;
using Settings;
using UI;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Ducks.Scripts
{
	public class DuckHunterGameController : MonoBehaviour
	{
		private const int SHORT_CIRCUIT_LOOP_INTERATIONS = 500;

		private const int MAX_COL_NUMBER = 5;

		private const int MAX_ROW_NUMBER = 5;

		private const int MIN_COL_NUMBER = 1;

		private const int MIN_ROW_NUMBER = 1;

		private const string TARGET_NAME = "Duck Target";

		private const int TARGET_TO_HIT_SCORE = 1;

		private const int TARGET_TO_MISS_SCORE = -2;

		private Vector2 _easyTargetSizeVector = new Vector2(28f, 28f);

		private Vector2 _hardTargetSizeVector = new Vector2(20f, 20f);

		private ScoreboardController _scoreboardController;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[SerializeField]
		private DuckHunterMultiDisplayScoringController multiDisplayController;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayScoringEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Scoring Strategy")]
		[SerializeField]
		private StandardRoundBasedScoringLogic scoringLogic;

		[Header("Max Objects")]
		[SerializeField]
		private int maxTargetsPerRow = 2;

		[SerializeField]
		private int maxTargetsPerCol = 3;

		[Header("Board Matrix")]
		[SerializeField]
		private GameObject[] boardLayoutRow1;

		[SerializeField]
		private GameObject[] boardLayoutRow2;

		[SerializeField]
		private GameObject[] boardLayoutRow3;

		[SerializeField]
		private GameObject[] boardLayoutRow4;

		[SerializeField]
		private GameObject[] boardLayoutRow5;

		private Dictionary<int, int> _colCount;

		private Dictionary<int, int> _rowCount;

		[Header("Target Objects")]
		[SerializeField]
		private GameObject duckFeatherAnimation;

		[SerializeField]
		private Sprite[] duckTargetsArray;

		[SerializeField]
		private Sprite[] duckTargetsToMissArray;

		[Header("External References")]
		private GameObject _featherAnimationObject;

		private Sprite[] targetsToHitArray;

		private Sprite[] targetsToMissArray;

		private Dictionary<string, List<bool>> _playerVisibleTargetList;

		private Stack<Dictionary<string, List<bool>>> _playerVisibleTargetHistory = new Stack<Dictionary<string, List<bool>>>();

		private List<GameObject> _targetPlacementList = new List<GameObject>();

		private bool _missRoutineRunning;

		private void OnEnable()
		{
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayScoringEvents.RaiseLoadScoringObject(multiDisplayController.gameObject);
			}
			gameEvents.OnNewGame += NewGame;
			gameEvents.OnGameOver += gameState.DisableTarget;
			gameEvents.OnUndo += HandleUndo;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			NewGame();
		}

		protected void OnDisable()
		{
			gameEvents.OnNewGame -= NewGame;
			gameEvents.OnGameOver -= gameState.DisableTarget;
			gameEvents.OnUndo -= HandleUndo;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			Object.Destroy(_featherAnimationObject);
			gameState.GameDifficulty = GameDifficulties.Easy;
			gameState.GameType = string.Empty;
		}

		private bool CreateAndPlaceTarget(GameObject gridPosition, Sprite[] targetSpriteArray, int arrayIndex, int targetPoints, int rowIndex, int colIndex)
		{
			if (gridPosition.transform.childCount == 0)
			{
				GameObject newTarget = new GameObject("Duck Target");
				newTarget.AddComponent<SpriteRenderer>().sprite = targetSpriteArray[arrayIndex];
				newTarget.name = targetSpriteArray[arrayIndex].name;
				newTarget.transform.SetParent(gridPosition.transform, worldPositionStays: false);
				newTarget.transform.localPosition = Vector3.zero;
				newTarget.transform.localScale = ((gameState.GameDifficulty == GameDifficulties.Easy) ? _easyTargetSizeVector : _hardTargetSizeVector);
				newTarget.AddComponent<PolygonCollider2D>();
				EventTrigger eventTrigger = newTarget.AddComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener(delegate
				{
					TargetClicked(targetPoints, newTarget);
				});
				eventTrigger.triggers.Add(entry);
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayController.AddTarget(newTarget, rowIndex, colIndex).AddComponent<OnActiveChangeController>().OnActiveChanged.AddListener(newTarget.SetActive);
				}
				_targetPlacementList.Add(newTarget);
				return true;
			}
			return false;
		}

		private void CreatePlayerBoards()
		{
			List<bool> list = new List<bool>();
			for (int i = 0; i < _targetPlacementList.Count; i++)
			{
				list.Add(item: true);
			}
			_playerVisibleTargetList = new Dictionary<string, List<bool>>();
			foreach (PlayerData player in playerState.players)
			{
				_playerVisibleTargetList.Add(player.PlayerName, new List<bool>(list));
			}
		}

		private void DestroyPlacedTargets()
		{
			Object.DestroyImmediate(_featherAnimationObject);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayController.Reset();
			}
			foreach (GameObject targetPlacement in _targetPlacementList)
			{
				EventTrigger component = targetPlacement.GetComponent<EventTrigger>();
				component.triggers.ForEach(delegate(EventTrigger.Entry entry)
				{
					entry.callback.RemoveAllListeners();
				});
				component.triggers.Clear();
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					targetPlacement.GetComponent<OnActiveChangeController>().OnActiveChanged.RemoveAllListeners();
				}
				Object.DestroyImmediate(targetPlacement);
			}
			_targetPlacementList.Clear();
		}

		private GameObject GetGridPosition(int rowNumber, int colNumber)
		{
			colNumber--;
			switch (rowNumber)
			{
			case 1:
				return boardLayoutRow1[colNumber];
			case 2:
				return boardLayoutRow2[colNumber];
			case 3:
				return boardLayoutRow3[colNumber];
			case 4:
				return boardLayoutRow4[colNumber];
			case 5:
				return boardLayoutRow5[colNumber];
			default:
				return new GameObject();
			}
		}

		private void GameTypeSettings()
		{
			targetsToHitArray = duckTargetsArray;
			targetsToMissArray = duckTargetsToMissArray;
			_featherAnimationObject = Object.Instantiate(duckFeatherAnimation, base.transform.parent.transform);
			_featherAnimationObject.SetActive(value: false);
			_featherAnimationObject.transform.localScale = ((gameState.GameDifficulty == GameDifficulties.Easy) ? new Vector2(40f, 40f) : new Vector2(25f, 25f));
		}

		private int GetRandomRowOrColumnNumber(int min, int max, Dictionary<int, int> countCheck, int itemMax = 99)
		{
			int num = 0;
			max++;
			int num2;
			do
			{
				num2 = Random.Range(min, max);
				num++;
				if (num != 1000)
				{
					continue;
				}
				for (int i = 1; i <= countCheck.Count; i++)
				{
					if (countCheck[i] < itemMax)
					{
						num2 = i;
						break;
					}
				}
			}
			while (countCheck[num2] >= itemMax && num < 1000);
			return num2;
		}

		private void GenerateTargets(Sprite[] targetSpriteArray, int targetScore)
		{
			for (int i = 0; i < targetSpriteArray.Length; i++)
			{
				bool flag = true;
				int num = 0;
				do
				{
					int randomRowOrColumnNumber = GetRandomRowOrColumnNumber(1, 5, _rowCount, maxTargetsPerRow);
					int randomRowOrColumnNumber2 = GetRandomRowOrColumnNumber(1, 5, _colCount, maxTargetsPerCol);
					GameObject gridPosition = GetGridPosition(randomRowOrColumnNumber, randomRowOrColumnNumber2);
					if (CreateAndPlaceTarget(gridPosition, targetSpriteArray, i, targetScore, randomRowOrColumnNumber, randomRowOrColumnNumber2))
					{
						_rowCount[randomRowOrColumnNumber]++;
						_colCount[randomRowOrColumnNumber2]++;
						flag = false;
					}
					num++;
				}
				while (flag || num >= 500);
			}
		}

		private void NewGame()
		{
			_playerVisibleTargetHistory.Clear();
			DestroyPlacedTargets();
			ResetRowAndColumnCounts();
			SetSettingsAndTargets();
			CreatePlayerBoards();
		}

		private void ResetRowAndColumnCounts()
		{
			_rowCount = new Dictionary<int, int>();
			_colCount = new Dictionary<int, int>();
			for (int i = 1; i <= 5; i++)
			{
				_rowCount.Add(i, 0);
				_colCount.Add(i, 0);
			}
		}

		private void SetSettingsAndTargets()
		{
			GameTypeSettings();
			GenerateTargets(targetsToHitArray, 1);
			GenerateTargets(targetsToMissArray, -2);
		}

		private void ShowHideTargets()
		{
			for (int i = 0; i < _targetPlacementList.Count; i++)
			{
				if (i < _playerVisibleTargetList[gameState.CurrentPlayer].Count)
				{
					_targetPlacementList[i].SetActive(_playerVisibleTargetList[gameState.CurrentPlayer][i]);
				}
			}
		}

		private void ShowHideTargets(bool isActive, GameObject hitTarget)
		{
			foreach (GameObject targetPlacement in _targetPlacementList)
			{
				if (targetPlacement != hitTarget)
				{
					targetPlacement.SetActive(isActive);
				}
			}
		}

		private void AddTargetVisibilityHistory()
		{
			_playerVisibleTargetHistory.Push(_playerVisibleTargetList.SimpleJsonClone());
		}

		private void HandleMiss()
		{
			scoringLogic.RecordScore(new StandardScoreModel());
			ShowHideTargets();
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

		private void HandleUndo()
		{
			if (_playerVisibleTargetHistory.Count != 0 && !gameState.IsTargetDisabled)
			{
				Dictionary<string, List<bool>> playerVisibleTargetList = _playerVisibleTargetHistory.Pop();
				_playerVisibleTargetList = playerVisibleTargetList;
				ShowHideTargets();
			}
		}

		public void TargetClicked(int score, GameObject spriteClicked)
		{
			if (!gameState.IsTargetDisabled)
			{
				AddTargetVisibilityHistory();
				int num = _targetPlacementList.IndexOf(spriteClicked);
				if (num != -1 && num < _targetPlacementList.Count && num < _playerVisibleTargetList[gameState.CurrentPlayer].Count)
				{
					StartCoroutine(HitAnimation(spriteClicked.transform, score));
					_playerVisibleTargetList[gameState.CurrentPlayer][num] = false;
				}
			}
		}

		private IEnumerator HitAnimation(Transform TargetTransform, int roundScore)
		{
			gameState.DisableTarget();
			Vector3 originalTransform = TargetTransform.localScale;
			Vector3 originalTransform2 = TargetTransform.localPosition;
			Quaternion originalTransform3 = TargetTransform.localRotation;
			Color originalColor = TargetTransform.gameObject.GetComponent<SpriteRenderer>().color;
			yield return LerpDuckAnimation(TargetTransform);
			TargetTransform.localScale = originalTransform;
			TargetTransform.localPosition = originalTransform2;
			TargetTransform.localRotation = originalTransform3;
			TargetTransform.gameObject.GetComponent<SpriteRenderer>().color = originalColor;
			gameState.EnableTarget();
			scoringLogic.RecordScore(new StandardScoreModel(roundScore));
			ShowHideTargets();
		}

		private IEnumerator LerpDuckAnimation(Transform TargetTransform)
		{
			ShowHideTargets(isActive: false, TargetTransform.gameObject);
			_featherAnimationObject.SetActive(value: true);
			_featherAnimationObject.transform.SetParent(TargetTransform.parent, worldPositionStays: false);
			_featherAnimationObject.transform.localPosition = TargetTransform.localPosition;
			_featherAnimationObject.GetComponent<ParticleSystem>().Play();
			float currentTime = 0f;
			float secondaryTime = 0f;
			float duration = 4f;
			Vector3 newPosition = TargetTransform.localPosition;
			newPosition += new Vector3(0f, -1000f, 0f);
			Vector3 newScale = TargetTransform.localScale;
			newScale += Vector3.one * 1.15f;
			_ = TargetTransform.eulerAngles;
			Vector3 newRotation = new Vector3(0f, 0f, 240f);
			float animationCulling = 3f;
			float secondaryAnimationDelayTime = 0.55f;
			while (currentTime < duration - animationCulling)
			{
				TargetTransform.localScale = Vector3.Lerp(TargetTransform.localScale, newScale, currentTime / duration);
				TargetTransform.eulerAngles = Vector3.Lerp(TargetTransform.eulerAngles, newRotation, currentTime / duration);
				if (currentTime > secondaryAnimationDelayTime)
				{
					TargetTransform.localPosition = Vector3.Lerp(TargetTransform.localPosition, newPosition, secondaryTime / duration);
					secondaryTime += Time.deltaTime;
				}
				currentTime += Time.deltaTime;
				yield return null;
			}
			_featherAnimationObject.GetComponent<ParticleSystem>().Stop();
			_featherAnimationObject.SetActive(value: false);
		}
	}
}
