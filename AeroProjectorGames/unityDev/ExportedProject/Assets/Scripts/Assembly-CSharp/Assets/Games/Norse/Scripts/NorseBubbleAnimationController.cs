using UnityEngine;

namespace Assets.Games.Norse.Scripts
{
	public class NorseBubbleAnimationController : MonoBehaviour
	{
		[SerializeField]
		private NorseEventsSO norseEvents;

		public void AnimationStarted()
		{
			norseEvents.RaiseOnBubbleAnimationStarting();
		}

		public void BurstComplete()
		{
			norseEvents.RaiseOnBubbleBurst();
		}

		public void GrowComplete()
		{
			norseEvents.RaiseOnBubbleGrown();
		}
	}
}
