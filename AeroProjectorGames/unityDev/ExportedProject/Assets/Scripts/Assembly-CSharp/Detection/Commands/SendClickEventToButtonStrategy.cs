using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/SendClickEventToButtonStrategy")]
	public class SendClickEventToButtonStrategy : ScriptableObject
	{
		public void SendClick(GameObject proxiedObject, PointerEventData pointerData)
		{
			if (proxiedObject.IsConvertibleTo<Button>(guaranteed: true))
			{
				Button component = proxiedObject.GetComponent<Button>();
				if (component != null)
				{
					component.onClick?.Invoke();
				}
			}
		}
	}
}
