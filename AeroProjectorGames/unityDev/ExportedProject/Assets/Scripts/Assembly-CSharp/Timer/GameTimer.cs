using System.Collections.Generic;
using System.Text;
using API;
using Admin.Demo;
using Admin_Panel.Events;
using Settings;
using TMPro;
using Timer.Admin;
using UnityEngine;
using UnityEngine.UI;
using VirtualKeyboard;

namespace Timer
{
	public class GameTimer : MonoBehaviour
	{
		private const int HOUR_IN_MINTUES = 60;

		private const int SECONDS_IN_MINTUES = 60;

		private const string PAUSED_TEXT = "PAUSED";

		private const string PAUSE_BUTTON_DEFAULT_TEXT = "Pause";

		private const string PAUSE_BUTTON_PAUSED_TEXT = "Start";

		private const string START_TIMER_TEXT = "START TIMER";

		private const string TIME_REMAINING_TEXT = "TIME REMAINING";

		private const string TIMER_START_TIME_TEXT = "00:00";

		private readonly Color TIMER_PAUSED_COLOR = Color.yellow;

		private readonly Color TIMER_STANDARD_COLOR = Color.white;

		[SerializeField]
		private List<Button> additionalTimeButtonList;

		[SerializeField]
		private List<TextMeshProUGUI> additionalTimeButtonText;

		[SerializeField]
		private TextMeshProUGUI countdownLabelText;

		[SerializeField]
		private TextMeshProUGUI countdownText;

		[SerializeField]
		private List<Button> gameLengthButtonList;

		[SerializeField]
		private List<TextMeshProUGUI> gameLengthButtonText;

		[SerializeField]
		private TMP_InputField pinEntryInputField;

		[Header("Timer Objects to Show / Hide")]
		[SerializeField]
		private GameObject timerDisplayGroupObject;

		[SerializeField]
		private GameObject timerLabelTextObject;

		[SerializeField]
		private GameObject timerStartButtonObject;

		[SerializeField]
		private GameObject timerTimeRemainingTextObject;

		[Header("Game Length Elements to Show / Hide")]
		[SerializeField]
		private GameObject timerGameLengthButtonGroupObject;

		[SerializeField]
		private GameObject timerGameLengthButtonParentObject;

		[Header("Admin Setting Elements to Show / Hide")]
		[SerializeField]
		private GameObject additionalTimeGroupObject;

		[SerializeField]
		private GameObject adminSettingsGroupObject;

		[SerializeField]
		private GameObject timerControlsGroupObject;

		[SerializeField]
		private GameObject timerControlsPauseTimeObject;

		[SerializeField]
		private TextMeshProUGUI timerControlsPauseTimeText;

		[SerializeField]
		private GameObject timerControlsResetTimeObject;

		[Header("Pin Entry Elements to Show / Hide")]
		[SerializeField]
		private GameObject pinEntry;

		[Header("Start Timer Warning Elements to Show / Hide")]
		[SerializeField]
		private GameObject startTimerWarningButtonObject;

		[SerializeField]
		private GameObject startTimerWarningGroupObject;

		[SerializeField]
		private GameObject startTimerWarningTextObject;

		[Header("External References")]
		[SerializeField]
		private AdminEventsSO adminEvents;

		[SerializeField]
		private DemoSO demoState;

		[SerializeField]
		private AdminTimerPinController timerPinController;

		[SerializeField]
		private AxcitementApiHandler api;

		[SerializeField]
		private SafetyVideoPlayer safetyVideoPlayer;

		[SerializeField]
		private VirtualKeyboardEventsSO virtualKeyboardEvents;

		[Header("Timer Refs")]
		[SerializeField]
		private SettingsEventsSO settingsEvents;

		[SerializeField]
		private TimerEventsSO timerEvents;

		[SerializeField]
		private TimerStateSO timerState;

		private SafetyVideoSettings _safetyVideoSettings;

		private void OnDisable()
		{
			adminEvents.OnAdminPanelClosed -= HideExtraSettings;
			adminEvents.OnAdminPanelOpened -= ShowExtraSettings;
			settingsEvents.OnSettingsReloaded -= ResetTimeIncrements;
			timerEvents.OnDisabledTimer -= DisableTimer;
			timerEvents.OnTimeIncrementsUpdated -= ResetTimeIncrements;
			timerEvents.OnTimerStateChange -= HandleTimerStateChanged;
			timerEvents.OnStartTimerWarningRequest -= ShowStartTimerWarning;
		}

		private void OnEnable()
		{
			adminEvents.OnAdminPanelClosed += HideExtraSettings;
			adminEvents.OnAdminPanelOpened += ShowExtraSettings;
			settingsEvents.OnSettingsReloaded += ResetTimeIncrements;
			timerEvents.OnDisabledTimer += DisableTimer;
			timerEvents.OnTimeIncrementsUpdated += ResetTimeIncrements;
			timerEvents.OnTimerStateChange += HandleTimerStateChanged;
			timerEvents.OnStartTimerWarningRequest += ShowStartTimerWarning;
			StopAndResetTimer();
			ResetTimeIncrements();
			additionalTimeGroupObject.SetActive(value: false);
			ShowHideAdditionalTimeButtons();
			adminSettingsGroupObject.SetActive(value: true);
			timerControlsGroupObject.SetActive(value: false);
			timerControlsPauseTimeObject.SetActive(value: true);
			timerControlsResetTimeObject.SetActive(value: true);
			startTimerWarningButtonObject.SetActive(value: true);
			startTimerWarningGroupObject.SetActive(value: false);
			startTimerWarningTextObject.SetActive(value: true);
		}

		private void Update()
		{
			if (timerState.CurrentState == TimerStateEnum.Running)
			{
				countdownText.text = GetTimeRemaining();
			}
		}

		public void DisableTimer(bool isDisabled)
		{
			if (isDisabled)
			{
				timerStartButtonObject.SetActive(value: false);
				timerDisplayGroupObject.SetActive(value: false);
				countdownText.gameObject.SetActive(value: false);
				countdownLabelText.gameObject.SetActive(value: false);
				timerGameLengthButtonGroupObject.SetActive(value: false);
				ShowHideGameLengthButtons();
				ShowExtraSettings();
			}
			else
			{
				ResetTimerGui();
			}
		}

		private string GetTimeRemaining()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = Mathf.FloorToInt(timerState.TimeRemaining / 60f);
			int num2 = Mathf.FloorToInt(timerState.TimeRemaining % 60f);
			stringBuilder.Append(num.ToString("00"));
			stringBuilder.Append(":");
			stringBuilder.Append(num2.ToString("00"));
			return stringBuilder.ToString();
		}

		private void HandleAddTimeKeypadEntryCompleted(string keypadInput)
		{
			if (keypadInput == timerPinController.TimerPin)
			{
				ShowExtraSettings();
			}
		}

		private void HandleStartTimerKeypadEntryCompleted(string keypadInput)
		{
			if (keypadInput == timerPinController.TimerPin)
			{
				pinEntryInputField.text = string.Empty;
				ShowTimerGameLengths();
			}
			else
			{
				ResetCompletely();
			}
		}

		private void HandleStartTimerKeypadEntryCancelled()
		{
			ResetCompletely();
		}

		private void HideExtraSettings()
		{
			additionalTimeGroupObject.SetActive(value: false);
			ShowHideAdditionalTimeButtons();
			timerControlsGroupObject.SetActive(value: false);
		}

		private void ResetTimeIncrements()
		{
			SetGameLengthButtons();
			SetAdditionalTimeLengthButtons();
		}

		private void SendTimeToServer()
		{
			if (SettingsStore.Timer.SendTimeToDashboard && !SettingsStore.Timer.IsDisabled)
			{
				int minutesRemaining = Mathf.FloorToInt(timerState.TimeRemaining / 60f);
				StartCoroutine(api.SaveGameTime(minutesRemaining, timerState.CurrentState.ToString(), delegate
				{
				}));
			}
		}

		private void SetAdditionalTimeLengthButtons()
		{
			foreach (Button additionalTimeButton in additionalTimeButtonList)
			{
				additionalTimeButton.onClick.RemoveAllListeners();
			}
			List<int> list = new List<int>
			{
				SettingsStore.Timer.AdditionalTimeIncrement1,
				SettingsStore.Timer.AdditionalTimeIncrement2,
				SettingsStore.Timer.AdditionalTimeIncrement3
			};
			for (int i = 0; i < additionalTimeButtonList.Count; i++)
			{
				if (i < list.Count)
				{
					additionalTimeButtonText[i].text = list[i].ToString();
					int addTime = list[i];
					additionalTimeButtonList[i].onClick.AddListener(delegate
					{
						AddTimeToTimer(addTime);
					});
				}
			}
		}

		public void SetGameLengthButtons()
		{
			List<int> list = new List<int>
			{
				SettingsStore.Timer.GameLength1,
				SettingsStore.Timer.GameLength2,
				SettingsStore.Timer.GameLength3
			};
			foreach (Button gameLengthButton in gameLengthButtonList)
			{
				gameLengthButton.onClick.RemoveAllListeners();
			}
			for (int i = 0; i < gameLengthButtonText.Count; i++)
			{
				if (i < list.Count)
				{
					gameLengthButtonText[i].text = list[i].ToString();
					int timeSet = list[i];
					gameLengthButtonList[i].onClick.AddListener(delegate
					{
						StartAndSetTimer(timeSet);
					});
				}
			}
		}

		private void SetTimerFontColor(TimerStateEnum currentStateEnum)
		{
			Color color;
			switch (currentStateEnum)
			{
			default:
				color = TIMER_STANDARD_COLOR;
				break;
			case TimerStateEnum.Paused:
				color = TIMER_PAUSED_COLOR;
				break;
			}
			countdownLabelText.color = color;
			countdownText.color = color;
		}

		private void ShowExtraSettings()
		{
			if (timerState.CurrentState == TimerStateEnum.Disabled)
			{
				HideExtraSettings();
				return;
			}
			SetAdditionalTimeLengthButtons();
			additionalTimeGroupObject.SetActive(value: true);
			ShowHideAdditionalTimeButtons(isShown: true);
			timerControlsGroupObject.SetActive(value: true);
		}

		private void ShowHideAdditionalTimeButtons(bool isShown = false)
		{
			foreach (Button additionalTimeButton in additionalTimeButtonList)
			{
				additionalTimeButton.gameObject.SetActive(isShown);
			}
		}

		private void ShowHideGameLengthButtons(bool isShown = false)
		{
			foreach (Button gameLengthButton in gameLengthButtonList)
			{
				gameLengthButton.gameObject.SetActive(isShown);
			}
		}

		public void AddTimeToTimer(int addTime)
		{
			timerState.AddMinutes(addTime);
			countdownText.text = GetTimeRemaining();
			SendTimeToServer();
		}

		public void MultiDisplayShowExtraButtons()
		{
			bool flag = additionalTimeGroupObject.activeSelf || timerControlsGroupObject.activeSelf;
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				if (flag)
				{
					HideExtraSettings();
				}
				else if (timerPinController.IsTimerPinDisabled)
				{
					ShowExtraSettings();
				}
				else
				{
					virtualKeyboardEvents.RaiseVirtualKeypadEntryRequest(HandleAddTimeKeypadEntryCompleted, delegate
					{
					});
				}
			}
		}

		public void PauseTimer()
		{
			switch (timerState.CurrentState)
			{
			case TimerStateEnum.Running:
				timerState.PauseTimer();
				timerControlsPauseTimeText.text = "Start";
				countdownLabelText.text = "PAUSED";
				break;
			case TimerStateEnum.Paused:
				timerState.StarTimer();
				timerControlsPauseTimeText.text = "Pause";
				countdownLabelText.text = "TIME REMAINING";
				break;
			}
			SendTimeToServer();
		}

		public void PinEntryTextChanged(TMP_InputField pinEntryInputField)
		{
			if (pinEntryInputField.text.Trim() == timerPinController.TimerPin)
			{
				pinEntryInputField.text = string.Empty;
				ShowTimerGameLengths();
			}
		}

		public void ResetCompletely()
		{
			StopAndResetTimer();
			HideExtraSettings();
		}

		public void ShowTimerPinEntry()
		{
			if (timerPinController.IsTimerPinDisabled)
			{
				ShowTimerGameLengths();
				return;
			}
			timerStartButtonObject.SetActive(value: false);
			pinEntry.SetActive(value: true);
			pinEntryInputField.Select();
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				virtualKeyboardEvents.RaiseVirtualKeypadEntryRequest(HandleStartTimerKeypadEntryCompleted, HandleStartTimerKeypadEntryCancelled);
			}
		}

		public void ShowStartTimerWarning()
		{
			if (!demoState.DemoIsRunning && timerState.CurrentState != TimerStateEnum.Running && timerState.CurrentState != TimerStateEnum.Disabled)
			{
				startTimerWarningGroupObject.SetActive(value: true);
			}
		}

		public void ShowTimerGameLengths()
		{
			pinEntry.SetActive(value: false);
			timerStartButtonObject.SetActive(value: false);
			timerDisplayGroupObject.SetActive(value: true);
			countdownText.gameObject.SetActive(value: true);
			countdownLabelText.gameObject.SetActive(value: true);
			countdownLabelText.text = "START TIMER";
			timerGameLengthButtonGroupObject.SetActive(value: true);
			timerGameLengthButtonParentObject.SetActive(value: true);
			ShowHideGameLengthButtons(isShown: true);
		}

		public void StartAndSetTimer(int newTime)
		{
			timerState.StarTimer(newTime);
			_safetyVideoSettings = SettingsStore.SafetyVideo;
			if (!_safetyVideoSettings.IsDisabled)
			{
				safetyVideoPlayer.PlaySafetyVideo();
			}
			SendTimeToServer();
		}

		private void ShowTime()
		{
			countdownLabelText.text = "TIME REMAINING";
			timerStartButtonObject.SetActive(value: false);
			timerDisplayGroupObject.SetActive(value: true);
			timerLabelTextObject.SetActive(value: true);
			timerTimeRemainingTextObject.SetActive(value: true);
			timerGameLengthButtonGroupObject.SetActive(value: false);
			timerGameLengthButtonParentObject.SetActive(value: false);
		}

		public void StopAndResetTimer()
		{
			timerState.StopTimer();
			ResetTimerGui();
			SendTimeToServer();
		}

		private void ResetTimerGui()
		{
			countdownLabelText.text = "START TIMER";
			countdownText.text = "00:00";
			timerControlsPauseTimeText.text = "Pause";
			pinEntry.SetActive(value: false);
			timerStartButtonObject.SetActive(value: true);
			timerDisplayGroupObject.SetActive(value: false);
			countdownText.gameObject.SetActive(value: false);
			countdownLabelText.gameObject.SetActive(value: false);
			timerGameLengthButtonGroupObject.SetActive(value: false);
			ShowHideGameLengthButtons();
		}

		private void HandleTimerStateChanged(TimerStateEnum newState)
		{
			if (newState == TimerStateEnum.Disabled)
			{
				DisableTimer(isDisabled: true);
			}
			if (newState == TimerStateEnum.Running)
			{
				ShowTime();
			}
			if (newState == TimerStateEnum.Stopped)
			{
				ResetTimerGui();
			}
			SetTimerFontColor(newState);
		}
	}
}
