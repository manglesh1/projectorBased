using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Admin
{
	public class AdminTimerPinController : MonoBehaviour
	{
		private const string DEFAULT_NEW_PIN_MESSAGE = "Enter a pin 4 to 15 numbers in length and hit Apply Pin";

		private const string LEFT_PIN_ENTRY_BLANK_MESSAGE = "You must apply a pin or it will not be enabled";

		private const string PIN_ENTRY_HIDE_PIN_TEXT = "Hide Pin";

		private const string PIN_LIMIT_REACHED_MESSAGE = "Pin length has been reached.";

		private const string MINIMUM_LENGTH_ERROR_MESSAGE = "Pin needs to be at least 4 numbers in length.";

		private const int MINIMUM_PIN_LENGTH = 4;

		private const string PIN_ENTRY_SHOW_PIN_TEXT = "Show Pin";

		private const string PIN_ENTRY_SUCCESS_MESSAGE = "Pin Saved";

		[SerializeField]
		private Toggle disableTimerPinToggle;

		[SerializeField]
		private TMP_Text pinEntryMessageText;

		[SerializeField]
		private TMP_InputField pinEntryInputField;

		[SerializeField]
		private GameObject pinEntryMainGroup;

		[SerializeField]
		private GameObject pinInputGroup;

		[SerializeField]
		private TMP_Text showPinButtonLabel;

		[Header("Settings Elements")]
		private TimerPinSettings _timerPinSettings;

		[Header("External References")]
		[SerializeField]
		private TimerEventsSO timerEvents;

		private bool _preventOnVauleChangeCalls;

		private bool _isPinDisabled;

		private string _pin;

		public bool IsTimerPinDisabled => _isPinDisabled;

		public string TimerPin => _pin;

		private void Awake()
		{
			timerEvents.OnDisabledTimer += TimerDisabled;
			timerEvents.OnDisplayTimerSettings += ConfigurePinSettings;
		}

		private void OnDisable()
		{
			timerEvents.OnDisabledTimer -= TimerDisabled;
			timerEvents.OnDisplayTimerSettings -= ConfigurePinSettings;
		}

		private void Start()
		{
			GetTimerPinSettings();
		}

		private void ConfigurePinSettings()
		{
			if (!_preventOnVauleChangeCalls)
			{
				_preventOnVauleChangeCalls = true;
				disableTimerPinToggle.isOn = _isPinDisabled;
				pinInputGroup.SetActive(!_isPinDisabled);
				pinEntryInputField.inputType = TMP_InputField.InputType.Password;
				pinEntryInputField.ForceLabelUpdate();
				pinEntryInputField.text = _pin;
				showPinButtonLabel.text = "Show Pin";
				if (!_isPinDisabled && _pin.Length >= 4)
				{
					pinEntryMessageText.text = string.Empty;
				}
				else
				{
					pinEntryMessageText.text = "Enter a pin 4 to 15 numbers in length and hit Apply Pin";
				}
				_preventOnVauleChangeCalls = false;
			}
		}

		private void GetTimerPinSettings()
		{
			_timerPinSettings = SettingsStore.TimerPin;
			_isPinDisabled = _timerPinSettings.IsDisabled;
			_pin = _timerPinSettings.TimerPin;
			if (_pin.Length < 4 && !_isPinDisabled)
			{
				_isPinDisabled = true;
				disableTimerPinToggle.isOn = true;
			}
		}

		public void MouseLeftPinEntrySettings()
		{
			if (_pin.Length < 4)
			{
				_isPinDisabled = true;
				pinEntryMessageText.text = "You must apply a pin or it will not be enabled";
			}
			else
			{
				pinEntryMessageText.text = string.Empty;
			}
		}

		private void SaveTimerPinSettings()
		{
			_timerPinSettings.IsDisabled = _isPinDisabled;
			_timerPinSettings.TimerPin = _pin;
			_timerPinSettings.Save();
		}

		private void TimerDisabled(bool isDisabled)
		{
			pinEntryMainGroup.SetActive(!isDisabled);
		}

		public void DisablePin(Toggle pinToggle)
		{
			if (!_preventOnVauleChangeCalls)
			{
				_isPinDisabled = pinToggle.isOn;
				_pin = string.Empty;
				if (_isPinDisabled)
				{
					SaveTimerPinSettings();
				}
				ConfigurePinSettings();
			}
		}

		public void PinInputFieldOnValueChange(TMP_InputField pinInputField)
		{
			if (pinInputField.text.Length == pinInputField.characterLimit)
			{
				pinEntryMessageText.text = "Pin length has been reached.";
			}
			else
			{
				pinEntryMessageText.text = "Enter a pin 4 to 15 numbers in length and hit Apply Pin";
			}
		}

		public void SubmitPin(TMP_InputField pinInputField)
		{
			string text = pinInputField.text.Trim();
			if (text.Trim().Length < 4)
			{
				pinEntryMessageText.text = "Pin needs to be at least 4 numbers in length.";
				return;
			}
			_pin = text;
			_isPinDisabled = false;
			SaveTimerPinSettings();
			pinEntryMessageText.text = "Pin Saved";
		}

		public void UnMaskPinInputFieldText(TMP_Text buttonLabel)
		{
			if (pinEntryInputField.inputType == TMP_InputField.InputType.Password)
			{
				buttonLabel.text = "Hide Pin";
				pinEntryInputField.inputType = TMP_InputField.InputType.Standard;
			}
			else
			{
				buttonLabel.text = "Show Pin";
				pinEntryInputField.inputType = TMP_InputField.InputType.Password;
			}
			pinEntryInputField.ForceLabelUpdate();
		}
	}
}
