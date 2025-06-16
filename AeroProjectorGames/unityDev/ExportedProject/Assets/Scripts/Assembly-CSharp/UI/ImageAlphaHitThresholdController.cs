using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	public class ImageAlphaHitThresholdController : MonoBehaviour
	{
		private const float ALPHA_THRESHOLD = 0.5f;

		private void OnEnable()
		{
			GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
		}
	}
}
