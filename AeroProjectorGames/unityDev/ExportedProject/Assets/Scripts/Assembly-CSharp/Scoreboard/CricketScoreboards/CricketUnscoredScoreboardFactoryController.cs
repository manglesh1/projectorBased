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
			switch (playerState.players.Count)
			{
			case 1:
				return unscoredCricketScoreboardTemplate1Player;
			case 2:
				return unscoredCricketScoreboardTemplate2Player;
			case 3:
				return unscoredCricketScoreboardTemplate3Player;
			case 4:
				return unscoredCricketScoreboardTemplate4Player;
			case 5:
				return unscoredCricketScoreboardTemplate5Player;
			case 6:
				return unscoredCricketScoreboardTemplate6Player;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
