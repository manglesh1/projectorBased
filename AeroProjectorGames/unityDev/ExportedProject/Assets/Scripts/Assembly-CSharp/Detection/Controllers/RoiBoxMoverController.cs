using Detection.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class RoiBoxMoverController : MonoBehaviour
	{
		private const int MAX_X_POSITION = 640;

		private const int MAX_Y_POSITION = 380;

		[Header("Size Controls")]
		[SerializeField]
		private Button incrementWidth;

		[SerializeField]
		private Button decrementWidth;

		[Header("Position Controls")]
		[Space]
		[SerializeField]
		private Button incrementPositionX;

		[SerializeField]
		private Button incrementPositionY;

		[SerializeField]
		private Button decrementPositionX;

		[SerializeField]
		private Button decrementPositionY;

		[Header("Increment Size")]
		[Space]
		[SerializeField]
		[Range(1f, 50f)]
		private int largeIncrements;

		[SerializeField]
		[Range(1f, 50f)]
		private int smallIncrements;

		[Header("Increment Toggle")]
		[Space]
		[SerializeField]
		private Toggle largeIncrementsToggle;

		[Header("Filtering")]
		[Space]
		[SerializeField]
		private Toggle roiFilterToggle;

		[Header("References")]
		[Space]
		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private RoiCoordinatesSO ROI;

		private void OnDisable()
		{
			incrementWidth.onClick.RemoveListener(HandleIncrementWidth);
			decrementWidth.onClick.RemoveListener(HandleDecrementWidth);
			incrementPositionX.onClick.RemoveListener(HandleIncrementPositionX);
			incrementPositionY.onClick.RemoveListener(HandleIncrementPositionY);
			decrementPositionX.onClick.RemoveListener(HandleDecrementPositionX);
			decrementPositionY.onClick.RemoveListener(HandleDecrementPositionY);
		}

		private void OnEnable()
		{
			largeIncrementsToggle.isOn = false;
			incrementWidth.onClick.AddListener(HandleIncrementWidth);
			decrementWidth.onClick.AddListener(HandleDecrementWidth);
			incrementPositionX.onClick.AddListener(HandleIncrementPositionX);
			incrementPositionY.onClick.AddListener(HandleIncrementPositionY);
			decrementPositionX.onClick.AddListener(HandleDecrementPositionX);
			decrementPositionY.onClick.AddListener(HandleDecrementPositionY);
		}

		private int Decrement(int valueToIncrement)
		{
			return valueToIncrement - (largeIncrementsToggle.isOn ? largeIncrements : smallIncrements);
		}

		private void HandleDecrementWidth()
		{
			ROI.BottomRight = new Vector2Int(Decrement(ROI.BottomRight.x), ROI.BottomRight.y);
		}

		private void HandleIncrementWidth()
		{
			ROI.BottomRight = new Vector2Int(Increment(ROI.BottomRight.x, 640), ROI.BottomRight.y);
		}

		private void HandleDecrementPositionX()
		{
			ROI.TopLeft = new Vector2Int(Decrement(ROI.TopLeft.x), ROI.TopLeft.y);
			ROI.BottomRight = new Vector2Int(Decrement(ROI.BottomRight.x), ROI.BottomRight.y);
		}

		private void HandleDecrementPositionY()
		{
			ROI.TopLeft = new Vector2Int(ROI.TopLeft.x, Decrement(ROI.TopLeft.y));
			ROI.BottomRight = new Vector2Int(ROI.BottomRight.x, Decrement(ROI.BottomRight.y));
		}

		private void HandleIncrementPositionX()
		{
			ROI.TopLeft = new Vector2Int(Increment(ROI.TopLeft.x, 640), ROI.TopLeft.y);
			ROI.BottomRight = new Vector2Int(Increment(ROI.BottomRight.x, 640), ROI.BottomRight.y);
		}

		private void HandleIncrementPositionY()
		{
			ROI.TopLeft = new Vector2Int(ROI.TopLeft.x, Increment(ROI.TopLeft.y, 380));
			ROI.BottomRight = new Vector2Int(ROI.BottomRight.x, Increment(ROI.BottomRight.y, 380));
		}

		private int Increment(int valueToIncrement, int maxBounds)
		{
			return valueToIncrement + (largeIncrementsToggle.isOn ? largeIncrements : smallIncrements);
		}
	}
}
