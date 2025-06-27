using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Detection.Commands
{
	public class RayCastFactory : MonoBehaviour
	{
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GetRayCastsFromGraphicsStrategy graphicsStrategy;

		[SerializeField]
		private GetRayCastsFrom2DPhysicsStrategy physics2dStrategy;

		[SerializeField]
		private GetRayCastsFrom3DPhysicsStrategy physics3dStrategy;

		public RaycastResult GetRayCasts(PointerEventData pointerData, Vector3 localPosition)
		{
			return gameState.GamePhysicsMode switch
			{
				GamePhysicsMode.Graphics => graphicsStrategy.GetRayCasts(pointerData, localPosition), 
				GamePhysicsMode.Physics2D => physics2dStrategy.GetRayCasts(pointerData, localPosition), 
				GamePhysicsMode.Physics3D => physics3dStrategy.GetRayCasts(pointerData, localPosition), 
				_ => default(RaycastResult), 
			};
		}
	}
}
