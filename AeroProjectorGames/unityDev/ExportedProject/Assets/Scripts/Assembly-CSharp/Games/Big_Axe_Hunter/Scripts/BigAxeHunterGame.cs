using System.Collections;
using Games.GameState;
using Games.SecondCameraGameboard;
using Games.SharedScoringLogic.Standard;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Big_Axe_Hunter.Scripts
{
	public class BigAxeHunterGame : MonoBehaviour
	{
		[Header("Gameboard")]
		[SerializeField]
		private GameObject gameboard;

		[Header("Scriptable Objects")]
		[SerializeField]
		private BigAxeHunterEventsSO bahEvents;

		[SerializeField]
		private BigAxeHunterStateSO bahState;

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
		private SecondCameraGameboardEventsSO secondCameraGameboardEvents;

		[Header("Scoring Strategy")]
		[SerializeField]
		private StandardRoundBasedScoringLogic scoringLogic;

		[Header("MultiDisplay")]
		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayScoringEvents;

		[SerializeField]
		private GameObject multiDisplayScoringPanel;

		private void OnDisable()
		{
			bahEvents.OnHit -= OnHit;
			gameEvents.OnMiss -= OnMiss;
			gameEvents.OnMissDetected -= OnMissDetected;
			gameEvents.OnNewGame -= OnNewGame;
			scoringLogic.OnFrameUpdateComplete -= HandleScoringFrameUpdate;
			scoringLogic.OnUndoComplete -= HandleScoringUndoComplete;
			bahEvents.OnCameraMovingAwayFrom -= HandleCameraMovingAwayFromPosition;
			bahEvents.OnCameraMovedTo -= HandleCameraArrivedAtNewPosition;
		}

		private void OnEnable()
		{
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			bahEvents.OnHit += OnHit;
			gameEvents.OnMiss += OnMiss;
			gameEvents.OnMissDetected += OnMissDetected;
			gameEvents.OnNewGame += OnNewGame;
			scoringLogic.OnFrameUpdateComplete += HandleScoringFrameUpdate;
			scoringLogic.OnUndoComplete += HandleScoringUndoComplete;
			bahEvents.OnCameraMovingAwayFrom += HandleCameraMovingAwayFromPosition;
			bahEvents.OnCameraMovedTo += HandleCameraArrivedAtNewPosition;
			HandleNewGame();
		}

		private void Start()
		{
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayScoringPanel.gameObject.SetActive(value: true);
				multiDisplayScoringEvents.RaiseLoadScoringObject(multiDisplayScoringPanel.gameObject);
			}
			else
			{
				multiDisplayScoringPanel.gameObject.SetActive(value: false);
			}
		}

		private void HandleCameraArrivedAtNewPosition(ViewPosition viewPosition)
		{
			gameState.EnableTarget();
		}

		private void HandleCameraMovingAwayFromPosition(ViewPosition viewPosition)
		{
			gameState.DisableTarget();
		}

		private void HandleMiss()
		{
			HandleScoreChange(0);
			bahEvents.RaiseHitOrMiss();
		}

		private void HandleScoreChange(int score)
		{
			scoringLogic.RecordScore(new StandardScoreModel(score));
		}

		private void HandleNewGame()
		{
			gameState.CurrentFrame = 0;
			secondCameraGameboardEvents.RaiseLoadGameObjectRequest(gameboard);
		}

		private void HandleScoringFrameUpdate()
		{
			bahEvents.RaiseLoadAnimalsForPosition(bahState.CurrentViewPosition);
		}

		private void HandleScoringUndoComplete()
		{
			bahEvents.RaiseUndo();
		}

		private IEnumerator MissDetectedRoutine(PointerEventData pointerEventData, Vector2? screenPoint)
		{
			if (!gameState.IsTargetDisabled)
			{
				scoreboardLoader.RaiseScoreboardMessageRequest(new ScoreboardMessageRequest(null, "Miss!", ScoreboardMessageStyle.Normal));
				if (pointerEventData != null)
				{
					gameState.DisableTarget();
					hitEffectEvents.RaiseHitEffect(pointerEventData.position);
					yield return new WaitForSecondsRealtime(3f);
					gameState.EnableTarget();
				}
				OnMiss();
			}
		}

		private void OnHit(int score)
		{
			HandleScoreChange(score);
			bahEvents.RaiseHitOrMiss();
		}

		private void OnMiss()
		{
			HandleMiss();
		}

		private void OnMissDetected(PointerEventData arg0, Vector2? arg1)
		{
			StartCoroutine(MissDetectedRoutine(arg0, arg1));
		}

		private void OnNewGame()
		{
			HandleNewGame();
		}
	}
}
