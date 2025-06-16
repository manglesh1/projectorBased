using TMPro;
using UnityEngine;

namespace VirtualKeyboard
{
	public class VirtualKeyboardButtonController : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text buttonText;

		[SerializeField]
		private bool dontRaiseKeyPressEvent;

		[SerializeField]
		private bool ignoreShiftClick;

		[Header("External References")]
		[SerializeField]
		private VirtualKeyboardEventsSO virtualKeyboardEvents;

		private void OnEnable()
		{
			virtualKeyboardEvents.OnShiftClicked += HandleOnShiftClicked;
		}

		private void OnDisable()
		{
			virtualKeyboardEvents.OnShiftClicked -= HandleOnShiftClicked;
		}

		private void HandleOnShiftClicked(bool isUppercase)
		{
			if (!ignoreShiftClick)
			{
				if (isUppercase)
				{
					buttonText.text = buttonText.text.ToUpper();
				}
				else
				{
					buttonText.text = buttonText.text.ToLower();
				}
			}
		}

		public void HandleVirtualKeyClick()
		{
			if (!dontRaiseKeyPressEvent)
			{
				virtualKeyboardEvents.RaiseKeyClicked(buttonText.text);
			}
		}
	}
}
