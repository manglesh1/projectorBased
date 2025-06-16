using Assets.Games.Norse.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Games.Norse
{
	public class NorseMultiDisplayGridController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public BoxCollider2D sourceCollider;

		public BoxCollider2D targetCollider;

		public NorseGameBoardController targetScript;

		public void OnPointerClick(PointerEventData eventData)
		{
			Vector3 vector = Camera.allCameras[1].ScreenToWorldPoint(eventData.position);
			Vector3 min = sourceCollider.bounds.min;
			Vector3 max = sourceCollider.bounds.max;
			float x = (vector.x - min.x) / (max.x - min.x);
			float y = (vector.y - min.y) / (max.y - min.y);
			Vector2 vector2 = new Vector2(x, y);
			Vector3 min2 = targetCollider.bounds.min;
			Vector3 max2 = targetCollider.bounds.max;
			float x2 = min2.x + vector2.x * (max2.x - min2.x);
			float y2 = min2.y + vector2.y * (max2.y - min2.y);
			Vector3 vector3 = Camera.main.WorldToScreenPoint(new Vector3(x2, y2, 0f));
			targetScript.OnPointerClick(new PointerEventData(EventSystem.current)
			{
				position = vector3
			});
		}
	}
}
