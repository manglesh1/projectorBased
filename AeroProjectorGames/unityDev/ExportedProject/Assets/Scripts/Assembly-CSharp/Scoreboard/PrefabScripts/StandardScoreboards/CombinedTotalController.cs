using System.Collections.Generic;
using Games.GameState;
using TMPro;
using UnityEngine;

namespace Scoreboard.PrefabScripts.StandardScoreboards
{
	public class CombinedTotalController : MonoBehaviour
	{
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private TextMeshProUGUI totalCombinedScore;

		private void Update()
		{
			int num = 0;
			foreach (KeyValuePair<string, List<int?>> roundScore in gameState.RoundScores)
			{
				foreach (int? item in roundScore.Value)
				{
					if (item.HasValue)
					{
						num += item.Value;
					}
				}
			}
			totalCombinedScore.text = num.ToString();
		}
	}
}
