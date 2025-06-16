using System.Collections.Generic;
using UnityEngine;

namespace Assets.Games.Norse.Scripts
{
	[CreateAssetMenu(menuName = "Games/ThrowAxe/ThrowAxeState")]
	public class NorseStateSO : ScriptableObject
	{
		public string CommandingPlayer { get; set; }

		public bool DirectionIsReversed { get; set; }

		public string GameWord { get; set; }

		public bool IsCommandSet { get; set; }

		public string PlayerName { get; set; }

		public Dictionary<string, NorsePlayerState> Players { get; set; } = new Dictionary<string, NorsePlayerState>(6);

		public float PowerUpLocationX { get; set; }

		public float PowerUpLocationY { get; set; }

		public NorsePowerUpEnum SelectedPowerUp { get; set; }

		public float TargetLocationX { get; set; }

		public float TargetLocationY { get; set; }
	}
}
