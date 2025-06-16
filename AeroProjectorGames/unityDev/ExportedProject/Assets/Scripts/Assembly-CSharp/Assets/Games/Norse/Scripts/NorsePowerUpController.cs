using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Games.Norse.Scripts
{
	public class NorsePowerUpController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private NorseEventsSO taEvents;

		public void OnPointerClick(PointerEventData eventData)
		{
			taEvents.RaiseOnPowerUpHit();
		}

		public void OnHitDetected(PointerEventData eventData)
		{
			OnPointerClick(eventData);
		}
	}
}
