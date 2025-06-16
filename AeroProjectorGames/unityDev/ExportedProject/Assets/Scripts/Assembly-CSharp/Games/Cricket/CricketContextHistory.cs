using System;
using Games.Cricket.Logic.Scoring;

namespace Games.Cricket
{
	[Serializable]
	public class CricketContextHistory
	{
		public int CurrentThrow { get; }

		public int CurrentPlayerIndex { get; }

		public CricketGameState CricketGameState { get; }

		public PlayerScoreCollection PlayerScoreCollection { get; }

		public CricketContextHistory(int currentThrow, int currentPlayerIndex, CricketGameState cricketGameState, PlayerScoreCollection playerScoreCollection)
		{
			CurrentThrow = currentThrow;
			CurrentPlayerIndex = currentPlayerIndex;
			CricketGameState = cricketGameState;
			PlayerScoreCollection = playerScoreCollection;
		}
	}
}
