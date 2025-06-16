using Detection.ScriptableObjects;
using UnityEngine;

namespace Detection.Controllers
{
	public class RealSenseProcessingPipeController : MonoBehaviour
	{
		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private RsProcessingPipe pipe;

		public void OnEnable()
		{
			cameraSettings.OnProcessingProfileChanged += HandlePipeChange;
			pipe.profile = cameraSettings.SelectedProcessingProfile;
		}

		public void OnDisable()
		{
			cameraSettings.OnProcessingProfileChanged -= HandlePipeChange;
		}

		public void HandlePipeChange()
		{
			pipe.profile = cameraSettings.SelectedProcessingProfile;
		}
	}
}
