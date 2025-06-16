using MainMenu;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Admin.Demo
{
	public class AdminDemoSettingsController : MonoBehaviour
	{
		private MainMenuSettings _mainMenuSettings;

		[SerializeField]
		private DemoSO demoManager;

		[SerializeField]
		private ToggleGroup timerToggleGroup;

		[Header("Use Demo Toggle")]
		[SerializeField]
		private Toggle toggleUseDemo;

		[Header("Wait Timer Toggles")]
		[SerializeField]
		private GameObject timeTogglesGroup;

		[SerializeField]
		private Toggle toggle1Min;

		[SerializeField]
		private Toggle toggle10Min;

		[SerializeField]
		private Toggle toggle20Min;

		[SerializeField]
		private Toggle toggle30Min;

		private void OnEnable()
		{
			_mainMenuSettings = SettingsStore.MainMenu;
			toggleUseDemo.isOn = demoManager.UseDemo;
			timeTogglesGroup.SetActive(demoManager.UseDemo);
		}

		private void Start()
		{
			SetTimerToggle();
		}

		private void SaveMainMenuSettings()
		{
			SettingsStore.MainMenu.UseDemo = demoManager.UseDemo;
			SettingsStore.MainMenu.WaitBeforeDemoTimeInMinutes = demoManager.WaitBeforeDemoTimeInMinutes;
			SettingsStore.MainMenu.Save();
		}

		private void SetTimerToggle()
		{
			if (demoManager.UseDemo)
			{
				switch (demoManager.WaitBeforeDemoTimeInMinutes)
				{
				case 1:
					toggle1Min.isOn = true;
					break;
				case 10:
					toggle10Min.isOn = true;
					break;
				case 20:
					toggle20Min.isOn = true;
					break;
				default:
					toggle30Min.isOn = true;
					break;
				}
			}
		}

		public void TimerToggleChanged()
		{
			string text = timerToggleGroup.GetFirstActiveToggle().name;
			if (text == toggle30Min.name)
			{
				demoManager.WaitBeforeDemoTimeInMinutes = 30;
			}
			else if (text == toggle20Min.name)
			{
				demoManager.WaitBeforeDemoTimeInMinutes = 20;
			}
			else if (text == toggle10Min.name)
			{
				demoManager.WaitBeforeDemoTimeInMinutes = 10;
			}
			else
			{
				demoManager.WaitBeforeDemoTimeInMinutes = 1;
			}
			SaveMainMenuSettings();
		}

		public void UseDemoToggleChanged()
		{
			demoManager.UseDemo = toggleUseDemo.isOn;
			timeTogglesGroup.SetActive(toggleUseDemo.isOn);
			SaveMainMenuSettings();
		}
	}
}
