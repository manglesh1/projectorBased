using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using API;
using API.RequestModels;
using API.ResponseModels;
using Authentication;
using GameSession;
using GameSession.ApiResponses;
using Games.HitCustomPhotoController.ScriptableObjects;
using Games.Hit_Custom_Photo.Scriptable_Objects;
using Games.Models;
using Players;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Games.HitCustomPhotoController.Scripts
{
	public class CustomImageController : MonoBehaviour
	{
		private const string DATA_RESPONSE_SIGNING_SECRET = "OztDJ1hOy3yCAml2";

		private const float SHOW_MESSAGE_TEXT_TIME_IN_SECONDS = 1f;

		private const int TIME_BETWEEN_AUTOMATIC_RETRYS_IN_SECONDS = 15;

		private const int MAX_POLLING_BEFORE_WAITING_FOR_APPROVAL_MESSAGE = 8;

		private readonly string[] WHITELIST_WEBSITES = new string[2] { "https://axestagingstorage.blob.core.windows.net", "https://axeprodstorage.blob.core.windows.net" };

		private IEnumerator _createGameSessionCoroutine;

		private IEnumerator _downloadTeamImagesCoroutine;

		private IEnumerator _finishCreatingSessionCoroutine;

		private IEnumerator _getGameSessionFromApiCoroutine;

		private IEnumerator _waitForMessageTimeCoroutine;

		private IEnumerator _waitForRetryingGameSessionCoroutine;

		private AxcitementApiHandler _apiHandler;

		private CreateGameSessionApiResponse _createGameSessionResponse;

		private GameSessionModel _gameSessionModel;

		private List<string> _teamNames = new List<string>();

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
		private AuthenticationStateSO authenticatioinState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameSO gameSO;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		[SerializeField]
		private HitCustomPhotoGameSessionSO hitCustomPhotoGameSession;

		[SerializeField]
		private HitCustomPhotoMultiDisplayEventsSO hitCustomPhotoMultiDisplayEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[Header("Session Elements")]
		[SerializeField]
		private TMP_Text sessionIDText;

		[SerializeField]
		private TMP_Text shortGameCodeText;

		private void OnDisable()
		{
			StopGetGameSessionCoroutines();
		}

		private void Start()
		{
			ChangeGettingImagesState(GettingGamePhotoStates.SettingThingsUp);
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
				CheckIfSessionIsReady();
				break;
			default:
				settingThingsUpText.SetActive(value: true);
				cancelButton.SetActive(value: true);
				CreateGameSession();
				break;
			}
			hitCustomPhotoMultiDisplayEvents.RaiseGettingPhotosStateChanged(gettingPhotosState);
		}

		public void CheckIfSessionIsReady()
		{
			StopGetGameSessionCoroutines();
			GetGameSession();
		}

		private bool IsValidUrl(string urlToCheck, string dataResponseId)
		{
			bool flag = false;
			string[] wHITELIST_WEBSITES = WHITELIST_WEBSITES;
			for (int i = 0; i < wHITELIST_WEBSITES.Length; i++)
			{
				Uri uri = new Uri(wHITELIST_WEBSITES[i]);
				Uri uri2 = new Uri(urlToCheck);
				if (uri.IsBaseOf(uri2))
				{
					flag = true;
					break;
				}
			}
			using (HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes("OztDJ1hOy3yCAml2")))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(urlToCheck);
				return Convert.ToBase64String(hMACSHA.ComputeHash(bytes)) == dataResponseId && flag;
			}
		}

		private void CreateGameSession()
		{
			if (_apiHandler == null)
			{
				_apiHandler = UnityEngine.Object.FindObjectOfType<AxcitementApiHandler>();
			}
			CreateTeamRequestApiModel();
			CreateGameSessionApiRequestModel createGameSessionApiRequestModel = new CreateGameSessionApiRequestModel();
			createGameSessionApiRequestModel.LicenseKey = authenticatioinState.LicenseKey;
			createGameSessionApiRequestModel.GameId = gameSO.GameId;
			createGameSessionApiRequestModel.Players = _teamNames;
			_createGameSessionCoroutine = _apiHandler.CreateGameSession(createGameSessionApiRequestModel, ProcessCreateSession);
			StartCoroutine(_createGameSessionCoroutine);
		}

		private void CreateTeamRequestApiModel()
		{
			int count = playerState.CurrentPlayerNames.Count;
			for (int i = 0; i < count; i++)
			{
				_teamNames.Add(playerState.CurrentPlayerNames[i]);
			}
			hitCustomPhotoGameSession.SetTeamRequestModel(_teamNames);
		}

		private void DisplayQrCode()
		{
			bool flag = _createGameSessionResponse.Data.UploadImagesSite.Equals("axephotos.com", StringComparison.CurrentCulture);
			qrProd.SetActive(flag);
			qrStaging.SetActive(!flag);
			hitCustomPhotoMultiDisplayEvents.RaiseSetQrCodeEnvironment(flag);
		}

		private IEnumerator DownloadTeamImages(List<string> teamImageLocations)
		{
			List<Texture2D> teamTextures = new List<Texture2D>();
			foreach (string uri in teamImageLocations)
			{
				UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
				yield return www.SendWebRequest();
				teamTextures.Add(DownloadHandlerTexture.GetContent(www));
			}
			hitCustomPhotoGameSession.SetPlayerTextures(new List<Texture2D>(teamTextures));
			_waitForMessageTimeCoroutine = WaitForMessageTime();
			yield return StartCoroutine(_waitForMessageTimeCoroutine);
			hitCustomPhotoEvents.RaiseChangeGameState(HitCustomPhotoGameStates.SpeedSelection);
			gameEvents.RaiseRemoveObjectFromGameFlexSpace(base.gameObject);
		}

		private IEnumerator FinishCreatingSession()
		{
			_waitForMessageTimeCoroutine = WaitForMessageTime();
			yield return StartCoroutine(_waitForMessageTimeCoroutine);
			ChangeGettingImagesState(GettingGamePhotoStates.WaitingOnUsers);
		}

		private void GetGameSession()
		{
			string gameSessionID = hitCustomPhotoGameSession.GetGameSessionID();
			_getGameSessionFromApiCoroutine = _apiHandler.GetGameSession(gameSessionID, ProcessGetSession);
			StartCoroutine(_getGameSessionFromApiCoroutine);
		}

		private void ProcessCreateSession(ApiResponse<CreateGameSessionApiResponse> response)
		{
			if (response != null && response.Result == UnityWebRequest.Result.Success && response.Data.Success)
			{
				_createGameSessionResponse = response.Data;
				hitCustomPhotoGameSession.SetGameSessionID(_createGameSessionResponse.Data.SessionId);
				visitUrlText.text = "Visit " + _createGameSessionResponse.Data.UploadImagesSite + " \n and enter game code:";
				sessionIDText.text = hitCustomPhotoGameSession.GetGameSessionID();
				shortGameCodeText.text = hitCustomPhotoGameSession.GetGameSessionID();
				DisplayQrCode();
				hitCustomPhotoMultiDisplayEvents.RaiseGameSessionMessageChanged(visitUrlText.text, sessionIDText.text, shortGameCodeText.text);
				_finishCreatingSessionCoroutine = FinishCreatingSession();
				StartCoroutine(_finishCreatingSessionCoroutine);
			}
			else
			{
				ChangeGettingImagesState(GettingGamePhotoStates.CreateSessionError);
			}
		}

		private void ProcessGetSession(ApiResponse<GetGameSessionApiResponse> response)
		{
			if (response != null && response.Result == UnityWebRequest.Result.Success && response.Data.Success)
			{
				_gameSessionModel = response.Data.Data;
				AxePhotosAdminApprovalStatus axePhotosAdminApprovalStatus = IsSessionApproved(_gameSessionModel);
				if (_gameSessionModel.ApprovalRequired == true)
				{
					switch (axePhotosAdminApprovalStatus)
					{
					case AxePhotosAdminApprovalStatus.WaitingForApproval:
						ChangeGettingImagesState(GettingGamePhotoStates.WaitingForApproval);
						break;
					case AxePhotosAdminApprovalStatus.Denied:
					{
						List<GameSessionTeamModel> list = _gameSessionModel.Teams.FindAll((GameSessionTeamModel t) => t.Approved == false);
						deniedPlayerNamesGroup1Text.text = string.Empty;
						deniedPlayerNamesGroup2Text.text = string.Empty;
						for (int num = 0; num < list.Count; num++)
						{
							if (num % 2 == 0)
							{
								TMP_Text tMP_Text = deniedPlayerNamesGroup1Text;
								tMP_Text.text = tMP_Text.text + list[num].TeamName + " \n";
							}
							else
							{
								TMP_Text tMP_Text2 = deniedPlayerNamesGroup2Text;
								tMP_Text2.text = tMP_Text2.text + list[num].TeamName + " \n";
							}
						}
						hitCustomPhotoMultiDisplayEvents.RaiseDeniedPlayersMessageChanged(deniedPlayerNamesGroup1Text.text, deniedPlayerNamesGroup2Text.text);
						ChangeGettingImagesState(GettingGamePhotoStates.ApprovalDenied);
						break;
					}
					}
				}
				if (_gameSessionModel.SetupComplete && (_gameSessionModel.ApprovalRequired == false || axePhotosAdminApprovalStatus == AxePhotosAdminApprovalStatus.Approved))
				{
					List<string> list2 = new List<string>();
					foreach (GameSessionTeamModel team in _gameSessionModel.Teams)
					{
						List<GameSessionTeamOptionsModel> options = team.Options;
						if (IsValidUrl(options[0].Value, options[0].DataResponseId))
						{
							list2.Add(options[0].Value);
						}
						else
						{
							ChangeGettingImagesState(GettingGamePhotoStates.SecurityError);
						}
					}
					if (!securityErrorMessageText.activeSelf)
					{
						StopGetGameSessionCoroutines();
						ChangeGettingImagesState(GettingGamePhotoStates.GettingImages);
						_downloadTeamImagesCoroutine = DownloadTeamImages(list2);
						StartCoroutine(_downloadTeamImagesCoroutine);
					}
				}
			}
			_waitForRetryingGameSessionCoroutine = WaitToRetryGettingGameSession();
			StartCoroutine(_waitForRetryingGameSessionCoroutine);
		}

		public void MainMenuButton()
		{
			gameEvents.RaiseMainMenu();
		}

		public void RetryCreateGameSession()
		{
			ChangeGettingImagesState(GettingGamePhotoStates.SettingThingsUp);
		}

		private AxePhotosAdminApprovalStatus IsSessionApproved(GameSessionModel session)
		{
			bool? approved = session.Approved;
			bool flag = session.Teams.All((GameSessionTeamModel t) => t.Approved == true);
			bool flag2 = session.Teams.Any((GameSessionTeamModel t) => !t.Approved.HasValue);
			if (session.Teams.All((GameSessionTeamModel t) => t.Options.Count > 0))
			{
				if (approved.HasValue)
				{
					if (!flag2)
					{
						if (approved != true)
						{
							if (!flag)
							{
								return AxePhotosAdminApprovalStatus.Denied;
							}
							return AxePhotosAdminApprovalStatus.WaitingForApproval;
						}
						if (flag)
						{
							return AxePhotosAdminApprovalStatus.Approved;
						}
						return AxePhotosAdminApprovalStatus.Denied;
					}
					return AxePhotosAdminApprovalStatus.WaitingForApproval;
				}
				return AxePhotosAdminApprovalStatus.WaitingForApproval;
			}
			return AxePhotosAdminApprovalStatus.NotReady;
		}

		private void StopGetGameSessionCoroutines()
		{
			if (_createGameSessionCoroutine != null)
			{
				StopCoroutine(_createGameSessionCoroutine);
			}
			if (_downloadTeamImagesCoroutine != null)
			{
				StopCoroutine(_downloadTeamImagesCoroutine);
			}
			if (_finishCreatingSessionCoroutine != null)
			{
				StopCoroutine(_finishCreatingSessionCoroutine);
			}
			if (_getGameSessionFromApiCoroutine != null)
			{
				StopCoroutine(_getGameSessionFromApiCoroutine);
			}
			if (_waitForMessageTimeCoroutine != null)
			{
				StopCoroutine(_waitForMessageTimeCoroutine);
			}
			if (_waitForRetryingGameSessionCoroutine != null)
			{
				StopCoroutine(_waitForRetryingGameSessionCoroutine);
			}
		}

		private IEnumerator WaitForMessageTime()
		{
			yield return new WaitForSeconds(1f);
		}

		private IEnumerator WaitToRetryGettingGameSession()
		{
			yield return new WaitForSeconds(15f);
			CheckIfSessionIsReady();
		}
	}
}
