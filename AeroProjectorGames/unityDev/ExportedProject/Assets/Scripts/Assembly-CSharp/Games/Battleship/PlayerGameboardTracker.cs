using System.Collections.Generic;
using UnityEngine;

namespace Games.Battleship
{
	public class PlayerGameboardTracker
	{
		public List<int> CellsClicked = new List<int>();

		public int PlayerToAttackIndex;

		[Tooltip("Index count is one more than gameboard cell count. \n Does not use index 0.")]
		public List<GameboardCellStates> CurrentPlayerCellStates = new List<GameboardCellStates>();

		public PlayerGameboardTracker()
		{
		}

		public PlayerGameboardTracker(int gameboardCellCount, int playerToAttackIndex)
		{
			PlayerToAttackIndex = playerToAttackIndex;
			CurrentPlayerCellStates = new List<GameboardCellStates>();
			for (int i = 0; i <= gameboardCellCount; i++)
			{
				CurrentPlayerCellStates.Add(GameboardCellStates.Empty);
			}
		}
	}
}
