using System.Collections.Generic;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Admin
{
	public class AdminEndOfTimeActionController : MonoBehaviour
	{
		[SerializeField]
		private List<EndOfTimerActionClass> endOfTimerActionToggleList;

		[SerializeField]
		private GameObject endOfTimerMainGroup;

		[Header("Settings Elements")]
		private EndOfTimerActionsSettings _endOfTimerActionSettings;

		[Header("External References")]
		[SerializeField]
		private TimerEventsSO timerEvents;

		private EndOfTimerActionStates _endOfTimerAction;

		private EndOfTimerActionStates endOfTimerAction
		{
			get
			{
				return _endOfTimerAction;
			}
			set
			{
				_endOfTimerAction = value;
				timerEvents.SelectedAction = value;
			}
		}

		private void Awake()
		{
			timerEvents.OnDisabledTimer += TimerDisabled;
			timerEvents.OnDisplayTimerSettings += ConfigureEndOfTimeActionSettings;
		}

		private void OnDisable()
		{
			timerEvents.OnDisabledTimer -= TimerDisabled;
			timerEvents.OnDisplayTimerSettings -= ConfigureEndOfTimeActionSettings;
		}

		private void Start()
		{
			GetEndOfTimerActionSettings();
		}

		private void ConfigureEndOfTimeActionSettings()
		{
			endOfTimerActionToggleList.Find((EndOfTimerActionClass t) => t.Action == endOfTimerAction).AssociatedToggle.isOn = true;
		}

		public void EndOfTimerActionSelectionChange(Toggle ActionToggle)
		{
			if (ActionToggle.isOn)
			{
				endOfTimerAction = endOfTimerActionToggleList.Find((EndOfTimerActionClass t) => t.AssociatedToggle == ActionToggle).Action;
			}
			SaveEndOfTimerActionSettings();
		}

		private void GetEndOfTimerActionSettings()
		{
			_endOfTimerActionSettings = SettingsStore.EndofTimerActions;
			endOfTimerAction = _endOfTimerActionSettings.EndOfTimerAction;
		}

		private void SaveEndOfTimerActionSettings()
		{
			_endOfTimerActionSettings.EndOfTimerAction = _endOfTimerAction;
			_endOfTimerActionSettings.Save();
		}

		private void TimerDisabled(bool isDisabled)
		{
			endOfTimerMainGroup.SetActive(!isDisabled);
		}
	}
}
