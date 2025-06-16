using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Axteroids
{
	public class BombHit : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[SerializeField]
		private BombExplosion _explosion;

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
