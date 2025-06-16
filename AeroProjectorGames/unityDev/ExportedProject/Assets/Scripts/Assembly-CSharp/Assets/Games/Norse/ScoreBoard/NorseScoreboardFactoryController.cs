using UnityEngine;

namespace Assets.Games.Norse.ScoreBoard
{
	public class NorseScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("Throw Axe Scoreboard Prefabs")]
		[SerializeField]
		private GameObject throwAxeScoreboard;

		public GameObject GetScoreboard()
		{
			return throwAxeScoreboard;
		}
	}
}
