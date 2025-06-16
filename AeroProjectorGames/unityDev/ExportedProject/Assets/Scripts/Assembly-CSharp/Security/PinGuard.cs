using System;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Security
{
	public class PinGuard : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField pinInput;

		[SerializeField]
		private GameObject pinPanel;

		[SerializeField]
		private SecurityModalEventsSO securityModalEvents;

		private void OnEnable()
		{
			securityModalEvents.OnPinAuthenticationRequest.AddListener(HandlePinAuthentication);
		}

		private void OnDisable()
		{
			securityModalEvents.OnPinAuthenticationRequest.RemoveListener(HandlePinAuthentication);
		}

		private void Update()
		{
			if (pinPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				ClosePinPanel();
			}
		}

		public void ClosePinPanel()
		{
			Reset();
			pinPanel.SetActive(value: false);
		}

		private void HandlePinAuthentication(Action successfulAuthenticationAction)
		{
			EventSystem.current.SetSelectedGameObject(null);
			pinPanel.SetActive(value: true);
			Reset();
			pinInput.onValueChanged.AddListener(delegate(string value)
			{
				if (value == SettingsStore.Admin.Pin)
				{
					successfulAuthenticationAction();
					ClosePinPanel();
				}
			});
		}

		private void Reset()
		{
			pinInput.onValueChanged.RemoveAllListeners();
			pinInput.text = string.Empty;
			pinInput.Select();
		}
	}
}
