using System;
using Players;
using UnityEngine;

namespace Scoreboard.CricketScoreboards
{
	public class CricketUnscoredScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("Unscored Cricket Scoreboard Prefabs")]
		[SerializeField]
		private GameObject unscoredCricketScoreboardTemplate1Player;

		[SerializeField]
		private GameObject unscoredCricketScoreboardTemplate2Player;

		[SerializeField]
		private GameObject unscoredCricketScoreboardTemplate3Player;

		[SerializeField]
		private GameObject unscoredCricketScoreboardTemplate4Player;

		[SerializeField]
		private GameObject unscoredCricketScoreboardTemplate5Player;

		[SerializeField]
		private GameObject unscoredCricketScoreboardTemplate6Player;

		public GameObject GetScoreboard(PlayerStateSO playerState)
		{
			return playerState.players.Count switch
			{
				1 => unscoredCricketScoreboardTemplate1Player, 
				2 => unscoredCricketScoreboardTemplate2Player, 
				3 => unscoredCricketScoreboardTemplate3Player, 
				4 => unscoredCricketScoreboardTemplate4Player, 
				5 => unscoredCricketScoreboardTemplate5Player, 
				6 => unscoredCricketScoreboardTemplate6Player, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}
}
