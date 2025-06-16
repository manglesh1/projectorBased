using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
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
using UnityEngine.UI;

namespace Games.Seasonal.Merry_Axemas
{
	public class MerryAxemasController : MonoBehaviour
	{
		private GameObject _hitEffectAnimationObject;

		private bool _missRoutineRunning;

		private GameObject _multiDisplayHitEffectAnimationObject;

		private Dictionary<string, List<bool>> _playerGameboardVisibility = new Dictionary<string, List<bool>>();

		private Stack<Dictionary<string, List<bool>>> _playerGameboardVisibilityHistory = new Stack<Dictionary<string, List<bool>>>();

		private ScoreboardController _scoreboardController;

		[Header("Multi Display")]
		[SerializeField]
		private List<GameObject> multiDisplayGameboardObjects = new List<GameObject>();

		[SerializeField]
		private GameObject multiDisplayPanel;

		[SerializeField]
		private MultiDisplayScoringEventsSO multiDisplayEvents;

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
		private List<GameObject> gameboardObjects = new List<GameObject>();

		[SerializeField]
		private GameObject hitEffect;

		private void OnEnable()
		{
			gameState.NumberOfRounds = (SettingsStore.Target.FiveFrameGame ? NumberOfRounds.FiveFrames : NumberOfRounds.TenFrames);
			scoreboardLoader.RaiseLoadScoreboardRequest(ScoreboardType.Standard);
			gameEvents.OnNewGame += ResetBoard;
			gameEvents.OnMainMenu += ResetBoard;
			gameEvents.OnMiss += HandleMiss;
			gameEvents.OnMissDetected += HandleMissDetected;
			scoringLogic.OnUndoComplete += UndoFromHistory;
			for (int i = 0; i < gameboardObjects.Count; i++)
			{
				int buttonIndex = i;
				ScoredButton scoredButton = gameboardObjects[i].GetComponent<ScoredButton>();
				scoredButton.onClick.AddListener(delegate
				{
					HandleScore(scoredButton, buttonIndex);
				});
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayGameboardObjects[i].GetComponent<ScoredButton>().onClick.AddListener(delegate
					{
						scoredButton.onClick.Invoke();
					});
				}
			}
			SetupPlayerGameboards();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayEvents.RaiseLoadScoringObject(multiDisplayPanel);
			}
		}

		protected void OnDisable()
		{
			gameEvents.OnNewGame -= ResetBoard;
			gameEvents.OnMainMenu -= ResetBoard;
			gameEvents.OnMiss -= HandleMiss;
			gameEvents.OnMissDetected -= HandleMissDetected;
			scoringLogic.OnUndoComplete -= UndoFromHistory;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				multiDisplayGameboardObjects.ForEach(delegate(GameObject gameObj)
				{
					gameObj.GetComponent<ScoredButton>().onClick.RemoveAllListeners();
				});
			}
			gameboardObjects.ForEach(delegate(GameObject gameObj)
			{
				gameObj.GetComponent<ScoredButton>().onClick.RemoveAllListeners();
			});
		}

		private void AddVisibilityHistory()
		{
			_playerGameboardVisibilityHistory.Push(_playerGameboardVisibility.SimpleJsonClone());
		}

		private void ResetBoard()
		{
			Object.DestroyImmediate(_hitEffectAnimationObject);
			_playerGameboardVisibilityHistory.Clear();
			foreach (GameObject gameboardObject in gameboardObjects)
			{
				gameboardObject.GetComponent<Image>().enabled = true;
			}
			SetupPlayerGameboards();
		}

		private void SetActiveBoard()
		{
			for (int i = 0; i < _playerGameboardVisibility[gameState.CurrentPlayer].Count; i++)
			{
				gameboardObjects[i].GetComponent<Image>().enabled = _playerGameboardVisibility[gameState.CurrentPlayer][i];
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					multiDisplayGameboardObjects[i].GetComponent<Image>().enabled = _playerGameboardVisibility[gameState.CurrentPlayer][i];
				}
			}
		}

		private void HandleMiss()
		{
			AddVisibilityHistory();
			scoringLogic.RecordScore(new StandardScoreModel());
			SetActiveBoard();
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

		private void HandleScore(ScoredButton scoredButton, int buttonIndex)
		{
			if (gameState.GameStatus != GameStatus.Finished && !gameState.IsTargetDisabled)
			{
				gameState.DisableTarget();
				AddVisibilityHistory();
				_playerGameboardVisibility[gameState.CurrentPlayer][buttonIndex] = false;
				scoredButton.GetComponent<Image>().enabled = false;
				_hitEffectAnimationObject.transform.SetParent(scoredButton.transform, worldPositionStays: false);
				_hitEffectAnimationObject.SetActive(value: true);
				_hitEffectAnimationObject.GetComponent<ParticleSystem>().Play();
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					GameObject gameObject = multiDisplayGameboardObjects[buttonIndex];
					gameObject.GetComponent<Image>().enabled = false;
					_multiDisplayHitEffectAnimationObject.transform.SetParent(gameObject.transform, worldPositionStays: false);
					_multiDisplayHitEffectAnimationObject.SetActive(value: true);
					_multiDisplayHitEffectAnimationObject.GetComponent<ParticleSystem>().Play();
				}
				StartCoroutine(WaitForExplosionThenRaiseScore(scoredButton.Score));
			}
		}

		private void SetupPlayerGameboards()
		{
			_hitEffectAnimationObject = Object.Instantiate(hitEffect, base.transform);
			_hitEffectAnimationObject.SetActive(value: false);
			_multiDisplayHitEffectAnimationObject = Object.Instantiate(hitEffect, base.transform);
			_multiDisplayHitEffectAnimationObject.SetActive(value: false);
			ParticleSystem.MainModule main = _hitEffectAnimationObject.GetComponent<ParticleSystem>().main;
			main.stopAction = ParticleSystemStopAction.Disable;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				_multiDisplayHitEffectAnimationObject = Object.Instantiate(hitEffect, base.transform);
				_multiDisplayHitEffectAnimationObject.SetActive(value: false);
				ParticleSystem.MainModule main2 = _multiDisplayHitEffectAnimationObject.GetComponent<ParticleSystem>().main;
				main2.stopAction = ParticleSystemStopAction.Disable;
			}
			_playerGameboardVisibility.Clear();
			playerState.players.ForEach(delegate(PlayerData player)
			{
				_playerGameboardVisibility.Add(player.PlayerName, gameboardObjects.Select((GameObject _) => true).ToList());
			});
		}

		private void UndoFromHistory()
		{
			if (_playerGameboardVisibilityHistory.Count != 0 && !gameState.IsTargetDisabled)
			{
				Dictionary<string, List<bool>> playerGameboardVisibility = _playerGameboardVisibilityHistory.Pop();
				_playerGameboardVisibility = playerGameboardVisibility;
				SetActiveBoard();
			}
		}

		private IEnumerator WaitForExplosionThenRaiseScore(int score)
		{
			yield return new WaitForSecondsRealtime(1f);
			gameState.EnableTarget();
			scoringLogic.RecordScore(new StandardScoreModel(score));
			SetActiveBoard();
		}
	}
}
