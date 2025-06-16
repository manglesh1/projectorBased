using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Axteroids
{
	public class ShipHit : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[SerializeField]
		private ShipExplosion _explosion;

		public void OnPointerDown(PointerEventData eventData)
		{
			_explosion.Hit(null, isMouseClicked: true);
		}

		private void OnHitDetected(PointerEventData pointerData)
		{
			_explosion.Hit(pointerData, isMouseClicked: false);
		}
	}
}
