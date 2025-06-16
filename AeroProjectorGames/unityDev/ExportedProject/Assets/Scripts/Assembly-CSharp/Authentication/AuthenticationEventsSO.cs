using UnityEngine;
using UnityEngine.Events;

namespace Authentication
{
	[CreateAssetMenu(menuName = "Authentication/AuthenticationEventsSO")]
	public class AuthenticationEventsSO : ScriptableObject
	{
		public event UnityAction OnAuthenticated;

		public event UnityAction OnInformationModalRequest;

		public event UnityAction OnLicenseCheck;

		public event UnityAction OnLicenseExpired;

		public event UnityAction<string> OnNotAuthenticated;

		public event UnityAction<string> OnRegistrationRequest;

		public event UnityAction OnResetLicense;

		public event UnityAction OnTemporaryOverride;

		public void RaiseAuthenticated()
		{
			this.OnAuthenticated?.Invoke();
		}

		public void RaiseInformationModalRequest()
		{
			this.OnInformationModalRequest?.Invoke();
		}

		public void RaiseLicenseCheck()
		{
			this.OnLicenseCheck?.Invoke();
		}

		public void RaiseLicenseExpired()
		{
			this.OnLicenseExpired?.Invoke();
		}

		public void RaiseRegistrationRequest(string licenseKey)
		{
			this.OnRegistrationRequest?.Invoke(licenseKey);
		}

		public void RaiseNotAuthenticated(string message)
		{
			this.OnNotAuthenticated?.Invoke(message);
		}

		public void RaiseResetLicense()
		{
			this.OnResetLicense?.Invoke();
		}

		public void RaiseTemporaryOverride()
		{
			this.OnTemporaryOverride?.Invoke();
		}
	}
}
