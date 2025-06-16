using System;
using Games.GameState;
using Players;
using UnityEngine;

namespace Scoreboard.BattleshipScoreboard
{
	public class BattleshipScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("21 Scoreboard Prefabs")]
		[SerializeField]
		private GameObject battleshipScoreboard1Player;

		[SerializeField]
		private GameObject battleshipScoreboard2Player;

		[SerializeField]
		private GameObject battleshipScoreboard3Player;

		[SerializeField]
		private GameObject battleshipScoreboard4Player;

		[SerializeField]
		private GameObject battleshipScoreboard5Player;

		[SerializeField]
		private GameObject battleshipScoreboard6Player;

		public GameObject GetScoreboard(NumberOfRounds numberOfRounds, PlayerStateSO playerState)
		{
			int count = playerState.players.Count;
			if (numberOfRounds == NumberOfRounds.InfiniteScored)
			{
				switch (count)
				{
				case 1:
					return battleshipScoreboard1Player;
				case 2:
					return battleshipScoreboard2Player;
				case 3:
					return battleshipScoreboard3Player;
				case 4:
					return battleshipScoreboard4Player;
				case 5:
					return battleshipScoreboard5Player;
				case 6:
					return battleshipScoreboard6Player;
				}
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}
