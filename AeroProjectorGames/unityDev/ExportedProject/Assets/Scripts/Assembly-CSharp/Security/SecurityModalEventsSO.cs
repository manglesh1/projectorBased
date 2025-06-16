using System;
using UnityEngine;
using UnityEngine.Events;

namespace Security
{
	[CreateAssetMenu(menuName = "Security/Security Modal Events")]
	public class SecurityModalEventsSO : ScriptableObject
	{
		public UnityEvent<Action> OnPinAuthenticationRequest = new UnityEvent<Action>();

		public UnityEvent<Action> OnSupportCodeModalRequest = new UnityEvent<Action>();

		public void RaisePinAuthenticationRequest(Action successAuthentication)
		{
			OnPinAuthenticationRequest?.Invoke(successAuthentication);
		}

		public void RaiseSupportCodeModalRequest(Action success)
		{
			OnSupportCodeModalRequest?.Invoke(success);
		}
	}
}
