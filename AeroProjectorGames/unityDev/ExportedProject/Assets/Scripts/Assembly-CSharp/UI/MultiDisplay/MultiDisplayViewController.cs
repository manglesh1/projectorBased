using System.Collections.Generic;
using Authentication;
using Games;
using UnityEngine;

namespace UI.MultiDisplay
{
	public class MultiDisplayViewController : MonoBehaviour
	{
		private bool _authenticated;

		[Header("Timer")]
		[SerializeField]
		private GameObject timerObj;

		[Header("Initially Hidden")]
		[SerializeField]
		private List<GameObject> initiallyHidden;

		[Header("Game View Objects")]
		[SerializeField]
		private List<GameObject> gameViewObjects;

		[Header("Main Menu View Objects")]
		[SerializeField]
		private List<GameObject> mainMenuViewObjects;

		[Header("Setup Teams View Objects")]
		[SerializeField]
		private List<GameObject> setupTeamsViewObjects;

		[Header("Authentication")]
		[SerializeField]
		private AuthenticationEventsSO authEvents;

		[SerializeField]
		private AuthenticationStateSO authState;

		[Header("Game Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnDisable()
		{
			authEvents.OnAuthenticated -= HandleAuthentication;
			authEvents.OnLicenseExpired -= HandleAuthentication;
			authEvents.OnNotAuthenticated -= HandleNotAuthenticated;
			authEvents.OnTemporaryOverride -= HandleAuthentication;
			authEvents.OnResetLicense -= HandleAuthentication;
			gameEvents.OnGameLoaded -= ShowGameView;
			gameEvents.OnMainMenu -= ShowMainMenuView;
		}

		private void OnEnable()
		{
			authEvents.OnAuthenticated += HandleAuthentication;
			authEvents.OnLicenseExpired += HandleAuthentication;
			authEvents.OnNotAuthenticated += HandleNotAuthenticated;
			authEvents.OnTemporaryOverride += HandleAuthentication;
			authEvents.OnResetLicense += HandleAuthentication;
			gameEvents.OnGameLoaded += ShowGameView;
			gameEvents.OnMainMenu += ShowMainMenuView;
			initiallyHidden.ForEach(delegate(GameObject obj)
			{
				obj.SetActive(value: false);
			});
			HideAllViews();
			HideTimer();
		}

		private void SetGameViewActive(bool active)
		{
			gameViewObjects.ForEach(delegate(GameObject obj)
			{
				obj.SetActive(active);
			});
		}

		private void SetMainMenuViewActive(bool active)
		{
			mainMenuViewObjects.ForEach(delegate(GameObject obj)
			{
				obj.SetActive(active);
			});
		}

		private void SetSetupTeamsViewActive(bool active)
		{
			setupTeamsViewObjects.ForEach(delegate(GameObject obj)
			{
				obj.SetActive(active);
			});
		}

		private void HandleAuthentication()
		{
			if (authState.AuthenticationStatus != AuthenticationStatus.Authenticated)
			{
				_authenticated = false;
				HideAllViews();
			}
			else if (!_authenticated)
			{
				ShowMainMenuView();
				_authenticated = true;
			}
		}

		private void HandleNotAuthenticated(string message)
		{
			HandleAuthentication();
		}

		private void HideAllViews()
		{
			HideGameView();
			HideMainMenuView();
			HideSetupTeamsView();
		}

		private void HideGameView()
		{
			SetGameViewActive(active: false);
		}

		private void HideMainMenuView()
		{
			SetMainMenuViewActive(active: false);
		}

		private void HideSetupTeamsView()
		{
			SetSetupTeamsViewActive(active: false);
		}

		private void HideTimer()
		{
			timerObj.SetActive(value: false);
		}

		private void ShowGameView()
		{
			HideAllViews();
			SetGameViewActive(active: true);
		}

		private void ShowMainMenuView()
		{
			HideAllViews();
			ShowTimer();
			SetMainMenuViewActive(active: true);
		}

		public void ShowSetupTeamsView()
		{
			HideAllViews();
			SetSetupTeamsViewActive(active: true);
		}

		private void ShowTimer()
		{
			timerObj.SetActive(value: true);
		}
	}
}
