using System;
using API;
using API.ResponseModels;
using Admin_Panel.ApiResponses;
using UnityEngine;
using UnityEngine.Networking;

public class GetBackupSettingsCommand : MonoBehaviour
{
	[SerializeField]
	private AxcitementApiHandler _apiAdapter;

	private Action<string> _callback;

	public void Execute(Action<string> responseCallback)
	{
		_callback = responseCallback;
		StartCoroutine(_apiAdapter.GetLicenseBackupSettings(ProcessResponse));
	}

	private void ProcessResponse(ApiResponse<GetLicenseBackupSettingsResponse> response)
	{
		string obj = null;
		if (response.Result == UnityWebRequest.Result.Success && response.Data.Success)
		{
			obj = response.Data.Data?.SettingsData;
		}
		_callback(obj);
	}
}
