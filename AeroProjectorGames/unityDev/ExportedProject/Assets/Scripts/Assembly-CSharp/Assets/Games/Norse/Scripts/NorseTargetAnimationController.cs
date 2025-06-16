using UnityEngine;

namespace Assets.Games.Norse.Scripts
{
	public class NorseTargetAnimationController : MonoBehaviour
	{
		[SerializeField]
		private NorseEventsSO taEvents;

		public void AnimationCompleted()
		{
			taEvents.RaiseOnTargetGrown();
		}
	}
}
