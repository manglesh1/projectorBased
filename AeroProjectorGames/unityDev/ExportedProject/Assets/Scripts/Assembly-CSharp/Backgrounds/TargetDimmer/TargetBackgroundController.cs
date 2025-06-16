using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Backgrounds.TargetDimmer
{
	public class TargetBackgroundController : MonoBehaviour
	{
		[SerializeField]
		private Image targetDimmerImage;

		[SerializeField]
		private BackgroundEventsSO backgroundEvents;

		private void OnDisable()
		{
			backgroundEvents.OnTargetDimmerChanged -= HandleDimmerChanged;
		}

		private void OnEnable()
		{
			HandleDimmerChanged();
			backgroundEvents.OnTargetDimmerChanged += HandleDimmerChanged;
		}

		private void HandleDimmerChanged()
		{
			targetDimmerImage.color = new Color(0f, 0f, 0f, SettingsStore.Backgrounds.TargetDimmer.Alpha);
		}
	}
}
