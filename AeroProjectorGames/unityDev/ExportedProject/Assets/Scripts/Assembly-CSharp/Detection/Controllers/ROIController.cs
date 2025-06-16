using Detection.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Detection.Controllers
{
	public class ROIController : MonoBehaviour
	{
		[SerializeField]
		private RoiCoordinatesSO Coordinates;

		[SerializeField]
		private TMP_InputField BottomX;

		[SerializeField]
		private TMP_InputField BottomY;

		[SerializeField]
		private TMP_InputField TopX;

		[SerializeField]
		private TMP_InputField TopY;

		private void BottomRightCoordinateChanged()
		{
			BottomX.text = Coordinates.BottomRight.x.ToString();
			BottomY.text = Coordinates.BottomRight.y.ToString();
		}

		private void BottomXChanged(string value)
		{
			int.TryParse(value, out var result);
			Coordinates.BottomRight = new Vector2Int(result, Coordinates.BottomRight.y);
		}

		private void BottomYChanged(string value)
		{
			int.TryParse(value, out var result);
			Coordinates.BottomRight = new Vector2Int(Coordinates.BottomRight.x, result);
		}

		private void TopLeftCoordinateChanged()
		{
			TopX.text = Coordinates.TopLeft.x.ToString();
			TopY.text = Coordinates.TopLeft.y.ToString();
		}

		private void TopXChanged(string value)
		{
			int.TryParse(value, out var result);
			Coordinates.TopLeft = new Vector2Int(result, Coordinates.TopLeft.y);
		}

		private void TopYChanged(string value)
		{
			int.TryParse(value, out var result);
			Coordinates.TopLeft = new Vector2Int(Coordinates.TopLeft.x, result);
		}

		private void OnDisable()
		{
			BottomX.onValueChanged.RemoveListener(BottomXChanged);
			BottomY.onValueChanged.RemoveListener(BottomYChanged);
			TopX.onValueChanged.RemoveListener(TopXChanged);
			TopY.onValueChanged.RemoveListener(TopYChanged);
			Coordinates.OnBottomRightChanged -= BottomRightCoordinateChanged;
			Coordinates.OnTopLeftChanged -= TopLeftCoordinateChanged;
		}

		private void OnEnable()
		{
			BottomX.onValueChanged.AddListener(BottomXChanged);
			BottomY.onValueChanged.AddListener(BottomYChanged);
			TopY.onValueChanged.AddListener(TopYChanged);
			TopX.onValueChanged.AddListener(TopXChanged);
			Coordinates.OnBottomRightChanged += BottomRightCoordinateChanged;
			Coordinates.OnTopLeftChanged += TopLeftCoordinateChanged;
			BottomX.text = Coordinates.BottomRight.x.ToString();
			BottomY.text = Coordinates.BottomRight.y.ToString();
			TopX.text = Coordinates.TopLeft.x.ToString();
			TopY.text = Coordinates.TopLeft.y.ToString();
		}
	}
}
