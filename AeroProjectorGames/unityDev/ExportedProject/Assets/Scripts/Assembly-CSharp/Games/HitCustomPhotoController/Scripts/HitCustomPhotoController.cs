using System.Collections;
using System.Collections.Generic;
using Extensions;
using Games.CustomComponents;
using Games.GameState;
using Games.HitCustomPhotoController.ScriptableObjects;
using Games.Hit_Custom_Photo.Scriptable_Objects;
using Games.SharedScoringLogic.Standard;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using Settings;
using Timer;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Games.HitCustomPhotoController.Scripts
{
	public class HitCustomPhotoController : MonoBehaviour
	{
		private const int TARGET_ONE_POINT_SCORE = 1;

		private const int TARGET_THREE_POINT_SCORE = 3;

		private const int TARGET_SEVEN_POINT_SCORE = 7;

		private const int TARGET_TEN_POINT_SCORE = 10;

		private readonly Vector2 TARGET_ONE_POINT_SCALE = new Vector2(0.3f, 0.3f);

		private readonly Vector2 TARGET_THREE_POINT_SCALE = new Vector2(0.25f, 0.25f);

		private readonly Vector2 TARGET_SEVEN_POINT_SCALE = new Vector2(0.2f, 0.2f);

		private readonly Vector2 TARGET_TEN_POINT_SCALE = new Vector2(0.15f, 0.15f);

		[Header("Game Groups")]
		[SerializeField]
		private GameObject checkGameSessionGroup;

		[SerializeField]
		private GameObject getUserImagesGroup;

		[SerializeField]
		private GameObject gameSpeedMenuGroup;

		[SerializeField]
		private GameObject mainGameGroup;

		[Header("Multi Display Scoring")]
		[SerializeField]
		private HitCustomPhotoMultiDisplayEventsSO hitCustomPhotoMultiDisplayEvents;

		[SerializeField]
		private GameObject multiDisplayScoringPanel;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		[SerializeField]
		private HitCustomPhotoGameSessionSO hitCustomPhotoGameSession;

		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Scoring Strategy")]
		[SerializeField]
		private StandardRoundTwoEventCallsBasedScoringLogic scoringLogic;

		[Header("Target Objects")]
		[SerializeField]
		private Transform scoringTargetParentTransform;

		[SerializeField]
		private GameObject scoringTargetPrefab;

		[SerializeField]
		private Transform targetParentTransform;

		[SerializeField]
		private GameObject targetPrefab;

		[Header("Target Count")]
		[SerializeField]
		private int targetOnePointCount = 2;

		[SerializeField]
		private int targetThreePointCount = 2;

		[SerializeField]
		private int targetSevenPointCount = 3;

		[SerializeField]
		private int targetTenPointCount = 3;

		[Header("Target Colors")]
		[SerializeField]
		private TargetColorsHolder targetColorsHolder;

		[Header("Target Positions")]
		[SerializeField]
		private TargetPlacementSelection targetPlacementSelection;

		[Header("Timer State")]
		[SerializeField]
		private TimerStateSO timerState;

		private IEnumerator _handleNewPlayerEnumerator;

		private IEnumerator _showHideTargetsEnumerator;

		private Dictionary<string, Sprite> _playerTargetImageList = new Dictionary<string, Sprite>();

		private Dictionary<string, List<bool>> _playerVisibleTargetList;

		private Stack<Dictionary<string, List<bool>>> _playerVisibleTargetHistory = new Stack<Dictionary<string, List<bool>>>();

		private List<GameObject> _targetPlacementList = new List<GameObject>();

		private void OnEnable()
		{
			gameEvents.OnNewGame += CheckGameSession;
			gameEvents.OnGameOver += gameState.DisableTarget;
			gameEvents.OnUndo += HandleUndo;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			hitCustomPhotoEvents.OnChangeGameState += ChangeGameState;
			hitCustomPhotoEvents.OnEndPlayerTurn += HandleEndPlayerTurn;
			hitCustomPhotoEvents.OnHitAnimationFinished += TargetHitAnimationFinished;
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayScoringPanel);
			}
		}

		protected void OnDisable()
		{
			gameEvents.OnNewGame -= CheckGameSession;
			gameEvents.OnGameOver -= gameState.DisableTarget;
			gameEvents.OnUndo -= HandleUndo;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			hitCustomPhotoEvents.OnChangeGameState -= ChangeGameState;
			hitCustomPhotoEvents.OnEndPlayerTurn -= HandleEndPlayerTurn;
			hitCustomPhotoEvents.OnHitAnimationFinished -= TargetHitAnimationFinished;
			gameState.GameDifficulty = GameDifficulties.Easy;
			gameState.GameType = string.Empty;
			if (_handleNewPlayerEnumerator != null)
			{
				StopCoroutine(_handleNewPlayerEnumerator);
			}
			if (timerState.CurrentState == TimerStateEnum.Disabled || timerState.CurrentState == TimerStateEnum.Stopped)
			{
				hitCustomPhotoEvents.RaiseEndGameSession();
			}
		}

		private void Start()
		{
			CheckGameSession();
		}

		private void AddScoreSpecificProperties(int targetScore, GameObject targetReference)
		{
			Image component = targetReference.transform.GetChild(0).GetComponent<Image>();
			ScoredButton component2 = targetReference.GetComponent<ScoredButton>();
			RectTransform component3 = targetReference.GetComponent<RectTransform>();
			switch (targetScore)
			{
			case 1:
				component.color = targetColorsHolder.YellowColor;
				component2.Score = 1;
				component3.localScale = TARGET_ONE_POINT_SCALE;
				break;
			case 3:
				component.color = targetColorsHolder.PinkColor;
				component2.Score = 3;
				component3.localScale = TARGET_THREE_POINT_SCALE;
				break;
			case 7:
				component.color = targetColorsHolder.BlueColor;
				component2.Score = 7;
				component3.localScale = TARGET_SEVEN_POINT_SCALE;
				break;
			case 10:
				component.color = targetColorsHolder.GreenColor;
				component2.Score = 10;
				component3.localScale = TARGET_TEN_POINT_SCALE;
				break;
			default:
				Debug.Log("NO TARGET SPECIFIC ELEMENTS WERE CHANGED");
				break;
			}
		}

		private void AddTargetVisibilityHistory()
		{
			_playerVisibleTargetHistory.Push(_playerVisibleTargetList.SimpleJsonClone());
		}

		private void ChangeGameState(HitCustomPhotoGameStates newgameState)
		{
			if (_showHideTargetsEnumerator != null)
			{
				StopCoroutine(_showHideTargetsEnumerator);
			}
			checkGameSessionGroup.SetActive(value: false);
			getUserImagesGroup.SetActive(value: false);
			gameSpeedMenuGroup.SetActive(value: false);
			mainGameGroup.SetActive(value: false);
			switch (newgameState)
			{
			case HitCustomPhotoGameStates.CheckExistingSession:
				gameEvents.RaiseAddToGameFlexSpace(Object.Instantiate(checkGameSessionGroup));
				break;
			case HitCustomPhotoGameStates.Playing:
				mainGameGroup.SetActive(value: true);
				HandleStartGame();
				break;
			case HitCustomPhotoGameStates.SpeedSelection:
				gameEvents.RaiseAddToGameFlexSpace(Object.Instantiate(gameSpeedMenuGroup));
				break;
			default:
				gameEvents.RaiseAddToGameFlexSpace(Object.Instantiate(getUserImagesGroup));
				break;
			}
			hitCustomPhotoMultiDisplayEvents.RaiseMultiDisplayStateChangeRequest(newgameState);
		}

		private void CheckGameSession()
		{
			scoreboardLoader.RaiseUnloadScoreboardRequest();
			ChangeGameState(HitCustomPhotoGameStates.CheckExistingSession);
		}

		private void CreateAndPlaceTargets(int targetScore, int targetCount)
		{
			for (int i = 0; i < targetCount; i++)
			{
				GameObject gameObject = Object.Instantiate(targetPrefab, targetParentTransform);
				GameObject gameObject2 = Object.Instantiate(scoringTargetPrefab, scoringTargetParentTransform);
				gameObject.GetComponent<TargetController>().Init(gameObject2);
				gameObject2.GetComponent<ScoringTargetController>().Init(gameObject);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				component.anchoredPosition = Vector3.zero;
				component.localPosition = Vector3.zero;
				AddScoreSpecificProperties(targetScore, gameObject);
				AddScoreSpecificProperties(targetScore, gameObject2);
				gameObject.SetActive(value: false);
				gameObject2.SetActive(value: false);
				_targetPlacementList.Add(gameObject);
			}
		}

		private void CreatePlayerBoards()
		{
			_playerVisibleTargetList = new Dictionary<string, List<bool>>();
			_playerTargetImageList = new Dictionary<string, Sprite>();
			GetTexture();
		}

		private void DestroyPlacedTargets()
		{
			foreach (GameObject targetPlacement in _targetPlacementList)
			{
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					Object.DestroyImmediate(targetPlacement.GetComponent<TargetController>().ScoringTarget);
				}
				Object.DestroyImmediate(targetPlacement);
			}
			_targetPlacementList.Clear();
		}

		private void GetTexture()
		{
			List<bool> list = new List<bool>();
			for (int i = 0; i < _targetPlacementList.Count; i++)
			{
				list.Add(item: true);
			}
			for (int j = 0; j < playerState.players.Count; j++)
			{
				_playerVisibleTargetList.Add(playerState.players[j].PlayerName, new List<bool>(list));
				Texture2D playerTextureByIndex = hitCustomPhotoGameSession.GetPlayerTextureByIndex(j);
				Sprite value = Sprite.Create(playerTextureByIndex, new Rect(0f, 0f, playerTextureByIndex.width, playerTextureByIndex.height), default(Vector2));
				_playerTargetImageList.Add(playerState.players[j].PlayerName, value);
			}
			_showHideTargetsEnumerator = ShowHideTargets();
			StartCoroutine(_showHideTargetsEnumerator);
			gameState.EnableTarget();
		}

		private void HandleEndPlayerTurn()
		{
			_handleNewPlayerEnumerator = HandleNextPlayer();
			StartCoroutine(_handleNewPlayerEnumerator);
		}

		private void HandleMiss()
		{
			StartCoroutine(HandleMissEnumerator());
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			if (!gameState.IsTargetDisabled)
			{
				StartCoroutine(MissDetectedRoutine(pointerEventData, screenPoint));
			}
		}

		private IEnumerator HandleMissEnumerator()
		{
			gameState.DisableTarget();
			scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, "Miss!", ScoreboardMessageStyle.Normal));
			yield return new WaitForSeconds(1f);
			yield return HandleScoring(0);
			scoringLogic.HandleUpdateNextFrame();
			gameState.EnableTarget();
		}

		private IEnumerator HandleNextPlayer(Transform TargetTransform = null)
		{
			_showHideTargetsEnumerator = ShowHideTargets(showTargets: false);
			StartCoroutine(_showHideTargetsEnumerator);
			if (TargetTransform != null)
			{
				TargetTransform.gameObject.GetComponent<Image>().enabled = true;
				TargetTransform.GetChild(0).gameObject.GetComponent<Image>().enabled = true;
			}
			if (gameState.GameStatus != GameStatus.Finished)
			{
				_showHideTargetsEnumerator = ShowHideTargets();
				StartCoroutine(_showHideTargetsEnumerator);
			}
			yield break;
		}

		private IEnumerator HandleScoring(int roundScore)
		{
			gameState.EnableTarget();
			scoringLogic.RecordScore(new StandardScoreModel(roundScore));
			gameState.DisableTarget();
			yield return new WaitForSeconds(1f);
		}

		private void HandleStartGame()
		{
			gameEvents.RaiseRemoveAllFromGameFlexSpace();
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
			_playerVisibleTargetHistory.Clear();
			DestroyPlacedTargets();
			CreateAndPlaceTargets(1, targetOnePointCount);
			CreateAndPlaceTargets(3, targetThreePointCount);
			CreateAndPlaceTargets(7, targetSevenPointCount);
			CreateAndPlaceTargets(10, targetTenPointCount);
			CreatePlayerBoards();
		}

		private void HandleUndo()
		{
			if (_playerVisibleTargetHistory.Count != 0 && !gameState.IsTargetDisabled)
			{
				Dictionary<string, List<bool>> playerVisibleTargetList = _playerVisibleTargetHistory.Pop();
				_playerVisibleTargetList = playerVisibleTargetList;
				_showHideTargetsEnumerator = ShowHideTargets();
				StartCoroutine(_showHideTargetsEnumerator);
			}
		}

		private IEnumerator HandleTargetHitAnimationFinished(Transform TargetTransform, int roundScore)
		{
			yield return HandleScoring(roundScore);
			scoringLogic.HandleUpdateNextFrame();
			gameState.EnableTarget();
		}

		private IEnumerator MissDetectedRoutine(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			gameState.DisableTarget();
			if (screenPoint.HasValue)
			{
				hitEffectEvents.RaiseHitEffect(screenPoint.Value);
			}
			HandleMiss();
			yield return null;
		}

		private IEnumerator ShowHideTargets(bool showTargets = true)
		{
			yield return null;
			PositionOptionTransforms targetPositions = targetPlacementSelection.GetTargetPositions();
			for (int i = 0; i < _targetPlacementList.Count; i++)
			{
				if (i < _playerVisibleTargetList[gameState.CurrentPlayer].Count && showTargets)
				{
					_targetPlacementList[i].transform.position = targetPositions.TargetPositions[i].position;
					_targetPlacementList[i].SetActive(_playerVisibleTargetList[gameState.CurrentPlayer][i]);
					_targetPlacementList[i].GetComponent<Image>().sprite = _playerTargetImageList[gameState.CurrentPlayer];
					if (SettingsStore.Interaction.MultiDisplayEnabled)
					{
						_targetPlacementList[i].GetComponent<TargetController>().ScoringTarget.GetComponent<Image>().sprite = _playerTargetImageList[gameState.CurrentPlayer];
					}
				}
				else
				{
					_targetPlacementList[i].SetActive(value: false);
				}
			}
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayScoringPanel.transform.localRotation = mainGameGroup.transform.localRotation;
			}
		}

		public void TargetHitAnimationFinished(ScoredButton scoredButton)
		{
			AddTargetVisibilityHistory();
			GameObject gameObject = scoredButton.gameObject;
			int num = _targetPlacementList.IndexOf(gameObject);
			if (num != -1 && num < _targetPlacementList.Count && num < _playerVisibleTargetList[gameState.CurrentPlayer].Count)
			{
				_playerVisibleTargetList[gameState.CurrentPlayer][num] = false;
				StartCoroutine(HandleTargetHitAnimationFinished(gameObject.transform, scoredButton.Score));
			}
		}
	}
}
