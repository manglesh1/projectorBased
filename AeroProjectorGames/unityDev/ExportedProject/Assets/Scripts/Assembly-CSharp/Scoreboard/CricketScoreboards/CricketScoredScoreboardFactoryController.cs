using System;
using Players;
using UnityEngine;

namespace Scoreboard.CricketScoreboards
{
	public class CricketScoredScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("Scored Cricket Scoreboard Prefabs")]
		[SerializeField]
		private GameObject scoredCricketScoreboardTemplate1Player;

		[SerializeField]
		private GameObject scoredCricketScoreboardTemplate2Player;

		[SerializeField]
		private GameObject scoredCricketScoreboardTemplate3Player;

		[SerializeField]
		private GameObject scoredCricketScoreboardTemplate4Player;

		[SerializeField]
		private GameObject scoredCricketScoreboardTemplate5Player;

		[SerializeField]
		private GameObject scoredCricketScoreboardTemplate6Player;

		public GameObject GetScoreboard(PlayerStateSO playerState)
		{
			switch (playerState.players.Count)
			{
			case 1:
				return scoredCricketScoreboardTemplate1Player;
			case 2:
				return scoredCricketScoreboardTemplate2Player;
			case 3:
				return scoredCricketScoreboardTemplate3Player;
			case 4:
				return scoredCricketScoreboardTemplate4Player;
			case 5:
				return scoredCricketScoreboardTemplate5Player;
			case 6:
				return scoredCricketScoreboardTemplate6Player;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
