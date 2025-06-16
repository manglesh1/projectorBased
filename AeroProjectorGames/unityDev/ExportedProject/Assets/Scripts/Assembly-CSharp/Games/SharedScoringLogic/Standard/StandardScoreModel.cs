namespace Games.SharedScoringLogic.Standard
{
	public class StandardScoreModel : IScore
	{
		public int Score { get; }

		public StandardScoreModel(int score = 0)
		{
			Score = score;
		}
	}
}
