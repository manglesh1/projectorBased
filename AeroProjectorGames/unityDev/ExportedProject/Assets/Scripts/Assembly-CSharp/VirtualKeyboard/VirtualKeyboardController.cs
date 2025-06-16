using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace VirtualKeyboard
{
	public class VirtualKeyboardController : MonoBehaviour
	{
		private bool _isUppercase;

		[Header("Virtual Keyboard Primary Panel")]
		[SerializeField]
		private GameObject virtualKeyboardPrimaryPanel;

		[Header("Keyboard Objects")]
		[SerializeField]
		private GameObject textKeyboardObject;

		[SerializeField]
		private GameObject symbolKeyboardObject;

		[Header("Input Field")]
		[SerializeField]
		private TMP_InputField textInput;

		[FormerlySerializedAs("VirtualKeyboardEvents")]
		[Header("External References")]
		[SerializeField]
		private VirtualKeyboardEventsSO virtualKeyboardEvents;

		private void OnDisable()
		{
			virtualKeyboardEvents.OnKeyClicked -= HandleKeyClicked;
			virtualKeyboardEvents.OnVirtualKeyboardEntryRequest -= HandleEntryRequest;
		}

		private void OnEnable()
		{
			textKeyboardObject.SetActive(value: true);
			symbolKeyboardObject.SetActive(value: false);
			virtualKeyboardEvents.RaiseShiftClicked(isUpperCase: false);
			virtualKeyboardEvents.OnKeyClicked += HandleKeyClicked;
			virtualKeyboardEvents.OnVirtualKeyboardEntryRequest += HandleEntryRequest;
		}

		private void HandleEntryRequest()
		{
			virtualKeyboardPrimaryPanel.SetActive(value: true);
		}

		private void HandleKeyClicked(string keyPressed)
		{
			switch (keyPressed.ToLower())
			{
			case "space":
				textInput.text += " ";
				break;
			default:
				textInput.text += keyPressed;
				break;
			case "delete":
				if (textInput.text.Length > 0)
				{
					textInput.text = textInput.text.Substring(0, textInput.text.Length - 1);
				}
				break;
			case "done":
				virtualKeyboardEvents.RaiseCompletedEntry(textInput.text);
				textInput.text = string.Empty;
				virtualKeyboardPrimaryPanel.SetActive(value: false);
				break;
			}
		}

		public void ShiftClicked()
		{
			_isUppercase = !_isUppercase;
			virtualKeyboardEvents.RaiseShiftClicked(_isUppercase);
		}
	}
}
