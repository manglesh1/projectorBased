using Detection.Models;
using Settings;
using UnityEngine;

namespace Detection.Managers
{
	public class HitDetectionDeviceManager : MonoBehaviour
	{
		[Header("Oak D Device")]
		[SerializeField]
		private GameObject[] oakDObjects;

		[Header("Real Sense Device")]
		[SerializeField]
		private GameObject[] realSenseObjects;

		private void Start()
		{
			switch (SettingsStore.DetectionSettings.DetectedCamera)
			{
			case DetectedCameraEnum.OakD:
				UseOakD();
				break;
			default:
				UseNoDevice();
				break;
			case DetectedCameraEnum.RealSense:
				UseRealSense();
				break;
			}
		}

		private void SetActiveDevices(bool active, GameObject[] cameraObjects)
		{
			for (int i = 0; i < cameraObjects.Length; i++)
			{
				cameraObjects[i].SetActive(active);
			}
		}

		private void UseNoDevice()
		{
			SetActiveDevices(active: false, oakDObjects);
			SetActiveDevices(active: false, realSenseObjects);
		}

		private void UseOakD()
		{
			SetActiveDevices(active: false, realSenseObjects);
			SetActiveDevices(active: true, oakDObjects);
		}

		private void UseRealSense()
		{
			SetActiveDevices(active: false, oakDObjects);
			SetActiveDevices(active: true, realSenseObjects);
		}
	}
}
