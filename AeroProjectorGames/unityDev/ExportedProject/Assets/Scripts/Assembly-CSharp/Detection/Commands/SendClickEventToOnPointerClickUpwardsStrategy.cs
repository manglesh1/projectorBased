using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/SendClickEventToOnPointerClickUpwardsStrategy")]
	public class SendClickEventToOnPointerClickUpwardsStrategy : ScriptableObject
	{
		public void SendClick(GameObject proxiedObject, PointerEventData pointerData)
		{
			proxiedObject.SendMessageUpwards("OnPointerClick", pointerData);
		}
	}
}
