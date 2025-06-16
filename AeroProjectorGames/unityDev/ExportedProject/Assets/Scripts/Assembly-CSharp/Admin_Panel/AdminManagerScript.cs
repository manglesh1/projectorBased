using System;
using Admin_Panel.Events;
using Authentication;
using Detection;
using Games;
using Helpers;
using Network;
using Security;
using Settings;
using TMPro;
using Timer;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Admin_Panel
{
	public class AdminManagerScript : MonoBehaviour
	{
		private const string NO_LICENSE_KEY = "No License Key Found";

		private AdminPanelStates _currentAdminState;

		private ColorBlock _defaultButtonColor;

		private ColorBlock _selectedButtonColor;

		[Header("Admin Panel Elements")]
		[SerializeField]
		private TextMeshProUGUI adminPanelLaneNumberTextMesh;

		[SerializeField]
		private TextMeshProUGUI adminPanelLicenseNumberTextMesh;

		[SerializeField]
		private GameObject adminPanelObject;

		[SerializeField]
		private TextMeshProUGUI adminPanelVersionNumberTextMesh;

		[SerializeField]
		private Button backgroundsSettingsButton;

		[SerializeField]
		private Button closeButton;

		[SerializeField]
		private Button detectionSettingsButton;

		[SerializeField]
		private Button editSizeAndPositionButton;

		[SerializeField]
		private Button settingsButton;

		[SerializeField]
		private Button updateButton;

		[Header("Edit Size and Position Elements")]
		[SerializeField]
		private Button editSizeAndPositionCloseButton;

		[SerializeField]
		private GameObject editSizeAndPositionObject;

		[Header("Settings - Elements")]
		[SerializeField]
		private Button coreSettingsButton;

		[SerializeField]
		private Button doneSettingsButton;

		[SerializeField]
		private Button gameAccessButton;

		[SerializeField]
		private Button safetyVideoSettingsButton;

		[SerializeField]
		private GameObject settingsBodyContainerGroupObject;

		[SerializeField]
		private GameObject settingsMainGroupObject;

		[SerializeField]
		private Button standardTargetSettingsButton;

		[SerializeField]
		private Button timerSettingsButton;

		[Header("Settings - Core Elements")]
		[SerializeField]
		private GameObject coreSettingsGroupObject;

		[Header("Settings - Game Access Elements")]
		[SerializeField]
		private GameObject gameAccessGroupObject;

		[Header("Settings - Safety Video Elements")]
		[SerializeField]
		private GameObject safetyVideoGroupObject;

		[Header("Settings - Standard Target Elements")]
		[SerializeField]
		private GameObject standarTargetGroupObject;

		[Header("Settings - Timer Elements")]
		[SerializeField]
		private GameObject timerGroupObject;

		[Header("External References")]
		[SerializeField]
		private AuthenticationStateSO authenticationStateInfo;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private TimerEventsSO timerEvents;

		[Header("Events")]
		[SerializeField]
		private AdminEventsSO adminEvents;

		[SerializeField]
		private ViewEventsSO backgroundViewEvents;

		[SerializeField]
		private DetectionUIEventsSO detectionUIEvents;

		[SerializeField]
		private NetworkStatusEventsSO networkEvents;

		[SerializeField]
		private SecurityModalEventsSO securityEvents;

		[Obsolete("This is being replaced by AdminEventsSO.")]
		[HideInInspector]
		public UnityEvent<bool> AdminPanelOpened = new UnityEvent<bool>();

		private SettingsState _currentSettingState;

		public AdminPanelStates CurrentAdminState => _currentAdminState;

		public SettingsState CurrentSettingsState
		{
			get
			{
				return _currentSettingState;
			}
			set
			{
				_currentSettingState = value;
				DisplayProperSettingsWindow(value);
			}
		}

		private void Awake()
		{
			SetSettingsButtonListeners();
			_defaultButtonColor = coreSettingsButton.colors;
			_selectedButtonColor = coreSettingsButton.colors;
			_selectedButtonColor.normalColor = _defaultButtonColor.pressedColor;
		}

		private void OnEnable()
		{
			DisplayProperAdminWindow(AdminPanelStates.Closed);
			CurrentSettingsState = SettingsState.Closed;
		}

		private void Update()
		{
			if (Keyboard.current.ctrlKey.isPressed && Keyboard.current.mKey.wasPressedThisFrame)
			{
				if (_currentAdminState == AdminPanelStates.Closed)
				{
					DisplayProperAdminWindow(AdminPanelStates.PinEntry);
				}
				else
				{
					DisplayProperAdminWindow(AdminPanelStates.Closed);
				}
			}
		}

		private void SetSettingsButtonListeners()
		{
			settingsButton.onClick.RemoveAllListeners();
			backgroundsSettingsButton.onClick.RemoveAllListeners();
			closeButton.onClick.RemoveAllListeners();
			detectionSettingsButton.onClick.RemoveAllListeners();
			coreSettingsButton.onClick.RemoveAllListeners();
			doneSettingsButton.onClick.RemoveAllListeners();
			gameAccessButton.onClick.RemoveAllListeners();
			safetyVideoSettingsButton.onClick.RemoveAllListeners();
			standardTargetSettingsButton.onClick.RemoveAllListeners();
			timerSettingsButton.onClick.RemoveAllListeners();
			editSizeAndPositionCloseButton.onClick.RemoveAllListeners();
			editSizeAndPositionButton.onClick.RemoveAllListeners();
			updateButton.onClick.RemoveAllListeners();
			settingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.Core);
			});
			editSizeAndPositionButton.onClick.AddListener(delegate
			{
				DisplayProperAdminWindow(AdminPanelStates.EditSizeAndPosition);
			});
			updateButton.onClick.AddListener(delegate
			{
				networkEvents.RaiseNetworkGuard(UpdateHelper.RunLauncher);
			});
			editSizeAndPositionCloseButton.onClick.AddListener(delegate
			{
				DisplayProperAdminWindow(AdminPanelStates.Closed);
			});
			backgroundsSettingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.Backgrounds);
			});
			closeButton.onClick.AddListener(delegate
			{
				DisplayProperAdminWindow(AdminPanelStates.Closed);
			});
			detectionSettingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.Detection);
			});
			coreSettingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.Core);
			});
			doneSettingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.Closed);
			});
			gameAccessButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.GameAccess);
			});
			safetyVideoSettingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.SafteyVideo);
			});
			standardTargetSettingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.StandardTarget);
			});
			timerSettingsButton.onClick.AddListener(delegate
			{
				ChangeSettingsWindow(SettingsState.Timer);
			});
		}

		public void DisplayProperAdminWindow(AdminPanelStates newState)
		{
			switch (newState)
			{
			case AdminPanelStates.Closed:
				_currentAdminState = AdminPanelStates.Closed;
				adminEvents.RaiseAdminPanelClosed();
				editSizeAndPositionObject.SetActive(value: false);
				adminPanelObject.SetActive(value: false);
				AdminPanelOpened.Invoke(arg0: false);
				DisplayProperSettingsWindow(SettingsState.Closed);
				break;
			case AdminPanelStates.PinEntry:
				_currentAdminState = AdminPanelStates.PinEntry;
				adminPanelObject.SetActive(value: false);
				securityEvents.RaisePinAuthenticationRequest(delegate
				{
					DisplayProperAdminWindow(AdminPanelStates.AdminPanel);
				});
				break;
			case AdminPanelStates.AdminPanel:
				adminEvents.RaiseAdminPanelOpened();
				_currentAdminState = AdminPanelStates.AdminPanel;
				adminPanelObject.SetActive(value: true);
				AdminPanelOpened.Invoke(arg0: true);
				adminPanelLicenseNumberTextMesh.text = ((authenticationStateInfo.LicenseKey != string.Empty) ? authenticationStateInfo.LicenseKey : "No License Key Found");
				SetLaneAndVersionText();
				break;
			case AdminPanelStates.EditSizeAndPosition:
				_currentAdminState = AdminPanelStates.EditSizeAndPosition;
				adminPanelObject.SetActive(value: false);
				editSizeAndPositionObject.SetActive(value: true);
				break;
			default:
				Debug.Log("Put a default action here");
				break;
			}
		}

		private void DisplayProperSettingsWindow(SettingsState newState)
		{
			settingsBodyContainerGroupObject.SetActive(value: false);
			settingsMainGroupObject.SetActive(value: false);
			coreSettingsGroupObject.SetActive(value: false);
			gameAccessGroupObject.SetActive(value: false);
			gameAccessGroupObject.SetActive(value: false);
			safetyVideoGroupObject.SetActive(value: false);
			standarTargetGroupObject.SetActive(value: false);
			timerGroupObject.SetActive(value: false);
			coreSettingsButton.colors = _defaultButtonColor;
			gameAccessButton.colors = _defaultButtonColor;
			safetyVideoSettingsButton.colors = _defaultButtonColor;
			standardTargetSettingsButton.colors = _defaultButtonColor;
			timerSettingsButton.colors = _defaultButtonColor;
			switch (newState)
			{
			case SettingsState.Backgrounds:
				backgroundViewEvents.OpenView();
				DisplayProperAdminWindow(AdminPanelStates.Closed);
				break;
			case SettingsState.Closed:
				break;
			case SettingsState.Core:
				settingsBodyContainerGroupObject.SetActive(value: true);
				settingsMainGroupObject.SetActive(value: true);
				coreSettingsGroupObject.SetActive(value: true);
				coreSettingsButton.colors = _selectedButtonColor;
				break;
			case SettingsState.Detection:
				detectionUIEvents.OpenDetectionUI();
				DisplayProperAdminWindow(AdminPanelStates.Closed);
				break;
			case SettingsState.GameAccess:
				settingsBodyContainerGroupObject.SetActive(value: true);
				settingsMainGroupObject.SetActive(value: true);
				gameAccessGroupObject.SetActive(value: true);
				gameAccessButton.colors = _selectedButtonColor;
				break;
			case SettingsState.Timer:
				settingsBodyContainerGroupObject.SetActive(value: true);
				settingsMainGroupObject.SetActive(value: true);
				timerGroupObject.SetActive(value: true);
				timerSettingsButton.colors = _selectedButtonColor;
				timerEvents.RaiseDisplayTimerSettings();
				break;
			case SettingsState.StandardTarget:
				settingsBodyContainerGroupObject.SetActive(value: true);
				settingsMainGroupObject.SetActive(value: true);
				standarTargetGroupObject.SetActive(value: true);
				standardTargetSettingsButton.colors = _selectedButtonColor;
				break;
			case SettingsState.SafteyVideo:
				settingsBodyContainerGroupObject.SetActive(value: true);
				settingsMainGroupObject.SetActive(value: true);
				safetyVideoGroupObject.SetActive(value: true);
				safetyVideoSettingsButton.colors = _selectedButtonColor;
				break;
			default:
				Debug.Log("Put a default action here");
				break;
			}
		}

		private void SetLaneAndVersionText()
		{
			adminPanelLaneNumberTextMesh.text = authenticationStateInfo.LaneNumber.ToString();
			adminPanelVersionNumberTextMesh.text = SettingsStore.Version.VersionNumber;
		}

		public void ChangeSettingsWindow(SettingsState newState)
		{
			if (newState == SettingsState.Core || newState == SettingsState.Backgrounds)
			{
				gameEvents.RaiseMainMenu();
			}
			CurrentSettingsState = newState;
		}
	}
}
