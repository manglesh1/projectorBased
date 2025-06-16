using Detection.Models;
using Settings;
using UnityEngine;

namespace Detection.Factories
{
	public class DetectionCanvasFactory : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] blankGameObject;

		[SerializeField]
		private GameObject[] oakDDetectionCanvas;

		[SerializeField]
		private GameObject[] realSenseDetectionCanvas;

		public GameObject[] GetDetectionCanvas()
		{
			switch (SettingsStore.DetectionSettings.DetectedCamera)
			{
			case DetectedCameraEnum.OakD:
				return oakDDetectionCanvas;
			default:
				return blankGameObject;
			case DetectedCameraEnum.RealSense:
				return realSenseDetectionCanvas;
			}
		}
	}
}
