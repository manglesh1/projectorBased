using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Concentration.Scripts.Gameboard.Models
{
	[Serializable]
	public class GameTokenLayoutModel : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("A list of the game objects that the player will interact with")]
		private List<GameObject> gameTokens = new List<GameObject>();

		public List<GameObject> GameTokens => gameTokens;
	}
}
