using System;
using System.Collections;
using System.Text;
using API.RequestModels;
using API.ResponseModels;
using Admin_Panel.ApiResponses;
using Authentication;
using Backgrounds.CustomBackgrounds.ApiResponses;
using GameSession.ApiResponses;
//using Games.Hit_Custom_Photo.Editor;
using Helpers;
using Logging;
using Network;
using Newtonsoft.Json;
using Telemetry.API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace API
{
    public class AxcitementApiHandler : MonoBehaviour
    {
        private const string API_URL = "https://api.axcitement.tech/";

        private const string AUTH_URI = "customers/authenticate";

        private const string CREATE_GAME_SESSION_URI = "games/photos/create";

        private const string CUSTOM_BACKGROUNDS_URI = "customers/backgrounds/";

        private const string LICENSES_URI = "customers/licenses";

        private const string DELETE_GAME_SESSION_URI = "games/photos/";

        private const string GET_GAME_SESSION_URI = "games/photos/";

        private const string LOG_URI = "logs";

        private const string SAVE_GAME_TIME_URI = "games/time";

        private const string SAVE_GAMES_PLAYED_STATS_URI = "stats/gamesplayed";

        private const string SETTINGS_URI = "settings";

        private const string REFRESH_URI = "customers/refresh";

        private const string AUTHORIZATION_HEADER = "Authorization";

        private const string CONTENT_TYPE = "Content-Type";

        private const string DELETE = "DELETE";

        private const string GET = "GET";

        private const string JSON = "application/json";

        private const string POST = "POST";

        private const string PUT = "PUT";

        private string _authUrl;

        private string _createGameSessionUrl;

        private string _customBackgroundsUrl;

        private string _logUrl;

        private string _refreshUrl;

        private string _saveGameTimeUrl;

        private string _saveGamesPlayedStatsUrl;

        private ApplicationSettingsOverrides _overrides;

        [SerializeField]
        private NetworkStatusEventsSO networkStatusEvents;

        [SerializeField]
        private AuthenticationStateSO authState;

        //[FormerlySerializedAs("_testSessions")]
        //[Header("Test Responses")]
        //[SerializeField]
        //private ApiTestResponsesSO testResponses;

        private void Awake()
        {
            _authUrl = "https://api.axcitement.tech/customers/authenticate";
            _createGameSessionUrl = "https://api.axcitement.tech/games/photos/create";
            _customBackgroundsUrl = "https://api.axcitement.tech/customers/backgrounds/";
            _logUrl = "https://api.axcitement.tech/logs";
            _refreshUrl = "https://api.axcitement.tech/customers/refresh";
            _saveGameTimeUrl = "https://api.axcitement.tech/games/time";
            _saveGamesPlayedStatsUrl = "https://api.axcitement.tech/stats/gamesplayed";
            _overrides = ApplicationSettingsOverrides.Instance;
            if (!string.IsNullOrEmpty(_overrides.ApiUrl))
            {
                _authUrl = _overrides.ApiUrl + "customers/authenticate";
                _createGameSessionUrl = _overrides.ApiUrl + "games/photos/create";
                _customBackgroundsUrl = _overrides.ApiUrl + "customers/backgrounds/";
                _logUrl = _overrides.ApiUrl + "logs";
                _refreshUrl = _overrides.ApiUrl + "customers/refresh";
                _saveGameTimeUrl = _overrides.ApiUrl + "games/time";
                _saveGamesPlayedStatsUrl = _overrides.ApiUrl + "stats/gamesplayed";
            }
        }

        public IEnumerator CreateGameSession(CreateGameSessionApiRequestModel request, Action<ApiResponse<CreateGameSessionApiResponse>> responseCallback)
        {
            //responseCallback(testResponses.CreateGameSessionSuccessfulResponse());
            yield break;
        }

        public IEnumerator DeleteGameSession(string sessionId, Action<ApiResponse<DeleteGameSessionApiResponse>> responseCallback)
        {
            responseCallback(new ApiResponse<DeleteGameSessionApiResponse>
            {
                Result = UnityWebRequest.Result.Success,
                Data = new DeleteGameSessionApiResponse
                {
                    Data = true,
                    Success = true
                }
            });
            yield break;
        }

        public IEnumerator GetCustomBackgrounds(Action<ApiResponse<GetCustomBackgroundsApiResponse>> responseCallback)
        {
            string url = _customBackgroundsUrl + authState.LicenseKey;
            using UnityWebRequest uwr = UnityWebRequest.Get(url);
            SetupGetRequest(uwr);
            yield return uwr.SendWebRequest();
            ApiResponse<GetCustomBackgroundsApiResponse> response = new ApiResponseBuilder<GetCustomBackgroundsApiResponse>().AddResponse(uwr.downloadHandler.text).AddResult(uwr.result).Build();
            responseCallback(response);
        }

        public IEnumerator GetLicenseBackupSettings(Action<ApiResponse<GetLicenseBackupSettingsResponse>> responseCallback)
        {
            using UnityWebRequest uwr = UnityWebRequest.Get(GetLicenseSettingsUrl(authState.LicenseKey));
            SetupGetRequest(uwr, authState.BearerToken);
            yield return uwr.SendWebRequest();
            ApiResponse<GetLicenseBackupSettingsResponse> response = new ApiResponseBuilder<GetLicenseBackupSettingsResponse>().AddResponse(uwr.downloadHandler.text).AddResult(uwr.result).Build();
            responseCallback(response);
        }

        public IEnumerator GetGameSession(string sessionId, Action<ApiResponse<GetGameSessionApiResponse>> responseCallback)
        {
            //responseCallback(testResponses.GetGameSessionApiResponse());
            yield break;
        }

        private string GetLicenseSettingsUrl(string licenseKey)
        {
            return (string.IsNullOrEmpty(_overrides?.ApiUrl) ? "https://api.axcitement.tech/" : _overrides.ApiUrl) + "customers/licenses/" + licenseKey + "/settings";
        }

        public IEnumerator RegisterLicense(string licenseKey, Action<ApiResponse<LicenseValidationInformation>> responseCallback)
        {
            RegisterLicenseApiRequestModel requestData = new RegisterLicenseApiRequestModel(licenseKey);
            using UnityWebRequest uwr = new UnityWebRequest(_authUrl, "POST");
            SetupPostRequest(uwr, requestData);
            yield return uwr.SendWebRequest();
            ApiResponse<LicenseValidationInformation> apiResponse = new ApiResponseBuilder<LicenseValidationInformation>().AddResponse(uwr.downloadHandler.text).AddResult(uwr.result).Build();
            HandleNetworkResponse(apiResponse.Result);
            responseCallback(apiResponse);
        }

        public IEnumerator SaveGameTime(int minutesRemaining, string timerStatus, Action<ApiResponse<BoolApiResponse>> responseCallback)
        {
            responseCallback(new ApiResponse<BoolApiResponse>
            {
                Result = UnityWebRequest.Result.Success,
                Data = new BoolApiResponse
                {
                    Data = true,
                    Success = true
                }
            });
            yield break;
        }

        public IEnumerator SaveLicenseBackupSettings(string serializedSettingsData, Action<ApiResponse<SaveLicenseBackupSettingsResponse>> responseCallback)
        {
            using UnityWebRequest uwr = new UnityWebRequest(GetLicenseSettingsUrl(authState.LicenseKey), "PUT");
            SetupPostRequest(uwr, serializedSettingsData, authState.BearerToken);
            yield return uwr.SendWebRequest();
            responseCallback(new ApiResponseBuilder<SaveLicenseBackupSettingsResponse>().AddResponse(uwr.downloadHandler.text).AddResult(uwr.result).Build());
        }

        public IEnumerator SendGamesPlayedStats(GamesPlayedStatsApiRequest request, Action<ApiResponse<BoolApiResponse>> responseCallback)
        {
            responseCallback(new ApiResponse<BoolApiResponse>
            {
                Result = UnityWebRequest.Result.Success,
                Data = new BoolApiResponse
                {
                    Data = true,
                    Success = true
                }
            });
            yield break;
        }

        public IEnumerator SendLog(LogRecord logRecord)
        {
            using UnityWebRequest uwr = new UnityWebRequest(_logUrl, "POST");
            SetupPostRequest(uwr, logRecord);
            yield return uwr.SendWebRequest();
        }

        public IEnumerator ValidateRefreshToken(ValidateRefreshTokenApiRequestModel request, Action<ApiResponse<LicenseValidationInformation>> responseCallback)
        {
            using UnityWebRequest uwr = new UnityWebRequest(_refreshUrl, "POST");
            SetupPostRequest(uwr, request);
            yield return uwr.SendWebRequest();
            ApiResponse<LicenseValidationInformation> apiResponse = new ApiResponseBuilder<LicenseValidationInformation>().AddResponse(uwr.downloadHandler.text).AddResult(uwr.result).Build();
            HandleNetworkResponse(apiResponse.Result);
            responseCallback(apiResponse);
        }

        private string GetDeleteSessionUrl(string sessionId)
        {
            string result = "https://api.axcitement.tech/games/photos/" + sessionId;
            if (!string.IsNullOrEmpty(_overrides.ApiUrl))
            {
                result = _overrides.ApiUrl + "games/photos/" + sessionId;
            }
            return result;
        }

        private string GetGameSessionUrl(string sessionId)
        {
            string result = "https://api.axcitement.tech/games/photos/" + sessionId;
            if (!string.IsNullOrEmpty(_overrides.ApiUrl))
            {
                result = _overrides.ApiUrl + "games/photos/" + sessionId;
            }
            return result;
        }

        private void HandleNetworkResponse(UnityWebRequest.Result result)
        {
            if (result != UnityWebRequest.Result.Success)
            {
                networkStatusEvents.RaiseFailure();
            }
            else
            {
                networkStatusEvents.RaiseSuccess();
            }
        }

        private void SetupDeleteRequest(UnityWebRequest uwr, string bearerToken = null)
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            if (bearerToken != null)
            {
                uwr.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            }
        }

        private void SetupGetRequest(UnityWebRequest uwr, string bearerToken = null)
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            if (bearerToken != null)
            {
                uwr.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            }
        }

        private void SetupPostRequest<T>(UnityWebRequest uwr, T requestData, string bearerToken = null)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestData));
            uwr.uploadHandler = new UploadHandlerRaw(bytes);
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            if (bearerToken != null)
            {
                uwr.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            }
        }
    }
}
