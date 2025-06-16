using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/SendClickEventToColliderStrategy")]
	public class SendClickEventToColliderStrategy : ScriptableObject
	{
		public void SendClick(GameObject proxiedObject, PointerEventData pointerData)
		{
			proxiedObject.SendMessage("OnHitDetected", pointerData);
		}
	}
}
