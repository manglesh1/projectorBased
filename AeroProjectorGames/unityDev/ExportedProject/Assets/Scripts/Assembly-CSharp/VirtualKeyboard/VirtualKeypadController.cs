using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VirtualKeyboard
{
	public class VirtualKeypadController : MonoBehaviour
	{
		private string _inputText;

		private Action _cancelCallback;

		private Action<string> _completeCallback;

		[Header("External References")]
		[SerializeField]
		private VirtualKeyboardEventsSO virtualKeyboardEvents;

		[Header("Keypad GameObject")]
		[SerializeField]
		private GameObject keypadGameObject;

		private void OnDisable()
		{
			_cancelCallback = null;
			_completeCallback = null;
			_inputText = string.Empty;
			virtualKeyboardEvents.OnKeyClicked -= HandleKeyClicked;
			virtualKeyboardEvents.OnVirtualKeypadEntryRequest -= HandleKeypadEntryRequest;
		}

		private void OnEnable()
		{
			_cancelCallback = null;
			_completeCallback = null;
			_inputText = string.Empty;
			virtualKeyboardEvents.OnKeyClicked += HandleKeyClicked;
			virtualKeyboardEvents.OnVirtualKeypadEntryRequest += HandleKeypadEntryRequest;
		}

		private void HandleKeypadEntryRequest(Action<string> onCompleteCallback, Action onCancelCallback)
		{
			keypadGameObject.SetActive(value: true);
			_cancelCallback = onCancelCallback;
			_completeCallback = onCompleteCallback;
			EventSystem.current.SetSelectedGameObject(keypadGameObject);
		}

		private void HandleKeyClicked(string keyPressed)
		{
			string text = keyPressed.ToLower();
			if (text == "enter")
			{
				EventSystem.current.SetSelectedGameObject(null);
				_completeCallback?.Invoke(_inputText);
				_inputText = string.Empty;
				keypadGameObject.SetActive(value: false);
			}
			else if (!(text == "cancel"))
			{
				_inputText += keyPressed;
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
				_cancelCallback?.Invoke();
				_inputText = string.Empty;
				keypadGameObject.SetActive(value: false);
			}
		}
	}
}
