using System;
using System.Collections.Generic;
using Admin_Panel.Events;
using Authentication;
using ResizingAndMoving;
using Security;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace SystemPanel
{
	public class SystemPanelController : MonoBehaviour
	{
		private const string RESET_ADMIN_PIN = "reset admin pin";

		private const string RESET_LICENSE = "reset license";

		private const string RESET_SIZE_AND_POSITIONS = "reset size and positions";

		private const string TEMPORARY_OVERRIDE_LICENSE = "temporary override license";

		private const string VALID_PIN = "9119";

		private int _currentCode;

		private Action _actionToInvoke;

		[SerializeField]
		private TMP_InputField pinField;

		[SerializeField]
		private SystemPanelStateSO systemPanelState;

		[SerializeField]
		private GameObject pinPanel;

		[SerializeField]
		private GameObject systemPanel;

		[SerializeField]
		private GameObject confirmationPanel;

		[Header("Authentication Events")]
		[SerializeField]
		private AuthenticationEventsSO authenticationEvents;

		[Header("Size and Position States")]
		[SerializeField]
		private List<SizeAndPositionStateSO> sizeAndPositionStates;

		[Space]
		[Header("Security Guard Authentication")]
		[SerializeField]
		private SecurityModalEventsSO securityModalEvents;

		[Space]
		[Header("Admin Events")]
		[SerializeField]
		private AdminEventsSO adminEvents;

		public void OnEnable()
		{
			SystemPanelStateSO systemPanelStateSO = systemPanelState;
			systemPanelStateSO.OnToggle = (UnityAction)Delegate.Combine(systemPanelStateSO.OnToggle, new UnityAction(SetPanelVisibility));
		}

		public void OnDisable()
		{
			SystemPanelStateSO systemPanelStateSO = systemPanelState;
			systemPanelStateSO.OnToggle = (UnityAction)Delegate.Remove(systemPanelStateSO.OnToggle, new UnityAction(SetPanelVisibility));
		}

		public void Update()
		{
			if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.eKey.wasPressedThisFrame)
			{
				systemPanelState.RaiseToggle();
			}
		}

		public void CancelAction()
		{
			_actionToInvoke = null;
			confirmationPanel.SetActive(value: false);
		}

		public void CheckPin()
		{
			if (pinField.text == "9119")
			{
				systemPanelState.visibilityState = SystemPanelVisibilityState.PanelOpen;
				SetPanelVisibility();
			}
		}

		public void ConfirmAction()
		{
			_actionToInvoke?.Invoke();
			ClosePanel();
		}

		public void ClosePanel()
		{
			systemPanelState.visibilityState = SystemPanelVisibilityState.Closed;
			SetPanelVisibility();
		}

		public void ShowConfirmation(string action)
		{
			switch (action)
			{
			case "temporary override license":
				_actionToInvoke = ConfirmTemporaryOverride;
				break;
			case "reset size and positions":
				_actionToInvoke = ResetSizeAndPositions;
				break;
			case "reset license":
				_actionToInvoke = ResetLicense;
				break;
			case "reset admin pin":
				_actionToInvoke = ResetAdminPin;
				break;
			}
			if (_actionToInvoke != null)
			{
				ShowConfirmationPanel();
			}
		}

		private void ConfirmTemporaryOverride()
		{
			authenticationEvents.RaiseTemporaryOverride();
			ClosePanel();
		}

		private void ResetAdminPin()
		{
			SettingsStore.Admin.Pin = "1234";
			SettingsStore.Admin.Save();
			adminEvents.RaiseAdminPinReset();
		}

		private void ResetLicense()
		{
			authenticationEvents.RaiseResetLicense();
		}

		private void ResetSizeAndPositions()
		{
			sizeAndPositionStates.ForEach(delegate(SizeAndPositionStateSO spState)
			{
				spState.RaiseReset();
			});
		}

		private void SetPanelVisibility()
		{
			switch (systemPanelState.visibilityState)
			{
			case SystemPanelVisibilityState.PinEntry:
				confirmationPanel.SetActive(value: false);
				systemPanel.SetActive(value: false);
				securityModalEvents.RaiseSupportCodeModalRequest(delegate
				{
					systemPanelState.visibilityState = SystemPanelVisibilityState.PanelOpen;
					SetPanelVisibility();
				});
				break;
			case SystemPanelVisibilityState.PanelOpen:
				EventSystem.current.SetSelectedGameObject(null);
				confirmationPanel.SetActive(value: false);
				pinField.text = null;
				pinPanel.SetActive(value: false);
				systemPanel.SetActive(value: true);
				break;
			case SystemPanelVisibilityState.Closed:
				EventSystem.current.SetSelectedGameObject(null);
				confirmationPanel.SetActive(value: false);
				pinField.text = null;
				pinPanel.SetActive(value: false);
				systemPanel.SetActive(value: false);
				break;
			}
		}

		private void ShowConfirmationPanel()
		{
			confirmationPanel.SetActive(value: true);
		}
	}
}
