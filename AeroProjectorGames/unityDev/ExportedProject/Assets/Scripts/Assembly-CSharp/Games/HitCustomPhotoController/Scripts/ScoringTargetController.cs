using System;
using Games.GameState;
using Games.HitCustomPhotoController.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.HitCustomPhotoController.Scripts
{
	public class ScoringTargetController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private bool _loaded;

		private GameObject _primaryTarget;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		private void EnsureLoaded()
		{
			if (!_loaded)
			{
				throw new ArgumentException("Init must be called on the Scoring Target Controller.");
			}
		}

		public void Init(GameObject primaryTarget)
		{
			_primaryTarget = primaryTarget;
			_loaded = true;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!gameState.IsTargetDisabled)
			{
				base.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
				_primaryTarget.SendMessage("OnPointerDown", eventData);
			}
		}
	}
}
