using System.Collections;
using Games.GameState;
using Games.SharedScoringLogic.Standard;
using HitEffects;
using Players;
using Scoreboard;
using Scoreboard.Messaging;
using Scoreboard.PrefabScripts;
using Settings;
using UI.MultiDisplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.BottleBash
{
	public class BottleBashGame : MonoBehaviour
	{
		private bool _missRoutineRunning;

		private ScoreboardController _scoreboardController;

		[SerializeField]
		private TokenSpawnerController tokenSpawnerController;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitEffectEventsSO hitEffectEvents;

		[SerializeField]
		private GameObject multiDisplayScoringPanel;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private ScoreboardLoaderEventsSO scoreboardLoader;

		[Header("Scoring Strategy")]
		[SerializeField]
		private StandardRoundBasedScoringLogic scoringLogic;

		private void OnEnable()
		{
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayScoringPanel);
			}
			HandleNewGame();
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnScoreChange += HandleScoreChange;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
		}

		protected void OnDisable()
		{
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnScoreChange -= HandleScoreChange;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
		}

		private void HandleScoreChange(int? score)
		{
			scoringLogic.RecordScore(new StandardScoreModel(score.GetValueOrDefault()));
			gameEvents.RaiseNewRound();
		}

		private void HandleMiss()
		{
			HandleScoreChange(null);
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

		private void HandleNewGame()
		{
			gameState.CurrentFrame = 0;
		}
	}
}
