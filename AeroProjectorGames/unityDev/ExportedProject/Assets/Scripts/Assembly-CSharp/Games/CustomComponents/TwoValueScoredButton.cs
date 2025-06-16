using UnityEngine;
using UnityEngine.UI;

namespace Games.CustomComponents
{
	[AddComponentMenu("Games/TwoValueScoredButton")]
	public class TwoValueScoredButton : Button
	{
		[SerializeField]
		private int secondaryScore;

		[SerializeField]
		private int score;

		public int SecondaryScore
		{
			get
			{
				return secondaryScore;
			}
			set
			{
				secondaryScore = value;
			}
		}

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
