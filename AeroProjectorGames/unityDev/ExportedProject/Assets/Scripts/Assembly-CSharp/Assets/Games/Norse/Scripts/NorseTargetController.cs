using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Games.Norse.Scripts
{
	public class NorseTargetController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private NorseEventsSO taEvents;

		public void OnPointerClick(PointerEventData eventData)
		{
			taEvents.RaiseOnTargetHit();
		}

		public void OnHitDetected(PointerEventData eventData)
		{
			OnPointerClick(eventData);
		}
	}
}
