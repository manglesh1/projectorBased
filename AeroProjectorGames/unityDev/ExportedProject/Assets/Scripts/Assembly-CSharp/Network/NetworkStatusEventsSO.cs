using System;
using UnityEngine;
using UnityEngine.Events;

namespace Network
{
	[CreateAssetMenu(menuName = "Network/Network Events")]
	public class NetworkStatusEventsSO : ScriptableObject
	{
		public event UnityAction OnFailure;

		public event UnityAction<Action> OnNetworkGuard;

		public event UnityAction OnDisplayNetworkStatusModal;

		public event UnityAction OnSuccess;

		public event UnityAction<NetworkStatus> OnStatusChange;

		public void RaiseFailure()
		{
			this.OnFailure?.Invoke();
		}

		public void RaiseNetworkGuard(Action action)
		{
			this.OnNetworkGuard?.Invoke(action);
		}

		public void RaiseDisplayNetworkStatusModal()
		{
			this.OnDisplayNetworkStatusModal?.Invoke();
		}

		public void RaiseStatusChange(NetworkStatus newStatus)
		{
			this.OnStatusChange?.Invoke(newStatus);
		}

		public void RaiseSuccess()
		{
			this.OnSuccess?.Invoke();
		}
	}
}
