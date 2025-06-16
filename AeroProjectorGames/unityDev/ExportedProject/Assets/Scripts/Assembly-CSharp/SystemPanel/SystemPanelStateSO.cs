using UnityEngine;
using UnityEngine.Events;

namespace SystemPanel
{
	[CreateAssetMenu(menuName = "System Panel State")]
	public class SystemPanelStateSO : ScriptableObject
	{
		public SystemPanelVisibilityState visibilityState = SystemPanelVisibilityState.Closed;

		public UnityAction OnToggle;

		private void OnEnable()
		{
			visibilityState = SystemPanelVisibilityState.Closed;
		}

		public void RaiseToggle()
		{
			SystemPanelVisibilityState systemPanelVisibilityState = visibilityState;
			if (systemPanelVisibilityState <= SystemPanelVisibilityState.PanelOpen)
			{
				visibilityState = SystemPanelVisibilityState.Closed;
			}
			else
			{
				visibilityState = SystemPanelVisibilityState.PinEntry;
			}
			OnToggle?.Invoke();
		}
	}
}
