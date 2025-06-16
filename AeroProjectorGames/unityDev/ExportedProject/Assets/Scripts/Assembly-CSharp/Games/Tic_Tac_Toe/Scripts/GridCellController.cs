using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Tic_Tac_Toe.Scripts
{
	public class GridCellController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnEnable()
		{
			gameEvents.OnNewGame += Initialize;
		}

		private void OnDisable()
		{
			gameEvents.OnNewGame -= Initialize;
		}

		private void OnHitDetected(PointerEventData pointerEventData)
		{
			OnPointerDown(pointerEventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			gameEvents.RaiseGameObjectClicked(base.gameObject);
		}

		private void Initialize()
		{
			base.gameObject.GetComponent<SpriteRenderer>().sprite = null;
			base.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		}
	}
}
