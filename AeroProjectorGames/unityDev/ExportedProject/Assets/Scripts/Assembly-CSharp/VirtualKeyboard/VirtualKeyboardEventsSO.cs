using System;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualKeyboard
{
	[CreateAssetMenu(menuName = "Virtual Keyboard/Keyboard Events")]
	public class VirtualKeyboardEventsSO : ScriptableObject
	{
		public event UnityAction<string> OnCompletedEntry;

		public event UnityAction<string> OnKeyClicked;

		public event UnityAction<bool> OnShiftClicked;

		public event UnityAction OnVirtualKeyboardEntryRequest;

		public event UnityAction<Action<string>, Action> OnVirtualKeypadEntryRequest;

		public void RaiseCompletedEntry(string text)
		{
			this.OnCompletedEntry?.Invoke(text);
		}

		public void RaiseKeyClicked(string keyCharacter)
		{
			this.OnKeyClicked?.Invoke(keyCharacter);
		}

		public void RaiseShiftClicked(bool isUpperCase)
		{
			this.OnShiftClicked?.Invoke(isUpperCase);
		}

		public void RaiseVirtualKeyboardEntryRequest()
		{
			this.OnVirtualKeyboardEntryRequest?.Invoke();
		}

		public void RaiseVirtualKeypadEntryRequest(Action<string> onComplete, Action onCancel)
		{
			this.OnVirtualKeypadEntryRequest?.Invoke(onComplete, onCancel);
		}
	}
}
