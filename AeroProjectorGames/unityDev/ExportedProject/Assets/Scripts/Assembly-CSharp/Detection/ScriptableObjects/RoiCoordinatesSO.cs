using UnityEngine;
using UnityEngine.Events;

namespace Detection.ScriptableObjects
{
	[CreateAssetMenu(fileName = "RoiCoordinatesSO", menuName = "Detection/ScriptableObjects/RoiCoordinatesSO", order = 2)]
	public class RoiCoordinatesSO : ScriptableObject
	{
		private const int DEFAULT_ROI_HEIGHT = 50;

		private const int FULL_SCALE_MULTIPLIER = 2;

		[SerializeField]
		private Vector2Int bottomRight;

		[SerializeField]
		private Vector2Int topLeft;

		public Vector2Int BottomRight
		{
			get
			{
				return bottomRight;
			}
			set
			{
				if (IsValidX(topLeft.x, value.x))
				{
					bottomRight.x = value.x;
					SetDefaultHeight();
					HandleChange(this.OnBottomRightChanged);
				}
			}
		}

		public Vector2Int BottomRightScaledDown => GetScaledDownVector(BottomRight.x, BottomRight.y);

		public Vector2Int TopLeft
		{
			get
			{
				return topLeft;
			}
			set
			{
				if (IsValidX(value.x, bottomRight.x))
				{
					topLeft = value;
					SetDefaultHeight();
					HandleChange(this.OnTopLeftChanged);
				}
			}
		}

		public Vector2Int TopLeftScaledDown => GetScaledDownVector(TopLeft.x, TopLeft.y);

		public event UnityAction OnChanged;

		public event UnityAction OnBottomRightChanged;

		public event UnityAction OnTopLeftChanged;

		private Vector2Int GetScaledDownVector(int x, int y)
		{
			return new Vector2Int(x / 2, y / 2);
		}

		private void SetDefaultHeight()
		{
			int y = topLeft.y + 50;
			bottomRight.y = y;
		}

		private bool IsValidX(int topLeftX, int bottomRightX)
		{
			return topLeftX <= bottomRightX;
		}

		private bool IsValidY(int topLeftY, int bottomRightY)
		{
			return topLeftY - bottomRightY == 50;
		}

		private void HandleChange(UnityAction optionalAction = null)
		{
			optionalAction?.Invoke();
			this.OnChanged?.Invoke();
		}
	}
}
