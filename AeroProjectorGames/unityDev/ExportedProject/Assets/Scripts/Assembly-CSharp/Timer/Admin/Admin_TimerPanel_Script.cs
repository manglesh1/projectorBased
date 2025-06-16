using System.Collections.Generic;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Admin
{
	public class Admin_TimerPanel_Script : MonoBehaviour
	{
		private const int MIN_TIMER_VALUE = 1;

		private const int MAX_TIMER_VALUE = 240;

		private bool _isDisabled;

		[SerializeField]
		private Toggle disableTimerToggle;

		[Header("Settings Elements")]
		private TimerSettings _timerSettings;

		[Header("Timer Length Elements")]
		[SerializeField]
		private Toggle sendTimerToDashboardToggle;

		[SerializeField]
		private List<TMP_InputField> adminInputAdditionalTimeIncrementsInputField;

		[SerializeField]
		private List<TMP_InputField> adminInputGameTimerLengthInputField;

		[SerializeField]
		private GameObject timerLengthGroupObject;

		[SerializeField]
		private TMP_Text timerLengthMessageText;

		[Header("External References")]
		[SerializeField]
		private SettingsEventsSO settingsEvents;

		[SerializeField]
		private TimerEventsSO timerEvents;

		[SerializeField]
		private TimerStateSO timerState;

		public List<int> CurrentAdditionalTimeIncrements { get; set; } = new List<int>();

		public List<int> CurrentGameTimerLengths { get; set; } = new List<int>();

		public int MaxTimerValue => 240;

		private void OnDisable()
		{
			settingsEvents.OnSettingsReloaded -= GetTimerSettings;
			timerEvents.OnDisplayTimerSettings -= DisplayCurrentValues;
		}

		private void OnEnable()
		{
			settingsEvents.OnSettingsReloaded += GetTimerSettings;
			timerEvents.OnDisplayTimerSettings += DisplayCurrentValues;
		}

		private void Start()
		{
			GetTimerSettings();
			if (_isDisabled)
			{
				timerState.DisableTimer();
			}
		}

		private List<int> CheckTimerMinMaxValues(List<int> checkList)
		{
			for (int i = 0; i < checkList.Count; i++)
			{
				if (checkList[i] < 1)
				{
					checkList[i] = 1;
					DisplayCurrentValues();
				}
				else if (checkList[i] > 240)
				{
					checkList[i] = 240;
					DisplayCurrentValues();
				}
			}
			Debug.Log("Show user a value was changed and the min and max values");
			return checkList;
		}

		private void DisplayCurrentValues()
		{
			sendTimerToDashboardToggle.isOn = SettingsStore.Timer.SendTimeToDashboard;
			disableTimerToggle.isOn = timerState.CurrentState == TimerStateEnum.Disabled;
			for (int i = 0; i < adminInputGameTimerLengthInputField.Count; i++)
			{
				if (i < CurrentGameTimerLengths.Count)
				{
					adminInputGameTimerLengthInputField[i].text = CurrentGameTimerLengths[i].ToString();
				}
			}
			for (int j = 0; j < adminInputAdditionalTimeIncrementsInputField.Count; j++)
			{
				if (j < CurrentAdditionalTimeIncrements.Count)
				{
					adminInputAdditionalTimeIncrementsInputField[j].text = CurrentAdditionalTimeIncrements[j].ToString();
				}
			}
			timerLengthGroupObject.SetActive(!disableTimerToggle.isOn);
			timerLengthMessageText.text = string.Empty;
		}

		private void GetTimerSettings()
		{
			_timerSettings = SettingsStore.Timer;
			CurrentGameTimerLengths.Clear();
			CurrentGameTimerLengths.Add(_timerSettings.GameLength1);
			CurrentGameTimerLengths.Add(_timerSettings.GameLength2);
			CurrentGameTimerLengths.Add(_timerSettings.GameLength3);
			_isDisabled = _timerSettings.IsDisabled;
			CurrentAdditionalTimeIncrements.Clear();
			CurrentAdditionalTimeIncrements.Add(_timerSettings.AdditionalTimeIncrement1);
			CurrentAdditionalTimeIncrements.Add(_timerSettings.AdditionalTimeIncrement2);
			CurrentAdditionalTimeIncrements.Add(_timerSettings.AdditionalTimeIncrement3);
		}

		private void SaveTimerSettings()
		{
			_timerSettings.GameLength1 = CurrentGameTimerLengths[0];
			_timerSettings.GameLength2 = CurrentGameTimerLengths[1];
			_timerSettings.GameLength3 = CurrentGameTimerLengths[2];
			_timerSettings.IsDisabled = _isDisabled;
			_timerSettings.AdditionalTimeIncrement1 = CurrentAdditionalTimeIncrements[0];
			_timerSettings.AdditionalTimeIncrement2 = CurrentAdditionalTimeIncrements[1];
			_timerSettings.AdditionalTimeIncrement3 = CurrentAdditionalTimeIncrements[2];
			_timerSettings.Save();
		}

		public void DisableTimer(Toggle disableToggle)
		{
			if (disableToggle.isOn)
			{
				_isDisabled = true;
				timerState.DisableTimer();
				timerLengthGroupObject.SetActive(value: false);
			}
			else
			{
				_isDisabled = false;
				timerState.EnableTimer();
				timerLengthGroupObject.SetActive(value: true);
			}
			SaveTimerSettings();
		}

		public void SendTimeToDashboardToggle(Toggle sendTimeToDashboardToggle)
		{
			_timerSettings.SendTimeToDashboard = sendTimeToDashboardToggle.isOn;
			SaveTimerSettings();
		}

		public void UpdateTimerSettings()
		{
			try
			{
				CurrentGameTimerLengths[0] = int.Parse(adminInputGameTimerLengthInputField[0].text);
				CurrentGameTimerLengths[1] = int.Parse(adminInputGameTimerLengthInputField[1].text);
				CurrentGameTimerLengths[2] = int.Parse(adminInputGameTimerLengthInputField[2].text);
				CurrentGameTimerLengths = CheckTimerMinMaxValues(CurrentGameTimerLengths);
				CurrentAdditionalTimeIncrements[0] = int.Parse(adminInputAdditionalTimeIncrementsInputField[0].text);
				CurrentAdditionalTimeIncrements[1] = int.Parse(adminInputAdditionalTimeIncrementsInputField[1].text);
				CurrentAdditionalTimeIncrements[2] = int.Parse(adminInputAdditionalTimeIncrementsInputField[2].text);
				CurrentAdditionalTimeIncrements = CheckTimerMinMaxValues(CurrentAdditionalTimeIncrements);
				SaveTimerSettings();
				timerEvents.RaiseTimeIncrementsUpdated();
			}
			catch
			{
				Debug.Log("Add user display for error");
			}
		}
	}
}
