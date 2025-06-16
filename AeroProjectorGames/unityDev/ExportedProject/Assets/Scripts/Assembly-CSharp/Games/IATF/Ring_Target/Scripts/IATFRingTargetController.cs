using System.Collections;
using System.Collections.Generic;
using Games.CustomComponents;
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

namespace Games.IATF.Ring_Target.Scripts
{
	public class IATFRingTargetController : MonoBehaviour
	{
		private bool _missRoutineRunning;

		private ScoreboardController _scoreboardController;

		[Header("Multi Display")]
		[SerializeField]
		private GameObject multiDisplayPanel;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

		[SerializeField]
		private List<TwoColorAnimatedScoredButton> multiDisplayScoredButtons;

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

		[Header("Scoring Strategy")]
		[SerializeField]
		private StandardRoundBasedScoringLogic scoringLogic;

		[Header("Game Specific")]
		[SerializeField]
		private List<TwoColorAnimatedScoredButton> scoredButtons;

		private void OnEnable()
		{
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
			for (int i = 0; i < scoredButtons.Count; i++)
			{
				TwoColorAnimatedScoredButton btn = scoredButtons[i];
				btn.onClick.AddListener(delegate
				{
					HandleRingClick(btn);
				});
				btn.TwoColorAnimator.OnAnimationFinished.AddListener(delegate
				{
					RecordScore(btn.Score);
				});
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					TwoColorAnimatedScoredButton multiDisplayButton = multiDisplayScoredButtons[i];
					multiDisplayButton.onClick.AddListener(delegate
					{
						multiDisplayButton.TwoColorAnimator.Animate();
						btn.onClick.Invoke();
					});
				}
			}
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayPanel);
			}
			else
			{
				multiDisplayPanel.SetActive(value: false);
			}
		}

		protected void OnDisable()
		{
			scoredButtons.ForEach(delegate(TwoColorAnimatedScoredButton btn)
			{
				btn.onClick.RemoveAllListeners();
				btn.TwoColorAnimator.OnAnimationFinished.RemoveAllListeners();
			});
			multiDisplayScoredButtons.ForEach(delegate(TwoColorAnimatedScoredButton btn)
			{
				btn.onClick.RemoveAllListeners();
			});
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
		}

		private void HandleMiss()
		{
			scoringLogic.RecordScore(new StandardScoreModel());
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

		private void HandleRingClick(TwoColorAnimatedScoredButton button)
		{
			if (!gameState.IsTargetDisabled)
			{
				gameState.DisableTarget();
				button.TwoColorAnimator.Animate();
			}
		}

		private void RecordScore(int score)
		{
			gameState.EnableTarget();
			scoringLogic.RecordScore(new StandardScoreModel(score));
		}
	}
}
