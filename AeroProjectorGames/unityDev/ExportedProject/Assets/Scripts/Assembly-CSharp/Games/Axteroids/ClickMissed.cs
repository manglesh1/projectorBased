using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Axteroids
{
	public class ClickMissed : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[SerializeField]
		private AxteroidsGameboardController animationController;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private AxteroidsObjectList objectName;

		[SerializeField]
		private int scoreValue;

		private void OnEnable()
		{
			gameEvents.OnMissDetected += OnHitDetected;
		}

		private void OnDisable()
		{
			gameEvents.OnMissDetected -= OnHitDetected;
		}

		private void ObjectHitAction(PointerEventData pointerData, bool isMouseClicked)
		{
			if (!gameState.IsTargetDisabled)
			{
				animationController.PlayAnimation(objectName, scoreValue, pointerData, isMouseClicked);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			ObjectHitAction(null, isMouseClicked: true);
		}

		private void OnHitDetected(PointerEventData pointerData, Vector2? screenPosition)
		{
			ObjectHitAction(pointerData, isMouseClicked: false);
		}
	}
}
