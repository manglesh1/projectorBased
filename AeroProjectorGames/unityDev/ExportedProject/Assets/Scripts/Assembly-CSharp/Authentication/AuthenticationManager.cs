using System;
using System.Collections;
using System.Globalization;
using System.IO;
using API;
using API.ResponseModels;
using Encryption;
using Helpers;
using Logging;
using MHLab.Patch.Core.Logging;
using Settings;
using UnityEngine;
using UnityEngine.Networking;

namespace Authentication
{
	public class AuthenticationManager : MonoBehaviour
	{
		private const int TEMPORARY_OVERRIDE_MULTIPLIER = 2;

		private const string FILE_NAME = "SystemHandler.dll";

		private const string LICENSE_RESET = "License reset";

		private const string LOST_LICENSE_MESSAGE = "LOST_LICENSE";

		private const string PROCESS_NAME = "Authentication";

		private EncryptServices _encryptService;

		private string _fullPath;

		private Coroutine _licenseCheckRoutine;

		private bool _lostLicense;

		private bool _override;

		private int _overrideCount;

		private Coroutine _overrideRoutine;

		private LicenseValidationInformation _validationInformation;

		[SerializeField]
		private AxcitementApiHandler apiHandler;

		[SerializeField]
		private AuthenticationStateSO authenticationStateInfo;

		[SerializeField]
		private AuthenticationEventsSO authenticationEvents;

		[SerializeField]
		private LogEventsSO logEvents;

		[SerializeField]
		private float hoursToWait = 6f;

		private void Awake()
		{
			MHLab.Patch.Core.Logging.Logger.LogInfo("AuthenticationManager Awake " + Application.persistentDataPath);
		}

		private void Start()
		{
			LoadLicenseFromDisk();
			Authenticated();
		}

		private void ClearLicenseAndReset()
		{
			if (File.Exists(_fullPath))
			{
				logEvents.RaiseLogRequest(LogSeverity.Info, "Authentication", "License was cleared and reset");
				StopCoroutine(_licenseCheckRoutine);
				File.Delete(_fullPath);
				_encryptService.DeleteLicenseFile();
				_validationInformation = new LicenseValidationInformation();
				NotAuthenticated("License reset");
			}
		}

		private DateTime GetExpirationDate()
		{
			DateTime.TryParse(_validationInformation.expires, null, DateTimeStyles.AdjustToUniversal, out var result);
			return result;
		}

		private void HandleLostLicense()
		{
			if (!string.IsNullOrEmpty(_encryptService.LicenseKey))
			{
				_validationInformation = new LicenseValidationInformation();
				_validationInformation.licenseKey = _encryptService.LicenseKey;
				_validationInformation.refreshToken = "LOST_LICENSE";
				RunLicenseCheck();
			}
			else
			{
				logEvents.RaiseLogRequest(LogSeverity.Warning, "Authentication", "Unable to repair the broken license");
				_validationInformation = new LicenseValidationInformation();
			}
		}

		private void LoadLicenseFromDisk()
		{
			_validationInformation = new LicenseValidationInformation();
			_validationInformation.expires = DateTime.UtcNow.AddDays(10.0).ToString("yyyy-MM-ddTHH:mm:ssZ");
		}

		private void ProcessRegistrationResponse(ApiResponse<LicenseValidationInformation> response)
		{
			if (response != null && response.Result == UnityWebRequest.Result.Success && response.Data.success)
			{
				_validationInformation = response.Data;
				authenticationStateInfo.LoadAuthInfoFromLicenseValidation(_validationInformation);
				SaveLicense();
				Authenticated();
				_licenseCheckRoutine = StartCoroutine(StartLicenseCheckSchedule());
			}
			else
			{
				string text = response?.Data?.reason;
				NotAuthenticated(text ?? response?.ErrorMessage);
			}
		}

		private void ProcessValidationTokenResponse(ApiResponse<LicenseValidationInformation> response)
		{
			if (response != null && response.Result == UnityWebRequest.Result.Success && response.Data.success)
			{
				_validationInformation = response.Data;
				authenticationStateInfo.LoadAuthInfoFromLicenseValidation(_validationInformation);
				SaveLicense();
			}
			if (GetExpirationDate() < DateTime.UtcNow)
			{
				LicenseExpired();
			}
			else
			{
				Authenticated();
			}
		}

		public void Register(string licenseKey)
		{
			StartCoroutine(apiHandler.RegisterLicense(licenseKey, ProcessRegistrationResponse));
		}

		public void RunLicenseCheck()
		{
			ValidateRefreshTokenApiRequestModel request = new ValidateRefreshTokenApiRequestModel(_validationInformation.licenseKey, _validationInformation.refreshToken, Application.platform.ToString(), SettingsStore.Version.VersionNumber);
			StartCoroutine(apiHandler.ValidateRefreshToken(request, ProcessValidationTokenResponse));
		}

		private void Authenticated()
		{
			authenticationStateInfo.AuthenticationStatus = AuthenticationStatus.Authenticated;
			authenticationEvents.RaiseAuthenticated();
		}

		private void LicenseExpired()
		{
			authenticationStateInfo.AuthenticationStatus = AuthenticationStatus.LicenseExpired;
			authenticationEvents.RaiseLicenseExpired();
		}

		private void NotAuthenticated(string message)
		{
			authenticationStateInfo.AuthenticationStatus = AuthenticationStatus.NotAuthenticated;
			authenticationEvents.RaiseNotAuthenticated(message);
		}

		private void SaveLicense()
		{
			_encryptService.SaveLicense(_validationInformation.licenseKey);
			string inputData = JsonUtility.ToJson(new LicenseValidationInformation
			{
				expires = _encryptService.AESEncryption(_validationInformation.expires),
				laneNumber = _validationInformation.laneNumber,
				licensedGames = _validationInformation.licensedGames,
				licenseKey = _encryptService.LicenseKey,
				refreshToken = _encryptService.AESEncryption(_validationInformation.refreshToken),
				token = _encryptService.AESEncryption(_validationInformation.token)
			});
			File.WriteAllText(_fullPath, _encryptService.XOREncryptDecrypt(inputData));
		}

		private void SetTemporaryOverride()
		{
			if (_licenseCheckRoutine != null)
			{
				StopCoroutine(_licenseCheckRoutine);
			}
			if (_overrideRoutine != null)
			{
				StopCoroutine(_overrideRoutine);
			}
			_overrideRoutine = StartCoroutine(TemporaryOverrideRoutine());
		}

		private IEnumerator TemporaryOverrideRoutine()
		{
			yield return new WaitForSecondsRealtime(HoursToOverride());
			Start();
		}

		private IEnumerator StartLicenseCheckSchedule()
		{
			while (Application.isPlaying)
			{
				RunLicenseCheck();
				yield return new WaitForSecondsRealtime(HoursToWaitInSeconds());
				yield return new WaitUntil(UpdateHelper.CheckForUpdates);
			}
		}

		private float HoursToWaitInSeconds()
		{
			return hoursToWait * 60f * 60f;
		}

		private float HoursToOverride()
		{
			return HoursToWaitInSeconds() * 2f;
		}
	}
}
