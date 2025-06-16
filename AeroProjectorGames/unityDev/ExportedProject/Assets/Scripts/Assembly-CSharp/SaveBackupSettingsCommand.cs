using System;
using API;
using API.ResponseModels;
using Admin_Panel.ApiResponses;
using Settings;
using UnityEngine;
using UnityEngine.Networking;

public class SaveBackupSettingsCommand : MonoBehaviour
{
	[SerializeField]
	private AxcitementApiHandler _apiAdapter;

	private Action<bool, DateTime?> _callback;

	public void Execute(Action<bool, DateTime?> responseCallback)
	{
		_callback = responseCallback;
		SettingsRepository settingsRepository = new SettingsRepository();
		StartCoroutine(_apiAdapter.SaveLicenseBackupSettings(settingsRepository.GetRawSettingsFile(), ProcessResponse));
	}

	private void ProcessResponse(ApiResponse<SaveLicenseBackupSettingsResponse> response)
	{
		if (response.Result == UnityWebRequest.Result.Success && response.Data.Success)
		{
			_callback(arg1: true, response.Data.Data.LastModifiedDateTime);
		}
		else
		{
			_callback(arg1: false, null);
		}
	}
}
