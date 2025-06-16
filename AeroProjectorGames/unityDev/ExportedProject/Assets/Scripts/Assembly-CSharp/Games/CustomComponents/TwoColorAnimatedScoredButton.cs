using Components;
using UnityEngine;

namespace Games.CustomComponents
{
	[AddComponentMenu("Games/Two Color Animated ScoredButton")]
	public class TwoColorAnimatedScoredButton : ScoredButton
	{
		[SerializeField]
		private LerpImageTwoColorAnimation lerpAnimation;

		public LerpImageTwoColorAnimation TwoColorAnimator
		{
			get
			{
				return lerpAnimation;
			}
			set
			{
				lerpAnimation = value;
			}
		}
	}
}
