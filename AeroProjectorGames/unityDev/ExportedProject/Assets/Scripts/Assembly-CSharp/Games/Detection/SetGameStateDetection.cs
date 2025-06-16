using Games.GameState;
using Games.Models;
using UnityEngine;

namespace Games.Detection
{
	public class SetGameStateDetection : MonoBehaviour
	{
		[SerializeField]
		private GameSO game;

		[SerializeField]
		private GameStateSO gameState;

		private void OnEnable()
		{
			gameState.DetectionEnabled = game.DetectionEnabled;
		}
	}
}
