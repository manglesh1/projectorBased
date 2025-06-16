using UnityEngine;
using UnityEngine.UI;

namespace Games.CustomComponents
{
	[AddComponentMenu("Games/ScoredButton")]
	public class ScoredButton : Button
	{
		[SerializeField]
		private int score;

		public int Score
		{
			get
			{
				return score;
			}
			set
			{
				score = value;
			}
		}
	}
}
