using Games.GameState;
using Games.SharedScoringLogic;

namespace Games._21.ScoringLogic
{
	public class TwentyOneScoreModel : IScore
	{
		public ScoreToken ScoreToken { get; set; }

		public int? TotalScore { get; set; }
	}
}
