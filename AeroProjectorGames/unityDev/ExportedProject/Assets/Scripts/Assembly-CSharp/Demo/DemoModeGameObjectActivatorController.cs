using UnityEngine;

namespace Demo
{
	public class DemoModeGameObjectActivatorController : MonoBehaviour
	{
		[SerializeField]
		private GameObject gameObjectToShowDuringDemo;

		[SerializeField]
		private DemoEventsSO demoEvents;

		private void OnDisable()
		{
			demoEvents.OnDemoModeStarted -= HandleDemoModeStarted;
			demoEvents.OnDemoModeStopped -= HandleDemoModeStopped;
			gameObjectToShowDuringDemo.SetActive(value: false);
		}

		private void OnEnable()
		{
			demoEvents.OnDemoModeStarted += HandleDemoModeStarted;
			demoEvents.OnDemoModeStopped += HandleDemoModeStopped;
			gameObjectToShowDuringDemo.SetActive(value: false);
		}

		private void HandleDemoModeStarted()
		{
			gameObjectToShowDuringDemo.SetActive(value: true);
		}

		private void HandleDemoModeStopped()
		{
			gameObjectToShowDuringDemo.SetActive(value: false);
		}
	}
}
