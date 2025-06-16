using System;
using Games.GameState;
using Players;
using UnityEngine;

namespace Scoreboard.TwentyOneScoreboard
{
	public class TwentyOneScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("21 Scoreboard Prefabs")]
		[SerializeField]
		private GameObject twentyOneScoreboard1Player;

		[SerializeField]
		private GameObject twentyOneScoreboard2Player;

		[SerializeField]
		private GameObject twentyOneScoreboard3Player;

		[SerializeField]
		private GameObject twentyOneScoreboard4Player;

		[SerializeField]
		private GameObject twentyOneScoreboard5Player;

		[SerializeField]
		private GameObject twentyOneScoreboard6Player;

		public GameObject GetScoreboard(NumberOfRounds numberOfRounds, PlayerStateSO playerState)
		{
			int count = playerState.players.Count;
			if (numberOfRounds == NumberOfRounds.InfiniteScored)
			{
				switch (count)
				{
				case 1:
					return twentyOneScoreboard1Player;
				case 2:
					return twentyOneScoreboard2Player;
				case 3:
					return twentyOneScoreboard3Player;
				case 4:
					return twentyOneScoreboard4Player;
				case 5:
					return twentyOneScoreboard5Player;
				case 6:
					return twentyOneScoreboard6Player;
				}
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}
