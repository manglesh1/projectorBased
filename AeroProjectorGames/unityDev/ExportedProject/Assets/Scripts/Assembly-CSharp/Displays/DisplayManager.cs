using Settings;
using UnityEngine;

namespace Displays
{
	public class DisplayManager : MonoBehaviour
	{
		private const int DISPLAY_2_INDEX = 1;

		private const int MULTI_DISPLAY_COUNT = 2;

		[SerializeField]
		private DisplayStateSO displayState;

		private void Awake()
		{
			displayState.SetMultiDisplayEnabled(enabled: false);
			if (SettingsStore.Interaction.MultiDisplayEnabled && Display.displays.Length == 2)
			{
				Display.displays[1].Activate();
				displayState.SetMultiDisplayEnabled(enabled: true);
			}
		}
	}
}
