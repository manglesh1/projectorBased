using Settings;
using UnityEngine;

namespace Platform
{
	public class WindowsPlatformManager : MonoBehaviour
	{
		private const int MAX_COUNTER = 60;

		private int _resolutionChangingCounter;

		private void Update()
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				if (!Screen.fullScreen || Screen.fullScreenMode != FullScreenMode.FullScreenWindow)
				{
					Debug.Log("Setting fullscreen");
					Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
					Screen.fullScreen = true;
				}
				if (SettingsStore.GlobalViewSettings.PortraitMode)
				{
					PortraitModeCheck();
				}
			}
		}

		private void PortraitModeCheck()
		{
			if (_resolutionChangingCounter > 5)
			{
				_resolutionChangingCounter++;
				if (Screen.currentResolution.height == Display.displays[0].systemHeight || _resolutionChangingCounter > 60)
				{
					_resolutionChangingCounter = 0;
				}
			}
			if (Screen.currentResolution.height < Display.displays[0].systemHeight && _resolutionChangingCounter <= 5)
			{
				_resolutionChangingCounter++;
				if (_resolutionChangingCounter > 5)
				{
					Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemHeight, fullscreen: true);
				}
			}
		}
	}
}
