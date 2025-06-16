using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Animations.Extensions
{
	public static class LERPExtensions
	{
		public static IEnumerator LerpColor(this Image image, Color startingColor, Color endingColor, float duration)
		{
			float currentTime = 0f;
			while (currentTime < duration)
			{
				image.color = Color.Lerp(startingColor, endingColor, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
		}

		public static IEnumerator LerpColor(this SpriteRenderer renderer, Color startingColor, Color endingColor, float duration)
		{
			float currentTime = 0f;
			while (currentTime < duration)
			{
				renderer.color = Color.Lerp(startingColor, endingColor, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
		}
	}
}
