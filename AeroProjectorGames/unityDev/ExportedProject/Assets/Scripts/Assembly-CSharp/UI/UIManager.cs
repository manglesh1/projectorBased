using System.Collections.Generic;
using Authentication;
using Detection;
using Detection.Factories;
using Games;
using UnityEngine;

namespace UI
{
	public class UIManager : MonoBehaviour
	{
		private bool _authenticated;

		private GameObject[] _detectionObjects;

		[Header("Logo Objects")]
		[SerializeField]
		private GameObject logoParentCanvas;

		[Space(1f)]
		[Header("Timer Objects")]
		[SerializeField]
		private GameObject timerParentCanvas;

		[SerializeField]
		private GameObject timerContent;

		[Space(1f)]
		[Header("Game Play Area")]
		[SerializeField]
		private GameObject gameboardParentCanvas;

		[Space]
		[Header("Scoreboard")]
		[SerializeField]
		private GameObject scoreboardParentCanvas;

		[SerializeField]
		private GameObject scoreboardContent;

		[Space(1f)]
		[Header("Game Selection Objects")]
		[SerializeField]
		private GameObject mainMenuParentCanvas;

		[SerializeField]
		private GameObject mainMenuContentScrollView;

		[Space(1f)]
		[Header("Admin Menu Objects")]
		[SerializeField]
		private GameObject adminParentCanvas;

		[SerializeField]
		private GameObject adminPinEntry;

		[SerializeField]
		private GameObject adminMenu;

		[SerializeField]
		private GameObject editPositions;

		[SerializeField]
		private GameObject adminSettings;

		[Space(1f)]
		[Header("Setup Teams Menu Objects")]
		[SerializeField]
		private GameObject setupCurrentTeams;

		[SerializeField]
		private GameObject setupModifyTeams;

		[SerializeField]
		private GameObject setupParentTeamsCanvas;

		[SerializeField]
		private GameObject setupRemoveTeamsButton;

		[SerializeField]
		private GameObject setupTeamsButtons;

		[Space(1f)]
		[Header("Main Scene Canvases")]
		[SerializeField]
		private Canvas authenticatedParentCanvas;

		[SerializeField]
		private Canvas licenseExpirationParentCanvas;

		[SerializeField]
		private GameObject mainViewCanvas;

		[SerializeField]
		private Canvas notAuthenticatedParentCanvas;

		[Space(1f)]
		[Header("Authentication Events")]
		[SerializeField]
		private AuthenticationEventsSO authenticationEvents;

		[Space(1f)]
		[Header("Game Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[Space(1f)]
		[Header("Detection Related")]
		[SerializeField]
		private DetectionCanvasFactory detectionCanvasFactory;

		[SerializeField]
		private DetectionUIEventsSO detectionEvents;

		[Space(1f)]
		[Header("System Panel")]
		[SerializeField]
		private GameObject systemPanelParentCanvas;

		[SerializeField]
		private List<GameObject> visibleSystemPanels;

		[SerializeField]
		private List<GameObject> notVisibleSystemPanels;

		private void Awake()
		{
			if (!mainViewCanvas.activeInHierarchy)
			{
				mainViewCanvas.SetActive(value: true);
			}
			authenticatedParentCanvas.gameObject.SetActive(value: false);
			licenseExpirationParentCanvas.gameObject.SetActive(value: false);
			notAuthenticatedParentCanvas.gameObject.SetActive(value: false);
			systemPanelParentCanvas.gameObject.SetActive(value: true);
			authenticationEvents.OnNotAuthenticated += ShowNotAuthenticatedView;
			authenticationEvents.OnLicenseExpired += ShowLicenseExpiredView;
			authenticationEvents.OnAuthenticated += HandleAuthenticated;
			authenticationEvents.OnTemporaryOverride += LoadDefaultView;
			gameEvents.OnMainMenu += ShowMainMenu;
			detectionEvents.OnOpenDetectionUI += ShowDetectionPanel;
			detectionEvents.OnCloseDetectionUI += HideDetectionPanel;
		}

		private void OnDestroy()
		{
			gameEvents.OnMainMenu -= ShowMainMenu;
			authenticationEvents.OnNotAuthenticated -= ShowNotAuthenticatedView;
			authenticationEvents.OnLicenseExpired -= ShowLicenseExpiredView;
			authenticationEvents.OnAuthenticated -= HandleAuthenticated;
			authenticationEvents.OnTemporaryOverride -= LoadDefaultView;
			detectionEvents.OnOpenDetectionUI -= ShowDetectionPanel;
			detectionEvents.OnCloseDetectionUI -= HideDetectionPanel;
		}

		private GameObject[] GetDetectionObjects()
		{
			if (_detectionObjects == null)
			{
				_detectionObjects = detectionCanvasFactory.GetDetectionCanvas();
			}
			return _detectionObjects;
		}

		private void HandleAuthenticated()
		{
			if (!_authenticated)
			{
				_authenticated = true;
				LoadDefaultView();
			}
		}

		public void LoadDefaultView()
		{
			HideDetectionPanel();
			notAuthenticatedParentCanvas.gameObject.SetActive(value: false);
			licenseExpirationParentCanvas.gameObject.SetActive(value: false);
			authenticatedParentCanvas.gameObject.SetActive(value: true);
			logoParentCanvas.SetActive(value: true);
			timerParentCanvas.SetActive(value: true);
			gameboardParentCanvas.SetActive(value: true);
			scoreboardParentCanvas.SetActive(value: true);
			scoreboardContent.SetActive(value: false);
			mainMenuParentCanvas.SetActive(value: true);
			mainMenuContentScrollView.SetActive(value: true);
			gameEvents.RaiseGameLicensedListUpdated();
			adminParentCanvas.SetActive(value: true);
			adminMenu.SetActive(value: false);
			editPositions.SetActive(value: false);
			adminSettings.SetActive(value: false);
			setupParentTeamsCanvas.SetActive(value: false);
			setupCurrentTeams.SetActive(value: false);
			setupTeamsButtons.SetActive(value: false);
			setupModifyTeams.SetActive(value: false);
			setupRemoveTeamsButton.SetActive(value: true);
			visibleSystemPanels.ForEach(delegate(GameObject panel)
			{
				panel.SetActive(value: true);
			});
			notVisibleSystemPanels.ForEach(delegate(GameObject panel)
			{
				panel.SetActive(value: false);
			});
			gameEvents.RaiseMainMenu();
		}

		public void HideDetectionPanel()
		{
			GameObject[] detectionObjects = GetDetectionObjects();
			for (int i = 0; i < detectionObjects.Length; i++)
			{
				detectionObjects[i].SetActive(value: false);
			}
		}

		public void ShowDetectionPanel()
		{
			LoadDefaultView();
			GameObject[] detectionObjects = GetDetectionObjects();
			for (int i = 0; i < detectionObjects.Length; i++)
			{
				detectionObjects[i].SetActive(value: true);
			}
		}

		private void ShowNotAuthenticatedView(string message = null)
		{
			_authenticated = false;
			authenticatedParentCanvas.gameObject.SetActive(value: false);
			licenseExpirationParentCanvas.gameObject.SetActive(value: false);
			notAuthenticatedParentCanvas.gameObject.SetActive(value: true);
		}

		private void ShowLicenseExpiredView()
		{
			authenticatedParentCanvas.gameObject.SetActive(value: false);
			licenseExpirationParentCanvas.gameObject.SetActive(value: true);
			notAuthenticatedParentCanvas.gameObject.SetActive(value: false);
			if (_authenticated)
			{
				_authenticated = false;
			}
		}

		public void ShowGameView()
		{
			mainMenuContentScrollView.SetActive(value: false);
			scoreboardContent.SetActive(value: true);
		}

		public void ShowMainMenu()
		{
			scoreboardContent.SetActive(value: false);
			mainMenuContentScrollView.SetActive(value: true);
		}

		public void ShowSetupTeamsMenu()
		{
			mainMenuParentCanvas.SetActive(value: false);
			setupParentTeamsCanvas.SetActive(value: true);
			setupTeamsButtons.SetActive(value: true);
			setupModifyTeams.SetActive(value: true);
		}

		public void ShowSubGameMenu()
		{
			mainMenuContentScrollView.SetActive(value: false);
		}
	}
}
