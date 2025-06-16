using System;

namespace Games.Cricket
{
	[Serializable]
	public enum CricketGameState
	{
		Setup = 0,
		Loading = 1,
		Scored = 2,
		Unscored = 3
	}
}
