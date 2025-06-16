using Games;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class RealSenseDetectionSettingsController : MonoBehaviour
	{
		[SerializeField]
		private RsFrameProvider cameraDevice;

		[SerializeField]
		private Toggle detectionEnabledToggle;

		[SerializeField]
		private Toggle objectRemovalDelayToggle;

		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnDisable()
		{
			detectionEnabledToggle.onValueChanged.RemoveListener(OnDetectionEnableToggleChanged);
			objectRemovalDelayToggle.onValueChanged.RemoveListener(OnObjectRemovalDelayToggleChanged);
		}

		private void OnEnable()
		{
			detectionEnabledToggle.SetIsOnWithoutNotify(SettingsStore.DetectionSettings.DetectionEnabled);
			objectRemovalDelayToggle.SetIsOnWithoutNotify(SettingsStore.DetectionSettings.ObjectRemovalDelay);
			detectionEnabledToggle.onValueChanged.AddListener(OnDetectionEnableToggleChanged);
			objectRemovalDelayToggle.onValueChanged.AddListener(OnObjectRemovalDelayToggleChanged);
		}

		public void OnDetectionEnableToggleChanged(bool newValue)
		{
			SettingsStore.DetectionSettings.DetectionEnabled = detectionEnabledToggle.isOn;
			SettingsStore.DetectionSettings.Save();
			gameEvents.RaiseGameLicensedListUpdated();
			cameraDevice.enabled = newValue;
		}

		private void OnObjectRemovalDelayToggleChanged(bool newValue)
		{
			SettingsStore.DetectionSettings.ObjectRemovalDelay = objectRemovalDelayToggle.isOn;
			SettingsStore.DetectionSettings.Save();
		}
	}
}
