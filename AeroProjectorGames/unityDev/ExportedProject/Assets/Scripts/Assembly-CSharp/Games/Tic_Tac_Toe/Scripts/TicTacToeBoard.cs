using System;

namespace Games.Tic_Tac_Toe.Scripts
{
	[Serializable]
	public class TicTacToeBoard
	{
		public TicTacToeBoardSection TopLeft { get; set; }

		public TicTacToeBoardSection TopCenter { get; set; }

		public TicTacToeBoardSection TopRight { get; set; }

		public TicTacToeBoardSection MiddleLeft { get; set; }

		public TicTacToeBoardSection MiddleCenter { get; set; }

		public TicTacToeBoardSection MiddleRight { get; set; }

		public TicTacToeBoardSection BottomLeft { get; set; }

		public TicTacToeBoardSection BottomCenter { get; set; }

		public TicTacToeBoardSection BottomRight { get; set; }
	}
}
