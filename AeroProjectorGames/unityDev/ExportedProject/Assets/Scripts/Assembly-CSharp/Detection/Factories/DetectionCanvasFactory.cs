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
			return SettingsStore.DetectionSettings.DetectedCamera switch
			{
				DetectedCameraEnum.OakD => oakDDetectionCanvas, 
				DetectedCameraEnum.RealSense => realSenseDetectionCanvas, 
				_ => blankGameObject, 
			};
		}
	}
}
