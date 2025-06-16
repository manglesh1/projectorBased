using System.Collections.Generic;

namespace Games.GameState
{
	public class GameStateHistory
	{
		public int CurrentFrame { get; set; }

		public string CurrentPlayer { get; set; }

		public int CurrentRound { get; set; }

		public int ThrowsRemaining { get; set; }

		public Dictionary<string, List<int?>> RoundScores { get; set; }

		public Dictionary<string, List<ScoreToken>> InfiniteScoredGameScores { get; set; } = new Dictionary<string, List<ScoreToken>>();

		public Dictionary<string, int?> TotalScores { get; set; } = new Dictionary<string, int?>();
	}
}
