using UnityEngine;

namespace Games.Tic_Tac_Toe.Models
{
	public class PlayerGridCell
	{
		public GameObject CurrentGameObject { get; set; }

		public string PlayerName { get; set; }

		public PlayerGridCell(GameObject currentGameObject, string playerName)
		{
			CurrentGameObject = currentGameObject;
			PlayerName = playerName;
		}
	}
}
