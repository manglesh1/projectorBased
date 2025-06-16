using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/SendClickEventToEventTriggerStrategy")]
	public class SendClickEventToEventTriggerStrategy : ScriptableObject
	{
		public void SendClick(GameObject proxiedObject, PointerEventData pointerData)
		{
			proxiedObject.SendMessage("OnPointerDown", pointerData);
		}
	}
}
