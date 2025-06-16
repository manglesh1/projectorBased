using System;
using API;
using API.ResponseModels;
using Admin_Panel.ApiResponses;
using UnityEngine;
using UnityEngine.Networking;

public class GetLastBackupSettingsDateTimeCommand : MonoBehaviour
{
	[SerializeField]
	private AxcitementApiHandler _apiAdapter;

	private Action<DateTime?> _callBack;

	public void Execute(Action<DateTime?> resultsCallback)
	{
		_callBack = resultsCallback;
		StartCoroutine(_apiAdapter.GetLicenseBackupSettings(ProcessResponse));
	}

	private void ProcessResponse(ApiResponse<GetLicenseBackupSettingsResponse> response)
	{
		DateTime? obj = null;
		if (response.Result == UnityWebRequest.Result.Success && response.Data.Success)
		{
			obj = response.Data.Data?.LastModifiedDateTime;
		}
		_callBack(obj);
	}
}
