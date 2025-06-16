using Detection.Commands;
using Detection.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	[RequireComponent(typeof(Image))]
	public class RoiOverlayController : MonoBehaviour
	{
		private RectTransform _rect;

		private Image _image;

		private Color _currentColor;

		[SerializeField]
		private RoiCoordinatesSO ROI;

		[SerializeField]
		private VectorEventHandler vectorEventHandler;

		private void OnDisable()
		{
			ROI.OnBottomRightChanged -= HandleRoiChange;
			ROI.OnTopLeftChanged -= HandleRoiChange;
			vectorEventHandler.OnObjectDetected -= ShowDetectionInROI;
			vectorEventHandler.OnObjectRemoved -= RemoveDetectionInROI;
		}

		private void OnEnable()
		{
			_rect = GetComponent<RectTransform>();
			_image = GetComponent<Image>();
			_currentColor = Color.yellow;
			HandleRoiChange();
			ROI.OnBottomRightChanged += HandleRoiChange;
			ROI.OnTopLeftChanged += HandleRoiChange;
			vectorEventHandler.OnObjectDetected += ShowDetectionInROI;
			vectorEventHandler.OnObjectRemoved += RemoveDetectionInROI;
		}

		private void Update()
		{
			if (_currentColor != _image.color)
			{
				_image.color = _currentColor;
			}
		}

		private void HandleRoiChange()
		{
			_rect.anchoredPosition = new Vector2(ROI.TopLeftScaledDown.x, ROI.TopLeftScaledDown.y * -1);
			_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ROI.GetScaledDownHeight());
			_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ROI.GetScaledDownWidth());
		}

		private void RemoveDetectionInROI()
		{
			_currentColor = Color.yellow;
		}

		private void ShowDetectionInROI(Vector3Int _)
		{
			_currentColor = Color.red;
		}
	}
}
