using Security;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
	[CreateAssetMenu(menuName = "Views/View Events SO")]
	public class ViewEventsSO : ScriptableObject
	{
		[SerializeField]
		private ViewStateSO viewState;

		[SerializeField]
		private SecurityModalEventsSO securityEvents;

		public event UnityAction OnCloseView;

		public event UnityAction OnOpenView;

		public void CloseView()
		{
			this.OnCloseView?.Invoke();
			viewState.SetClosed();
		}

		public void OpenView()
		{
			this.OnOpenView?.Invoke();
			viewState.SetOpen();
		}

		public void ToggleView()
		{
			Debug.Log("Toggle");
			if (viewState.PanelVisibilityEnum == PanelVisibilityEnum.Closed)
			{
				Debug.Log("open");
				OpenView();
			}
			else
			{
				Debug.Log("Close");
				CloseView();
			}
		}

		public void ToggleViewWithSecurity()
		{
			if (viewState.PanelVisibilityEnum == PanelVisibilityEnum.Closed)
			{
				securityEvents.RaisePinAuthenticationRequest(OpenView);
			}
			else
			{
				CloseView();
			}
		}
	}
}
