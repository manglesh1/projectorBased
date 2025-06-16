using Assets.Scripts.Detection.ScriptableObjects;
using Intel.RealSense;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Detection.Controllers
{
	public class RealSenseWizardCameraDetectedController : MonoBehaviour
	{
		[Header("UI Components")]
		[SerializeField]
		private TMP_Text wizardText;

		[Header("Events")]
		[SerializeField]
		private RealSenseWizardEventsSO wizardEvents;

		private const string MESSAGE_BAD = "The camera was not found";

		private const string MESSAGE_GOOD = "The camera was detected.";

		private bool CameraExists()
		{
			using (Context context = new Context())
			{
				return context.QueryDevices().Count > 0;
			}
		}

		private void OnEnable()
		{
			if (CameraExists())
			{
				wizardEvents.RaiseOnRoadBlockResolved();
				wizardText.text = "The camera was detected.";
			}
			else
			{
				wizardEvents.RaiseOnRoadBlocked();
				wizardText.text = "The camera was not found";
			}
		}
	}
}
