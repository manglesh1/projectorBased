using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Authentication
{
	public class AuthenticationFormController : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField licenseKeyField;

		[SerializeField]
		private TMP_Text authenticationErrorMessage;

		[SerializeField]
		private Button registerButton;

		[SerializeField]
		private AuthenticationEventsSO authenticationEvents;

		public void Awake()
		{
			authenticationEvents.OnNotAuthenticated += HandleNotAuthenticated;
		}

		public void SubmitRegistration()
		{
			registerButton.interactable = false;
			authenticationEvents.RaiseRegistrationRequest(licenseKeyField.text);
		}

		private void HandleNotAuthenticated(string message)
		{
			authenticationErrorMessage.text = message;
			registerButton.interactable = true;
		}
	}
}
