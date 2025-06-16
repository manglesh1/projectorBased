using Detection.Models;
using Games.GameState;
using UnityEngine;

namespace Assets.Scripts.Detection.Controllers
{
	public class GameHitDetectionController : MonoBehaviour
	{
		[SerializeField]
		private GameClickableObjectTypeEnum clickableObjectType;

		[SerializeField]
		private GamePhysicsMode gamePhysicsMode;

		[SerializeField]
		private GameStateSO gameState;

		private void OnDisable()
		{
			gameState.ClickableObjectType = GameClickableObjectTypeEnum.NotSet;
			gameState.GamePhysicsMode = GamePhysicsMode.NotSet;
		}

		private void OnEnable()
		{
			gameState.ClickableObjectType = clickableObjectType;
			gameState.GamePhysicsMode = gamePhysicsMode;
		}
	}
}
