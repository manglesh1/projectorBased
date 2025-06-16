using Settings;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Displays
{
	public class DisplaySettingsController : MonoBehaviour
	{
		private GlobalViewSettings _settings;

		[SerializeField]
		private Toggle portraitModeToggle;

		private void OnEnable()
		{
			_settings = SettingsStore.GlobalViewSettings;
			portraitModeToggle.SetIsOnWithoutNotify(_settings.PortraitMode);
			portraitModeToggle.onValueChanged.AddListener(TogglePortraitMode);
		}

		private void OnDisable()
		{
			portraitModeToggle.onValueChanged.RemoveListener(TogglePortraitMode);
		}

		public void SetResolution()
		{
			Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemHeight, fullscreen: true);
		}

		private void TogglePortraitMode(bool newValue)
		{
			_settings.PortraitMode = newValue;
			_settings.Save();
		}
	}
}
