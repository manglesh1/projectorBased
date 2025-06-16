using UnityEngine;

namespace Scoreboard.WordWhackScoreboard
{
	public class WordWhackScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("WordWhack Scoreboard Prefab")]
		[SerializeField]
		private GameObject wordWhackScoreboard;

		public GameObject GetScoreboard()
		{
			return wordWhackScoreboard;
		}
	}
}
