using Games.Hit_Custom_Photo.Scriptable_Objects;
using Settings;
using TMPro;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class SecondDisplayCustomImageController : MonoBehaviour
	{
		[Header("UI Elements")]
		[SerializeField]
		private GameObject approvalDeniedPanel;

		[SerializeField]
		private TMP_Text deniedPlayerNamesGroup1Text;

		[SerializeField]
		private TMP_Text deniedPlayerNamesGroup2Text;

		[SerializeField]
		private GameObject cancelButton;

		[SerializeField]
		private GameObject createSessionErrorText;

		[SerializeField]
		private GameObject gameSessionCodeGroup;

		[SerializeField]
		private GameObject gameSessionCodeText;

		[SerializeField]
		private GameObject gettingImagesText;

		[SerializeField]
		private GameObject qrProd;

		[SerializeField]
		private GameObject qrStaging;

		[SerializeField]
		private GameObject retryButton;

		[SerializeField]
		private GameObject securityErrorMessageText;

		[SerializeField]
		private GameObject settingThingsUpText;

		[SerializeField]
		private GameObject shortPanelInfo;

		[SerializeField]
		private TMP_Text visitUrlText;

		[SerializeField]
		private GameObject waitingForApprovalText;

		[Header("Scriptable Objects")]
		[SerializeField]
		private HitCustomPhotoMultiDisplayEventsSO gameMultiDisplayEvents;

		[Header("Session Elements")]
		[SerializeField]
		private TMP_Text sessionIDText;

		[SerializeField]
		private TMP_Text shortGameCodeText;

		private void OnEnable()
		{
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				gameMultiDisplayEvents.OnDeniedPlayersMessageChanged += HandleDeniedPlayersMessageChanges;
				gameMultiDisplayEvents.OnGettingPhotosStateChanged += ChangeGettingImagesState;
				gameMultiDisplayEvents.OnGameSessionMessageChanged += HandleGameSessionMessageChanges;
				gameMultiDisplayEvents.OnQrCodeEnvironmentSet += DisplayOnQrCode;
				ChangeGettingImagesState(GettingGamePhotoStates.SettingThingsUp);
			}
		}

		private void OnDisable()
		{
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				gameMultiDisplayEvents.OnDeniedPlayersMessageChanged -= HandleDeniedPlayersMessageChanges;
				gameMultiDisplayEvents.OnGettingPhotosStateChanged -= ChangeGettingImagesState;
				gameMultiDisplayEvents.OnGameSessionMessageChanged -= HandleGameSessionMessageChanges;
				gameMultiDisplayEvents.OnQrCodeEnvironmentSet -= DisplayOnQrCode;
			}
		}

		private void ChangeGettingImagesState(GettingGamePhotoStates gettingPhotosState)
		{
			approvalDeniedPanel.SetActive(value: false);
			cancelButton.SetActive(value: false);
			createSessionErrorText.SetActive(value: false);
			gameSessionCodeGroup.SetActive(value: false);
			gameSessionCodeText.SetActive(value: false);
			gettingImagesText.SetActive(value: false);
			retryButton.SetActive(value: false);
			securityErrorMessageText.SetActive(value: false);
			settingThingsUpText.SetActive(value: false);
			shortPanelInfo.SetActive(value: false);
			visitUrlText.gameObject.SetActive(value: false);
			waitingForApprovalText.SetActive(value: false);
			switch (gettingPhotosState)
			{
			case GettingGamePhotoStates.ApprovalDenied:
				approvalDeniedPanel.SetActive(value: true);
				shortPanelInfo.SetActive(value: true);
				gameSessionCodeGroup.SetActive(value: true);
				cancelButton.SetActive(value: true);
				break;
			case GettingGamePhotoStates.CreateSessionError:
				createSessionErrorText.SetActive(value: true);
				retryButton.SetActive(value: true);
				cancelButton.SetActive(value: true);
				break;
			case GettingGamePhotoStates.GettingImages:
				gettingImagesText.SetActive(value: true);
				cancelButton.SetActive(value: true);
				break;
			case GettingGamePhotoStates.SecurityError:
				securityErrorMessageText.SetActive(value: true);
				cancelButton.SetActive(value: true);
				break;
			case GettingGamePhotoStates.WaitingForApproval:
				waitingForApprovalText.SetActive(value: true);
				shortPanelInfo.SetActive(value: true);
				gameSessionCodeGroup.SetActive(value: true);
				cancelButton.SetActive(value: true);
				break;
			case GettingGamePhotoStates.WaitingOnUsers:
				gameSessionCodeGroup.SetActive(value: true);
				gameSessionCodeText.SetActive(value: true);
				visitUrlText.gameObject.SetActive(value: true);
				cancelButton.SetActive(value: true);
				break;
			default:
				settingThingsUpText.SetActive(value: true);
				cancelButton.SetActive(value: true);
				break;
			}
		}

		private void HandleDeniedPlayersMessageChanges(string player1Group, string player2Group)
		{
			deniedPlayerNamesGroup1Text.text = player1Group;
			deniedPlayerNamesGroup2Text.text = player2Group;
		}

		private void HandleGameSessionMessageChanges(string visitUrl, string gameSessionId, string shortCodeText)
		{
			visitUrlText.text = visitUrl;
			sessionIDText.text = gameSessionId;
			shortGameCodeText.text = gameSessionId;
		}

		private void DisplayOnQrCode(bool isProd)
		{
			qrProd.SetActive(isProd);
			qrStaging.SetActive(!isProd);
		}
	}
}
