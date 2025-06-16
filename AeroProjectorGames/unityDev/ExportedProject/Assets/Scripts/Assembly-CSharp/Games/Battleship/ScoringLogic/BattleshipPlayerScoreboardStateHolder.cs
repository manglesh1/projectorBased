namespace Games.Battleship.ScoringLogic
{
	public class BattleshipPlayerScoreboardStateHolder
	{
		public bool IsTwoHitShipSunk;

		public bool IsThreeHitShipSunk;

		public bool IsFourHitShipSunk;

		public int TwoHitShipHitCount;

		public int ThreeHitShipHitCount;

		public int FourHitShipHitCount;

		public BattleshipPlayerScoreboardStateHolder()
		{
			IsTwoHitShipSunk = false;
			IsThreeHitShipSunk = false;
			IsFourHitShipSunk = false;
			TwoHitShipHitCount = 0;
			ThreeHitShipHitCount = 0;
			FourHitShipHitCount = 0;
		}
	}
}
