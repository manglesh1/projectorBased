using System;
using Network;
using TMPro;
using UnityEngine;

namespace Authentication.Authentication_Information_Modal
{
	public class AuthenticationInformationModalController : MonoBehaviour
	{
		private const string DAYS = "days";

		[SerializeField]
		private GameObject authenticationInformationModalPanel;

		[Header("UI Elements")]
		[SerializeField]
		private TextMeshProUGUI laneNumberText;

		[SerializeField]
		private TextMeshProUGUI licenseExpirationText;

		[SerializeField]
		private TextMeshProUGUI licenseKeyText;

		[SerializeField]
		private TextMeshProUGUI tokenStatusText;

		[SerializeField]
		private TextMeshProUGUI networkStatusText;

		[Header("External References")]
		[SerializeField]
		private AuthenticationEventsSO authenticationEvents;

		[SerializeField]
		private AuthenticationStateSO authenticationState;

		[SerializeField]
		private NetworkStateSO networkState;

		private void OnEnable()
		{
			authenticationEvents.OnInformationModalRequest += OpenInformationModal;
		}

		private void OnDisable()
		{
			authenticationEvents.OnInformationModalRequest -= OpenInformationModal;
		}

		private void OpenInformationModal()
		{
			int days = authenticationState.TokenExpiration.Subtract(DateTime.UtcNow).Days;
			string text = ((days > 0) ? days.ToString() : "0") + " days";
			laneNumberText.text = authenticationState.LaneNumber.ToString();
			licenseExpirationText.text = text;
			licenseKeyText.text = authenticationState.LicenseKey;
			tokenStatusText.text = authenticationState.AuthenticationStatus.ToString();
			networkStatusText.text = networkState.Status.ToString();
			authenticationInformationModalPanel.SetActive(value: true);
		}

		public void Close()
		{
			authenticationInformationModalPanel.SetActive(value: false);
		}
	}
}
