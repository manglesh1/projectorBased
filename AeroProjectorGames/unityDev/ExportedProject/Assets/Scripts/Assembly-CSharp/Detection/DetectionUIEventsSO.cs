using Security;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Detection
{
	[CreateAssetMenu(menuName = "Detection/UI/Detection UI Events")]
	public class DetectionUIEventsSO : ScriptableObject
	{
		[SerializeField]
		private DetectionUIStateSO detectionUIState;

		[SerializeField]
		private SecurityModalEventsSO securityEvents;

		public event UnityAction OnCloseDetectionUI;

		public event UnityAction OnOpenDetectionUI;

		public void CloseDetectionUI()
		{
			this.OnCloseDetectionUI?.Invoke();
			detectionUIState.SetClosed();
		}

		public void OpenDetectionUI()
		{
			this.OnOpenDetectionUI?.Invoke();
			detectionUIState.SetOpen();
		}

		public void ToggleDetectionUI()
		{
			if (detectionUIState.PanelVisibilityEnum == PanelVisibilityEnum.Closed)
			{
				securityEvents.RaisePinAuthenticationRequest(OpenDetectionUI);
			}
			else
			{
				CloseDetectionUI();
			}
		}
	}
}
