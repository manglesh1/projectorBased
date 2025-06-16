using System;
using Games.CustomComponents;
using Games.GameState;
using Games.HitCustomPhotoController.ScriptableObjects;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.HitCustomPhotoController.Scripts
{
	[RequireComponent(typeof(TargetMovementController))]
	public class TargetController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private bool _loaded;

		private GameObject _scoringTarget;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		public GameObject ScoringTarget => _scoringTarget;

		private void OnDisable()
		{
			if (_scoringTarget != null)
			{
				_scoringTarget.SetActive(value: false);
			}
		}

		private void OnEnable()
		{
			GetComponent<TargetMovementController>().enabled = true;
			if (_scoringTarget != null)
			{
				_scoringTarget.SetActive(value: true);
			}
		}

		private void OnHitDetected(PointerEventData pointerData)
		{
			OnPointerDown(pointerData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			EnsureLoaded();
			if (!gameState.IsTargetDisabled)
			{
				base.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
				hitCustomPhotoEvents.RaiseTargetHit(base.gameObject.GetComponent<ScoredButton>());
			}
		}

		private void Update()
		{
			if (_loaded && SettingsStore.Interaction.MultiDisplayEnabled)
			{
				_scoringTarget.transform.localPosition = base.transform.localPosition;
				_scoringTarget.transform.localScale = base.transform.localScale;
			}
		}

		private void EnsureLoaded()
		{
			if (!_loaded)
			{
				throw new ArgumentException("Init must be called on the TargetController before it can be used");
			}
		}

		public void Init(GameObject scoringTarget)
		{
			_scoringTarget = scoringTarget;
			_loaded = true;
		}
	}
}
