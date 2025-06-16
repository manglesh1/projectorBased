using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Games.Norse.Scripts
{
	public class NorseGameBoardController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[Header("Scriptable Objects")]
		[SerializeField]
		private NorseEventsSO taEvents;

		public void OnPointerClick(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponentInParent<RectTransform>(), eventData.position, Camera.main, out var localPoint);
			taEvents.RaiseOnGameBoardHit(localPoint);
		}

		public void OnHitDetected(PointerEventData eventData)
		{
			OnPointerClick(eventData);
		}
	}
}
