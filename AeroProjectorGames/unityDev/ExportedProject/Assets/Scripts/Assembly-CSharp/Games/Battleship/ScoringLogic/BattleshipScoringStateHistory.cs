using System.Collections.Generic;

namespace Games.Battleship.ScoringLogic
{
	public class BattleshipScoringStateHistory
	{
		public int CurrentFrame { get; set; }

		public string CurrentPlayer { get; set; }

		public int CurrentRound { get; set; }

		public List<string> EliminatedPlayers { get; set; } = new List<string>();

		public Dictionary<string, BattleshipPlayerScoreboardStateHolder> PlayerScoreboardStateholder { get; set; } = new Dictionary<string, BattleshipPlayerScoreboardStateHolder>();

		public int ThrowsRemaining { get; set; }

		public Dictionary<string, List<int?>> RoundScores { get; set; }

		public Dictionary<string, int?> TotalScores { get; set; } = new Dictionary<string, int?>();
	}
}
