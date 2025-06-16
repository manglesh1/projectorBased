using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.HitCustomPhotoController.Scripts
{
	public class MissTargetController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnHitDetected(PointerEventData pointerEventData)
		{
			gameEvents.RaiseMissDetected(pointerEventData, pointerEventData.position);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Vector3 vector = eventData.position;
			Vector2 value = new Vector2(vector.x, vector.y);
			gameEvents.RaiseMissDetected(null, value);
		}
	}
}
