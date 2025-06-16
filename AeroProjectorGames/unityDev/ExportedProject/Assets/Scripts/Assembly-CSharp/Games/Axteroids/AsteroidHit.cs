using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Axteroids
{
	public class AsteroidHit : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerClickHandler
	{
		[SerializeField]
		private AsteroidExplosion _explosion;

		public void OnPointerDown(PointerEventData pointerData)
		{
			_explosion.HitDetected(pointerData, isMouseClicked: false);
		}

		private void OnHitDetected(PointerEventData pointerData)
		{
			_explosion.HitDetected(pointerData, isMouseClicked: false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_explosion.HitDetected(null, isMouseClicked: true);
		}
	}
}
