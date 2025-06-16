using System.Collections;
using Animations.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Components
{
	public class LerpImageTwoColorAnimation : MonoBehaviour
	{
		[SerializeField]
		private Image image;

		[SerializeField]
		private Color accentFlashColor;

		[SerializeField]
		private Color mainColor;

		[SerializeField]
		private Color transitionColor;

		public UnityEvent OnAnimationFinished = new UnityEvent();

		public UnityEvent OnAnimationStarting = new UnityEvent();

		public void Animate()
		{
			RaiseAnimationStarting();
			StartCoroutine(RunAnimation());
		}

		private IEnumerator RunAnimation()
		{
			yield return image.LerpColor(mainColor, transitionColor, 0.08f);
			yield return image.LerpColor(accentFlashColor, transitionColor, 0.06f);
			yield return image.LerpColor(transitionColor, mainColor, 0.2f);
			image.color = mainColor;
			RaiseAnimationFinished();
		}

		private void RaiseAnimationFinished()
		{
			OnAnimationFinished?.Invoke();
		}

		private void RaiseAnimationStarting()
		{
			OnAnimationStarting?.Invoke();
		}
	}
}
