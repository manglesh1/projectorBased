using UnityEngine;
using UnityEngine.Events;

namespace Backgrounds
{
	[CreateAssetMenu(menuName = "Backgrounds/Background Events")]
	public class BackgroundEventsSO : ScriptableObject
	{
		public event UnityAction OnCustomBackgroundsUpdated;

		public event UnityAction OnLoadBackgroundRequest;

		public event UnityAction OnTargetDimmerChanged;

		public event UnityAction OnUpdateCustomBackgroundsRequest;

		public void RaiseOnCustomBackgroundsUpdated()
		{
			this.OnCustomBackgroundsUpdated?.Invoke();
		}

		public void RaiseLoadBackgroundRequest()
		{
			this.OnLoadBackgroundRequest?.Invoke();
		}

		public void RaiseTargetDimmerChanged()
		{
			this.OnTargetDimmerChanged?.Invoke();
		}

		public void RaiseUpdateCustomBackgroundsRequest()
		{
			this.OnUpdateCustomBackgroundsRequest?.Invoke();
		}
	}
}
