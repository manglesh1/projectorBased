using System.Collections;
using UnityEngine;

namespace Helpers
{
	public class AnimationHelper
	{
		public IEnumerator LerpLocalAnimation(RectTransform rectTransform, Vector3 endScale, Vector3 endLocalPosition, Quaternion endRotation, float duration, float animationCulling)
		{
			float currentTime = 0f;
			while (currentTime < duration - animationCulling)
			{
				rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, endScale, currentTime / duration);
				rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, endLocalPosition, currentTime / duration);
				rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, endRotation, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
			rectTransform.localScale = endScale;
			rectTransform.localPosition = endLocalPosition;
			rectTransform.rotation = endRotation;
		}

		public IEnumerator LerpWorldAnimation(RectTransform rectTransform, Vector3 endScale, Vector3 endWorldPosition, Quaternion endRotation, float duration, float animationCulling)
		{
			float currentTime = 0f;
			while (currentTime < duration - animationCulling)
			{
				rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, endScale, currentTime / duration);
				rectTransform.position = Vector3.Lerp(rectTransform.position, endWorldPosition, currentTime / duration);
				rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, endRotation, currentTime / duration);
				currentTime += Time.deltaTime;
				yield return null;
			}
			rectTransform.localScale = endScale;
			rectTransform.position = endWorldPosition;
			rectTransform.rotation = endRotation;
		}
	}
}
