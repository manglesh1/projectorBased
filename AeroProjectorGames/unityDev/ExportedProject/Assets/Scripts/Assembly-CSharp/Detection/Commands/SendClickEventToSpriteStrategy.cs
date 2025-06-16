using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/SendClickEventToSpriteStrategy")]
	public class SendClickEventToSpriteStrategy : ScriptableObject
	{
		public void SendClick(GameObject proxiedObject, PointerEventData pointerData)
		{
			if (proxiedObject.IsConvertibleTo<Button>(guaranteed: true))
			{
				int num = ((proxiedObject.GetComponent<Sprite>() != null) ? 1 : 0);
			}
		}
	}
}
