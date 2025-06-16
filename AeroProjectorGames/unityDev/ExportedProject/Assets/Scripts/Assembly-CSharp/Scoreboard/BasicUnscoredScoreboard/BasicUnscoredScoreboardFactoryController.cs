using UnityEngine;

namespace Scoreboard.BasicUnscoredScoreboard
{
	public class BasicUnscoredScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("Basic Scoreboard Prefab")]
		[SerializeField]
		private GameObject basicScoreboard;

		public GameObject GetScoreboard()
		{
			return basicScoreboard;
		}
	}
}
