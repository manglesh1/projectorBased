using Detection.Models;
using Detection.ScriptableObjects;
using UnityEngine;

namespace Detection.Controllers
{
	public class RealSensePipelineViewController : MonoBehaviour
	{
		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private bool resetOnDisable;

		[SerializeField]
		private ProcessingProfileEnum selectedProcessingProfile;

		private void OnEnable()
		{
			cameraSettings.SetProcessingProfile(selectedProcessingProfile);
		}

		private void OnDisable()
		{
			if (resetOnDisable)
			{
				cameraSettings.SetProcessingProfile(ProcessingProfileEnum.PlaneBreakingProcessingProfile);
			}
		}
	}
}
