using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class CloseDetectionMenuController : MonoBehaviour
	{
		[SerializeField]
		private Button closeButton;

		[SerializeField]
		private DetectionUIEventsSO detectionUiEvents;

		private void OnDisable()
		{
			closeButton.onClick.RemoveListener(detectionUiEvents.CloseDetectionUI);
		}

		private void OnEnable()
		{
			closeButton.onClick.AddListener(detectionUiEvents.CloseDetectionUI);
		}
	}
}
