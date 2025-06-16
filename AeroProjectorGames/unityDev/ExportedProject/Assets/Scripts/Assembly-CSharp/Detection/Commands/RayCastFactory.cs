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
			switch (gameState.GamePhysicsMode)
			{
			case GamePhysicsMode.Graphics:
				return graphicsStrategy.GetRayCasts(pointerData, localPosition);
			case GamePhysicsMode.Physics2D:
				return physics2dStrategy.GetRayCasts(pointerData, localPosition);
			case GamePhysicsMode.Physics3D:
				return physics3dStrategy.GetRayCasts(pointerData, localPosition);
			default:
				return default(RaycastResult);
			}
		}
	}
}
