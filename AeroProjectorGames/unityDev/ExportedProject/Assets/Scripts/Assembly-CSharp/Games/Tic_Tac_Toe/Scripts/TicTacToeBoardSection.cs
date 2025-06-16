using System;
using UnityEngine;

namespace Games.Tic_Tac_Toe.Scripts
{
	[Serializable]
	public class TicTacToeBoardSection
	{
		public int BoardIndex { get; set; }

		public SpriteRenderer GameboardSpriteRenderer { get; set; }

		public SpriteRenderer MultiDisplayScoringPanelSpriteRenderer { get; set; }
	}
}
