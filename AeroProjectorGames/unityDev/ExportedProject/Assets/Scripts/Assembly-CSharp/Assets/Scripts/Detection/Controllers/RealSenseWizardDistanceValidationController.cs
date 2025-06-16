using Assets.Scripts.Detection.ScriptableObjects;
using Detection.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Detection.Controllers
{
	public class RealSenseWizardDistanceValidationController : MonoBehaviour
	{
		[Header("UI Components")]
		[SerializeField]
		private TMP_Text textField;

		[Header("Events")]
		[SerializeField]
		private RealSenseWizardEventsSO events;

		[Header("Scriptable Objects")]
		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		private const string MESSAGE_BAD = "The minimum and maximum distances are conflicting. Either your minimum distance is greater than the maximum distance or the maximum distance is below the minimum distance. Please fix this issue before continuing.";

		private const string MESSAGE_GOOD = "The next two screens are meant to be repeated as necessary by using the Next and Back buttons. The exposure, ROI, and boundaries are volatile settings and repeated use of these screens will help to dial in your setup.";

		private bool IsValid()
		{
			return cameraSettings.MinDistance < cameraSettings.MaxDistance && cameraSettings.MaxDistance > cameraSettings.MinDistance;
		}

		private void OnDisable()
		{
			events.RaiseOnRoadBlockResolved();
		}

		private void OnEnable()
		{
			if (IsValid())
			{
				textField.text = "The next two screens are meant to be repeated as necessary by using the Next and Back buttons. The exposure, ROI, and boundaries are volatile settings and repeated use of these screens will help to dial in your setup.";
				return;
			}
			events.RaiseOnRoadBlocked();
			textField.text = "The minimum and maximum distances are conflicting. Either your minimum distance is greater than the maximum distance or the maximum distance is below the minimum distance. Please fix this issue before continuing.";
		}
	}
}
