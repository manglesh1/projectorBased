using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts.UI
{
	public sealed class ProgressBar : MonoBehaviour
	{
		public RectTransform InnerProgressBar;

		[Range(0f, 1f)]
		public float Progress;

		private void Update()
		{
			InnerProgressBar.anchorMax = new Vector2(Progress, InnerProgressBar.anchorMax.y);
		}
	}
}
