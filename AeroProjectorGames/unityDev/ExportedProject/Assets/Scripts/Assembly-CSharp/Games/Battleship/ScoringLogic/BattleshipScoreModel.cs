using Games.SharedScoringLogic;

namespace Games.Battleship.ScoringLogic
{
	public class BattleshipScoreModel : IScore
	{
		public string AttackedPlayerName { get; }

		public int Score { get; }

		public BattleshipScoreModel(string attackedPlayer, int score = 0)
		{
			AttackedPlayerName = attackedPlayer;
			Score = score;
		}
	}
}
