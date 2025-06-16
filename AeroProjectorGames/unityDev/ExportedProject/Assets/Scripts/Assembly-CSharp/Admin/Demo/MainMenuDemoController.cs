using System.Collections;
using System.Collections.Generic;
using Demo;
using Games;
using MainMenu;
using Settings;
using Timer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Admin.Demo
{
	public class MainMenuDemoController : MonoBehaviour
	{
		private const int DEFAULT_IDLE_TIME_IN_MINUTES = 30;

		private const int DELAY_BETWEEN_GAMES_DURING_DEMO_IN_SECONDS = 3;

		private const int LOWEST_IDLE_TIME_IN_MINUTES = 1;

		private const int RECALCULATE_IDLE_TIME_IN_SECONDS = 30;

		private const int SECONDS_IN_A_MINTUE = 60;

		private float _currentIdleTimeInSeconds;

		private float _idleTimeBeforeStartingDemoInSeconds;

		private MainMenuSettings _mainMenuSettings;

		private IEnumerator _runDemoEnumerator;

		private float _timetoRecalculateIdleBeforeTimeInSeconds;

		[SerializeField]
		private DemoEventsSO demoEvents;

		[SerializeField]
		private DemoSO demoManager;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private TimerStateSO timerState;

		[SerializeField]
		private GameObject mainMenuContent;

		[Header("Games To Exclude From Demo")]
		[Tooltip("List of game names to exclude from demo mode")]
		[SerializeField]
		private List<string> excludeGamesByName = new List<string>();

		private void OnEnable()
		{
			if (demoManager.DemoIsRunning)
			{
				_runDemoEnumerator = RunDemo();
				StartCoroutine(_runDemoEnumerator);
			}
			LoadSavedSettings();
			_currentIdleTimeInSeconds = 0f;
			_timetoRecalculateIdleBeforeTimeInSeconds = 0f;
			GetIdleTimeBeforeDemoInSeconds();
		}

		private void OnDisable()
		{
			StopDemoCoroutine();
		}

		private void Update()
		{
			CheckToStopDemo();
			CheckForUserInput();
			CheckForUserManuallyStartingDemo();
			IdleTimer();
		}

		private float CheckForDefaultTimeInMinutes(float idleTime)
		{
			if ((timerState.CurrentState != TimerStateEnum.Stopped && timerState.CurrentState != TimerStateEnum.Disabled) || idleTime > 1f)
			{
				return 30f;
			}
			return idleTime;
		}

		private void CheckForExcludedGames(GameObject selectedGameMenuItem)
		{
			foreach (string item in excludeGamesByName)
			{
				if (selectedGameMenuItem.name == item)
				{
					GetRandomMenuItem();
					return;
				}
			}
			LoadGameInDemoMode(selectedGameMenuItem);
		}

		private void CheckForUserInput()
		{
			if (!demoManager.DemoIsRunning && (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
			{
				_currentIdleTimeInSeconds = 0f;
			}
		}

		private void CheckForUserManuallyStartingDemo()
		{
			if (!demoManager.DemoIsRunning && Keyboard.current.ctrlKey.isPressed && Keyboard.current.dKey.wasPressedThisFrame)
			{
				demoManager.DemoIsRunning = true;
				GetRandomMenuItem();
			}
		}

		private void CheckToStopDemo()
		{
			bool flag = Touchscreen.current != null && Touchscreen.current.press.wasPressedThisFrame;
			if ((Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || flag) && demoManager.DemoIsRunning)
			{
				demoManager.DemoIsRunning = false;
				StopDemoCoroutine();
				gameEvents.RaiseMainMenu();
				demoEvents.RaiseDemoModeStopped();
			}
		}

		private float ConvertMinutesIntoSeconds(float timeInMinutes)
		{
			return timeInMinutes * 60f;
		}

		private void GetIdleTimeBeforeDemoInSeconds()
		{
			float idleTime = demoManager.WaitBeforeDemoTimeInMinutes;
			idleTime = CheckForDefaultTimeInMinutes(idleTime);
			_idleTimeBeforeStartingDemoInSeconds = ConvertMinutesIntoSeconds(idleTime);
		}

		private void GetRandomMenuItem()
		{
			int randomNumber = GetRandomNumber(1, mainMenuContent.transform.childCount);
			GameObject selectedGameMenuItem = mainMenuContent.transform.GetChild(randomNumber).gameObject;
			CheckForExcludedGames(selectedGameMenuItem);
		}

		private int GetRandomNumber(int minNumber, int maxNumber)
		{
			return Random.Range(minNumber, maxNumber);
		}

		private void IdleTimer()
		{
			if (demoManager.UseDemo && _idleTimeBeforeStartingDemoInSeconds != 0f && !demoManager.DemoIsRunning)
			{
				_currentIdleTimeInSeconds += Time.deltaTime;
				_timetoRecalculateIdleBeforeTimeInSeconds += Time.deltaTime;
				if (_timetoRecalculateIdleBeforeTimeInSeconds >= 30f)
				{
					GetIdleTimeBeforeDemoInSeconds();
				}
				if (_currentIdleTimeInSeconds >= _idleTimeBeforeStartingDemoInSeconds)
				{
					_runDemoEnumerator = RunDemo();
					StartCoroutine(_runDemoEnumerator);
				}
			}
			else if (demoManager.UseDemo && _idleTimeBeforeStartingDemoInSeconds == 0f && !demoManager.DemoIsRunning)
			{
				GetIdleTimeBeforeDemoInSeconds();
			}
		}

		private void LoadGameInDemoMode(GameObject selectedMenuOption)
		{
			selectedMenuOption.GetComponent<Button>().onClick.Invoke();
		}

		private IEnumerator RunDemo()
		{
			float seconds;
			if (demoManager.DemoIsRunning)
			{
				seconds = 3f;
			}
			else
			{
				seconds = 0f;
				demoManager.DemoIsRunning = true;
				demoEvents.RaiseDemoModeStarted();
			}
			yield return new WaitForSeconds(seconds);
			if (HasDemoGameAvailable())
			{
				GetRandomMenuItem();
			}
		}

		private bool HasDemoGameAvailable()
		{
			bool result = false;
			for (int i = 0; i < mainMenuContent.transform.childCount; i++)
			{
				GameObject gameObject = mainMenuContent.transform.GetChild(i).gameObject;
				if (!excludeGamesByName.Contains(gameObject.name))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private void LoadSavedSettings()
		{
			_mainMenuSettings = SettingsStore.MainMenu;
			demoManager.UseDemo = _mainMenuSettings.UseDemo;
			demoManager.WaitBeforeDemoTimeInMinutes = _mainMenuSettings.WaitBeforeDemoTimeInMinutes;
		}

		private void StopDemoCoroutine()
		{
			if (_runDemoEnumerator != null)
			{
				StopCoroutine(_runDemoEnumerator);
			}
		}
	}
}
