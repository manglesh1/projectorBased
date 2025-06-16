using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Commands
{
	public class GetRayCastsFrom3DPhysicsStrategy : MonoBehaviour
	{
		public RaycastResult GetRayCasts(PointerEventData pointerData, Vector3 localPoint)
		{
			RaycastResult result = default(RaycastResult);
			if (Physics.Raycast(new Vector3(localPoint.x, localPoint.y, 0f), Vector3.forward, out var hitInfo))
			{
				result.gameObject = hitInfo.transform.gameObject;
			}
			return result;
		}
	}
}
