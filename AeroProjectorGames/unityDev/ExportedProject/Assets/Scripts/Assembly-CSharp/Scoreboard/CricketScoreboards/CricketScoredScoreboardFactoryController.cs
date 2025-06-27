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
			return playerState.players.Count switch
			{
				1 => scoredCricketScoreboardTemplate1Player, 
				2 => scoredCricketScoreboardTemplate2Player, 
				3 => scoredCricketScoreboardTemplate3Player, 
				4 => scoredCricketScoreboardTemplate4Player, 
				5 => scoredCricketScoreboardTemplate5Player, 
				6 => scoredCricketScoreboardTemplate6Player, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}
}
