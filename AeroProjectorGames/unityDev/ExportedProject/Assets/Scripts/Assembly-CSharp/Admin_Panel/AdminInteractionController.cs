using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Admin_Panel
{
	public class AdminInteractionController : MonoBehaviour
	{
		[SerializeField]
		private Toggle multiDisplayToggle;

		[SerializeField]
		private Toggle virtualKeyboardToggle;

		private void OnEnable()
		{
			multiDisplayToggle.isOn = SettingsStore.Interaction.MultiDisplayEnabled;
			virtualKeyboardToggle.isOn = SettingsStore.Interaction.VirtualKeyboardEnabled;
		}

		public void ToggleMultiDisplay()
		{
			SettingsStore.Interaction.MultiDisplayEnabled = multiDisplayToggle.isOn;
			SettingsStore.Interaction.Save();
		}

		public void ToggleVirtualKeyboard()
		{
			SettingsStore.Interaction.VirtualKeyboardEnabled = virtualKeyboardToggle.isOn;
			SettingsStore.Interaction.Save();
		}
	}
}
