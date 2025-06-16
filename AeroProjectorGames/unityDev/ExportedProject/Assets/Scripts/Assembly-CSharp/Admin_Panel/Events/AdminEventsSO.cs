using UnityEngine;
using UnityEngine.Events;

namespace Admin_Panel.Events
{
	[CreateAssetMenu(menuName = "Admin/Admin Events")]
	public class AdminEventsSO : ScriptableObject
	{
		public event UnityAction OnAdminPanelOpened;

		public event UnityAction OnAdminPanelClosed;

		public event UnityAction OnAdminPinReset;

		public void RaiseAdminPanelClosed()
		{
			this.OnAdminPanelClosed?.Invoke();
		}

		public void RaiseAdminPanelOpened()
		{
			this.OnAdminPanelOpened?.Invoke();
		}

		public void RaiseAdminPinReset()
		{
			this.OnAdminPinReset?.Invoke();
		}
	}
}
