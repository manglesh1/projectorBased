using UnityEngine;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackGameboardAnimationsController : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem foundAnimation;

		[SerializeField]
		private ParticleSystem notFoundAnimation;

		[SerializeField]
		private WordWhackEventsSO wordWhackEvents;

		private void OnDisable()
		{
			foundAnimation.Stop();
			notFoundAnimation.Stop();
			foundAnimation.Clear();
			notFoundAnimation.Clear();
			wordWhackEvents.OnFoundAnimation -= PlayFoundAnimation;
			wordWhackEvents.OnNotFoundAnimation -= PlayNotFoundAnimation;
			wordWhackEvents.OnStopGameboardAnimationsRequest -= StopAnimations;
		}

		private void OnEnable()
		{
			wordWhackEvents.OnFoundAnimation += PlayFoundAnimation;
			wordWhackEvents.OnNotFoundAnimation += PlayNotFoundAnimation;
			wordWhackEvents.OnStopGameboardAnimationsRequest += StopAnimations;
		}

		private void PlayNotFoundAnimation()
		{
			notFoundAnimation.Play();
		}

		private void PlayFoundAnimation()
		{
			foundAnimation.Play();
		}

		private void StopAnimations()
		{
			foundAnimation.Stop();
			notFoundAnimation.Stop();
			foundAnimation.Clear();
			notFoundAnimation.Clear();
		}
	}
}
