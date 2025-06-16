using System.Collections.Generic;

namespace Games.Concentration.Scripts.Gameboard.SaveState
{
	public class GameboardSaveStateModel
	{
		public List<GameboardLayoutStateModel> GameboardLayoutState;

		public int StealCardsInPlayCount;

		public int WildCardsInPlayCount;
	}
}
