using System;
using Players;
using UnityEngine;

namespace Scoreboard.Concentration
{
	public class ConcentrationScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("Concentration Scoreboard Prefabs")]
		[SerializeField]
		private GameObject concentrationScoreboard1Player;

		[SerializeField]
		private GameObject concentrationScoreboard2Player;

		[SerializeField]
		private GameObject concentrationScoreboard3Player;

		[SerializeField]
		private GameObject concentrationScoreboard4Player;

		[SerializeField]
		private GameObject concentrationScoreboard5Player;

		[SerializeField]
		private GameObject concentrationScoreboard6Player;

		public GameObject GetScoreboard(PlayerStateSO playerState)
		{
			switch (playerState.players.Count)
			{
			case 1:
				return concentrationScoreboard1Player;
			case 2:
				return concentrationScoreboard2Player;
			case 3:
				return concentrationScoreboard3Player;
			case 4:
				return concentrationScoreboard4Player;
			case 5:
				return concentrationScoreboard5Player;
			case 6:
				return concentrationScoreboard6Player;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
