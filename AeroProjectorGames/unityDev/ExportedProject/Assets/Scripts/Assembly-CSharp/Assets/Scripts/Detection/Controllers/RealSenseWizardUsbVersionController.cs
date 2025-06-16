using Assets.Scripts.Detection.ScriptableObjects;
using Intel.RealSense;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Detection.Controllers
{
	public class RealSenseWizardUsbVersionController : MonoBehaviour
	{
		[Header("UI Components")]
		[SerializeField]
		private TMP_Text wizardText;

		[Header("Events")]
		[SerializeField]
		private RealSenseWizardEventsSO wizardEvents;

		private const string GOOD_VERSION = "3.2";

		private const string MESSAGE_BAD = "The camera is communicating using the wrong version of USB. This is typically caused by either being plugged into the wrong USB port on the computer or problems with the cabling. Please make sure that the camera is plugged into the ports marked USB 1 or USB 2. These ports are easily identifiable because of the blue tab on the inside of the port.";

		private const string MESSAGE_GOOD = "The camera is connected using USB version 3.2";

		private bool CheckUsbVersion()
		{
			using (Context context = new Context())
			{
				if (context.QueryDevices().Count == 0)
				{
					return false;
				}
				return context.Devices[0].Info[CameraInfo.UsbTypeDescriptor].Equals("3.2");
			}
		}

		private void OnEnable()
		{
			if (CheckUsbVersion())
			{
				wizardEvents.RaiseOnRoadBlockResolved();
				wizardText.text = "The camera is connected using USB version 3.2";
			}
			else
			{
				wizardEvents.RaiseOnRoadBlocked();
				wizardText.text = "The camera is communicating using the wrong version of USB. This is typically caused by either being plugged into the wrong USB port on the computer or problems with the cabling. Please make sure that the camera is plugged into the ports marked USB 1 or USB 2. These ports are easily identifiable because of the blue tab on the inside of the port.";
			}
		}
	}
}
