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
			return playerState.players.Count switch
			{
				1 => concentrationScoreboard1Player, 
				2 => concentrationScoreboard2Player, 
				3 => concentrationScoreboard3Player, 
				4 => concentrationScoreboard4Player, 
				5 => concentrationScoreboard5Player, 
				6 => concentrationScoreboard6Player, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}
}
