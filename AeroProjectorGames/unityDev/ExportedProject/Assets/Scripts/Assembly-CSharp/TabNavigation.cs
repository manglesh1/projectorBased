using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TabNavigation : MonoBehaviour
{
	[SerializeField]
	private Selectable[] selectables;

	[SerializeField]
	private InputAction tabAction;

	private void Navigate(InputAction.CallbackContext _)
	{
		GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		Selectable value = (currentSelectedGameObject ? currentSelectedGameObject.GetComponent<Selectable>() : null);
		int num = Array.IndexOf(selectables, value);
		int num2 = (Keyboard.current.shiftKey.isPressed ? (num - 1) : (num + 1));
		if (num2 < 0)
		{
			num2 = selectables.Length - 1;
		}
		if (num2 >= selectables.Length)
		{
			num2 = 0;
		}
		selectables[num2].Select();
	}

	private void OnEnable()
	{
		tabAction.performed += Navigate;
		tabAction.Enable();
	}

	private void OnDisable()
	{
		tabAction.performed -= Navigate;
		tabAction.Disable();
	}
}
