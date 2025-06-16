using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using Games.SharedScoringLogic.Standard;
using Games.Standard_Target.SO;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using Scoreboard.PrefabScripts;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Standard_Target
{
	public class StandardTargetGameController : MonoBehaviour
	{
		private const int KILLZONE_FRAMES_1 = 4;

		private const int KILLZONE_FRAMES_2 = 9;

		private IEnumerator _missDetectedRoutineEnumerator;

		private IEnumerator _startGameRoutineEnumerator;

		private bool _killZones;

		private bool _missRoutineRunning;

		private bool _playerChoosesTargetColor;

		private bool _rotateTargets;

		private ScoreboardController _scoreboardController;

		private bool _useSixRingTarget;

		private List<GameObject>.Enumerator _currentTarget;

		private readonly Dictionary<int, GameObject> _frameTargetMap = new Dictionary<int, GameObject>();

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[SerializeField]
		private StandardTargetGameEventsSO standardTargetGameEvents;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayScoringObjectEvents;

		[SerializeField]
		private StandardTargetMultiDisplayScoringController multiDisplayScoringPanel;

		[Header("Scoring Strategy")]
		[SerializeField]
		private StandardRoundBasedScoringLogic scoringLogic;

		[Header("Killzone Container")]
		[SerializeField]
		private GameObject killZones;

		[SerializeField]
		private GameObject sixRingKillZones;

		[Header("Target Color Elements")]
		[SerializeField]
		private GameObject multiDisplayTargetColorMenu;

		[SerializeField]
		private GameObject targetColorMenu;

		[SerializeField]
		private StandardTargetColorController targetColorController;

		[Header("Target 5 Rings Groups")]
		[SerializeField]
		private GameObject bottomLeftRingLocation;

		[SerializeField]
		private GameObject bottomRightRingLocation;

		[SerializeField]
		private GameObject centerRingLocation;

		[SerializeField]
		private GameObject topLeftRingLocation;

		[SerializeField]
		private GameObject topRightRingLocation;

		[Header("Target 6 Rings Groups")]
		[SerializeField]
		private GameObject sixRingBottomLeftRingLocation;

		[SerializeField]
		private GameObject sixRingBottomRightRingLocation;

		[SerializeField]
		private GameObject sixRingCenterRingLocation;

		[SerializeField]
		private GameObject sixRingTopLeftRingLocation;

		[SerializeField]
		private GameObject sixRingTopRightRingLocation;

		private void OnEnable()
		{
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			_killZones = SettingsStore.Target.ShowKillZones;
			_rotateTargets = SettingsStore.Target.RotateTargets;
			_useSixRingTarget = SettingsStore.Target.UseSixRingTarget;
			_playerChoosesTargetColor = targetColorController.CheckIfPlayerChoosesTargetColor();
			HandleNewGame();
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnNewRound += SetActiveTarget;
			gameEvents.OnScoreChange += HandleScoreChange;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			scoringLogic.OnUndoComplete += SetActiveTarget;
			scoringLogic.OnFrameUpdateComplete += HandleOnFrameChange;
			standardTargetGameEvents.OnStartGameplay += HandleStartGame;
		}

		protected void OnDisable()
		{
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnNewRound -= SetActiveTarget;
			gameEvents.OnScoreChange -= HandleScoreChange;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			scoringLogic.OnUndoComplete -= SetActiveTarget;
			scoringLogic.OnFrameUpdateComplete -= HandleOnFrameChange;
			standardTargetGameEvents.OnStartGameplay -= HandleStartGame;
			DisableAllEnumerators();
		}

		private void HandleOnFrameChange()
		{
			SetActiveTarget();
		}

		private void InitializeGame()
		{
			if (_playerChoosesTargetColor)
			{
				gameEvents.RaiseAddToGameFlexSpace(Object.Instantiate(targetColorMenu));
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayTargetColorMenu.SetActive(value: true);
					multiDisplayScoringObjectEvents.RaiseLoadScoringObject(multiDisplayTargetColorMenu);
				}
			}
			else
			{
				HandleStartGame();
			}
		}

		private void CreateFrameTargetMap()
		{
			if (_frameTargetMap.Count == 0)
			{
				_frameTargetMap.Add(0, _useSixRingTarget ? sixRingCenterRingLocation : centerRingLocation);
				_frameTargetMap.Add(1, _useSixRingTarget ? sixRingTopLeftRingLocation : topLeftRingLocation);
				_frameTargetMap.Add(2, _useSixRingTarget ? sixRingBottomRightRingLocation : bottomRightRingLocation);
				_frameTargetMap.Add(3, _useSixRingTarget ? sixRingTopRightRingLocation : topRightRingLocation);
				if (_useSixRingTarget)
				{
					_frameTargetMap.Add(4, _killZones ? sixRingCenterRingLocation : sixRingBottomLeftRingLocation);
					_frameTargetMap.Add(5, _killZones ? sixRingTopLeftRingLocation : sixRingCenterRingLocation);
					_frameTargetMap.Add(6, _killZones ? sixRingBottomRightRingLocation : sixRingTopLeftRingLocation);
					_frameTargetMap.Add(7, _killZones ? sixRingTopRightRingLocation : sixRingBottomRightRingLocation);
					_frameTargetMap.Add(8, _killZones ? sixRingBottomLeftRingLocation : sixRingCenterRingLocation);
					_frameTargetMap.Add(9, _killZones ? sixRingCenterRingLocation : sixRingTopLeftRingLocation);
				}
				else
				{
					_frameTargetMap.Add(4, _killZones ? centerRingLocation : bottomLeftRingLocation);
					_frameTargetMap.Add(5, _killZones ? topLeftRingLocation : centerRingLocation);
					_frameTargetMap.Add(6, _killZones ? bottomRightRingLocation : topLeftRingLocation);
					_frameTargetMap.Add(7, _killZones ? topRightRingLocation : bottomRightRingLocation);
					_frameTargetMap.Add(8, _killZones ? bottomLeftRingLocation : centerRingLocation);
					_frameTargetMap.Add(9, _killZones ? centerRingLocation : topLeftRingLocation);
				}
			}
		}

		private void DisableAllEnumerators()
		{
			if (_missDetectedRoutineEnumerator != null)
			{
				StopCoroutine(_missDetectedRoutineEnumerator);
			}
			if (_startGameRoutineEnumerator != null)
			{
				StopCoroutine(_startGameRoutineEnumerator);
			}
		}

		private void HandleMiss()
		{
			HandleScoreChange(null);
		}

		private void HandleMissDetected(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			_missDetectedRoutineEnumerator = MissDetectedRoutine(pointerEventData, screenPoint);
			StartCoroutine(_missDetectedRoutineEnumerator);
		}

		private void HandleNewGame()
		{
			HideAllGameElements();
			gameState.CurrentFrame = 0;
			gameState.GameStatus = GameStatus.InProgress;
			InitializeGame();
		}

		private void HandleScoreChange(int? score)
		{
			scoringLogic.RecordScore(new StandardScoreModel(score.GetValueOrDefault()));
		}

		private void HandleStartGame()
		{
			_startGameRoutineEnumerator = StartGameRoutine();
			StartCoroutine(_startGameRoutineEnumerator);
		}

		private void HideAllGameElements()
		{
			HideAllTarget();
			multiDisplayScoringPanel.gameObject.SetActive(value: false);
			multiDisplayTargetColorMenu.SetActive(value: false);
			targetColorMenu.SetActive(value: false);
			UnloadAllScoreboardObjects();
		}

		private void HideAllTarget()
		{
			bottomLeftRingLocation.SetActive(value: false);
			bottomRightRingLocation.SetActive(value: false);
			centerRingLocation.SetActive(value: false);
			topLeftRingLocation.SetActive(value: false);
			topRightRingLocation.SetActive(value: false);
			sixRingBottomLeftRingLocation.SetActive(value: false);
			sixRingBottomRightRingLocation.SetActive(value: false);
			sixRingCenterRingLocation.SetActive(value: false);
			sixRingTopLeftRingLocation.SetActive(value: false);
			sixRingTopRightRingLocation.SetActive(value: false);
		}

		private bool IsKillZoneRound()
		{
			int currentFrame = gameState.CurrentFrame;
			bool flag = _killZones;
			switch (currentFrame)
			{
			case 9:
				if (flag)
				{
					return true;
				}
				break;
			case 4:
				if (flag)
				{
					return true;
				}
				break;
			}
			return false;
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

		private void SetActiveTarget()
		{
			if (gameState.GameStatus == GameStatus.Finished || gameState.IsTargetDisabled)
			{
				return;
			}
			if (_rotateTargets)
			{
				foreach (KeyValuePair<int, GameObject> item in _frameTargetMap)
				{
					item.Value.SetActive(value: false);
				}
				_frameTargetMap[gameState.CurrentFrame].SetActive(value: true);
			}
			bool flag = IsKillZoneRound();
			if (_useSixRingTarget)
			{
				sixRingKillZones.SetActive(flag);
			}
			else
			{
				killZones.SetActive(flag);
			}
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayScoringPanel.SetKillzoneActive(flag);
			}
		}

		private void SetStartingTarget()
		{
			HideAllTarget();
			if (_useSixRingTarget)
			{
				sixRingCenterRingLocation.SetActive(value: true);
			}
			else
			{
				centerRingLocation.SetActive(value: true);
			}
		}

		private IEnumerator StartGameRoutine()
		{
			HideAllGameElements();
			gameState.EnableTarget();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayScoringPanel.gameObject.SetActive(value: true);
				multiDisplayScoringObjectEvents.RaiseLoadScoringObject(multiDisplayScoringPanel.gameObject);
			}
			else
			{
				multiDisplayScoringPanel.gameObject.SetActive(value: false);
			}
			SetStartingTarget();
			CreateFrameTargetMap();
			SetActiveTarget();
			yield return null;
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
		}

		private void UnloadAllScoreboardObjects()
		{
			gameEvents.RaiseRemoveAllFromGameFlexSpace();
			scoreboardLoader.RaiseUnloadScoreboardRequest();
		}
	}
}
