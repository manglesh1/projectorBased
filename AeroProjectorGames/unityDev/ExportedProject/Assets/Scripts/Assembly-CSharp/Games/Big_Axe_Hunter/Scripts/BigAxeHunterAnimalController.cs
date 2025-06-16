using System;
using System.Collections;
using Games.GameState;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Big_Axe_Hunter.Scripts
{
	public class BigAxeHunterAnimalController : MonoBehaviour
	{
		private CapsuleCollider[] _colliders;

		private bool _initialized;

		private int _scoreValue;

		private SpawnPoint _spawnPoint;

		private ViewPosition _viewPosition;

		[Header("Scoring")]
		[SerializeField]
		private int scoreValue;

		[Header("References")]
		[SerializeField]
		private BigAxeHunterEventsSO bahEvents;

		[Header("Animation Related")]
		[SerializeField]
		private Animator animalAnimator;

		[SerializeField]
		private AnimationClip deathAnimation;

		[SerializeField]
		private AnimationClip runAnimation;

		[SerializeField]
		private BigAxeHunterEventsSO bigAxeHunterEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		private void OnDisable()
		{
			CapsuleCollider[] colliders = _colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].GetComponent<EventTrigger>().triggers.ForEach(delegate(EventTrigger.Entry entry)
				{
					entry.callback.RemoveAllListeners();
				});
			}
			bahEvents.OnMultiDisplaySpawnPointHit -= HandleMultiDisplayHit;
		}

		private void OnEnable()
		{
			_colliders = GetComponentsInChildren<CapsuleCollider>();
			CapsuleCollider[] colliders = _colliders;
			for (int i = 0; i < colliders.Length; i++)
			{
				EventTrigger eventTrigger = colliders[i].AddComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback.AddListener(delegate
				{
					HandleHit();
				});
				eventTrigger.triggers.Add(entry);
			}
			bahEvents.OnMultiDisplaySpawnPointHit += HandleMultiDisplayHit;
		}

		private void HandleMultiDisplayHit(SpawnPoint spawnPoint)
		{
			if (_spawnPoint == spawnPoint)
			{
				HandleHit();
			}
		}

		private IEnumerator AnimateFlee(AnimationClip anim)
		{
			animalAnimator.Play(anim.name);
			yield return new WaitForSecondsRealtime(anim.length);
		}

		private IEnumerator AnimateHitEffect(AnimationClip anim, Action onCompleted = null)
		{
			gameState.DisableTarget();
			animalAnimator.Play(anim.name);
			yield return new WaitForSecondsRealtime(anim.length);
			Vector3 originalPosition = base.transform.parent.localPosition;
			Vector3 blinkPosition = new Vector3(-1000f, -1000f, -1000f);
			float blinkDelay = 0.1f;
			yield return BlinkAnimal(blinkPosition, originalPosition, blinkDelay);
			yield return BlinkAnimal(blinkPosition, originalPosition, blinkDelay);
			yield return BlinkAnimal(blinkPosition, originalPosition, blinkDelay);
			yield return BlinkAnimal(blinkPosition, originalPosition, blinkDelay);
			yield return BlinkAnimal(blinkPosition, originalPosition, blinkDelay);
			yield return BlinkAnimal(blinkPosition, originalPosition, blinkDelay);
			yield return BlinkAnimal(blinkPosition, originalPosition, blinkDelay);
			gameState.EnableTarget();
			onCompleted?.Invoke();
			UnityEngine.Object.Destroy(base.transform.parent.gameObject);
		}

		private IEnumerator BlinkAnimal(Vector3 blinkPosition, Vector3 originalPosition, float delay)
		{
			base.transform.parent.localPosition = blinkPosition;
			yield return new WaitForSecondsRealtime(delay);
			base.transform.parent.localPosition = originalPosition;
			yield return new WaitForSecondsRealtime(delay);
		}

		private void EnsureInitialized()
		{
			if (!_initialized)
			{
				throw new ArgumentException("Animal must be initialized");
			}
		}

		public void Flee()
		{
			EnsureInitialized();
			StartCoroutine(AnimateFlee(runAnimation));
		}

		private void HandleHit()
		{
			EnsureInitialized();
			if (!gameState.IsTargetDisabled)
			{
				StartCoroutine(AnimateHitEffect(deathAnimation, delegate
				{
					bahEvents.RaiseOnHit(_scoreValue);
				}));
			}
		}

		public void Initialize(SpawnPoint spawnPoint, int score)
		{
			_spawnPoint = spawnPoint;
			_scoreValue = score;
			_initialized = true;
		}
	}
}
