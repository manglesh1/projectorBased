using System;
using UnityEngine;

namespace Network
{
	public class NetworkStatusManager : MonoBehaviour
	{
		[SerializeField]
		private NetworkStateSO networkState;

		[SerializeField]
		private NetworkStatusEventsSO networkStatusEvents;

		protected void OnEnable()
		{
			networkStatusEvents.OnFailure += HandleNetworkFailures;
			networkStatusEvents.OnNetworkGuard += HandleNetworkGuard;
			networkStatusEvents.OnSuccess += HandleNetworkSuccess;
		}

		protected void OnDisable()
		{
			networkStatusEvents.OnFailure -= HandleNetworkFailures;
			networkStatusEvents.OnNetworkGuard -= HandleNetworkGuard;
			networkStatusEvents.OnSuccess -= HandleNetworkSuccess;
		}

		private void HandleNetworkFailures()
		{
			SetStatus(NetworkStatus.Offline);
		}

		private void HandleNetworkGuard(Action action)
		{
			if (networkState.Status == NetworkStatus.Offline)
			{
				networkStatusEvents.RaiseDisplayNetworkStatusModal();
			}
			else
			{
				action();
			}
		}

		private void HandleNetworkSuccess()
		{
			if (networkState.Status != NetworkStatus.Online)
			{
				SetStatus(NetworkStatus.Online);
			}
		}

		private void SetStatus(NetworkStatus status)
		{
			networkState.Status = status;
			networkStatusEvents.RaiseStatusChange(networkState.Status);
		}
	}
}
