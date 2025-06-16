using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Authentication
{
	public class ExpiredLicenseController : MonoBehaviour
	{
		private const string NO_LICENSE_KEY = "No License Key Found";

		[SerializeField]
		private TMP_Text validationErrorMessage;

		[SerializeField]
		private Button tryAgainButton;

		[Header("View License Elements")]
		[SerializeField]
		private GameObject viewLicenseModal;

		[SerializeField]
		private TMP_Text licenseKeyText;

		[Header("External References")]
		[SerializeField]
		private AuthenticationStateSO authenticationStateInfo;

		[SerializeField]
		private AuthenticationEventsSO authenticationEvents;

		public void OnEnable()
		{
			authenticationEvents.OnLicenseExpired += HandleLicenseExpired;
			validationErrorMessage.text = string.Empty;
			tryAgainButton.interactable = true;
			viewLicenseModal.SetActive(value: false);
		}

		public void OnDisable()
		{
			authenticationEvents.OnLicenseExpired -= HandleLicenseExpired;
		}

		public void DisplayLicenseNumber()
		{
			licenseKeyText.text = ((authenticationStateInfo.LicenseKey != string.Empty) ? authenticationStateInfo.LicenseKey : "No License Key Found");
			viewLicenseModal.SetActive(value: true);
		}

		public void TryAgain()
		{
			tryAgainButton.interactable = false;
			validationErrorMessage.text = "Running...";
			authenticationEvents.RaiseLicenseCheck();
		}

		private void HandleLicenseExpired()
		{
			tryAgainButton.interactable = true;
			validationErrorMessage.text = "Validation failed, please wait a few minutes and try again";
		}
	}
}
