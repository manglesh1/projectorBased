using UnityEngine;

namespace Scoreboard.UnscoredTwoPlayerScoreboard
{
	public class UnscoredTwoPlayerScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("Scoreboard Prefab")]
		[SerializeField]
		private GameObject unscoredTwoPlayerScoreboard;

		public GameObject GetScoreboard()
		{
			return unscoredTwoPlayerScoreboard;
		}
	}
}
