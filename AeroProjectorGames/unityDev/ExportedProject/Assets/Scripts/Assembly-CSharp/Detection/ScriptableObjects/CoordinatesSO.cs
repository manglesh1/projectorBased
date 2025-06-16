using UnityEngine;
using UnityEngine.Events;

namespace Detection.ScriptableObjects
{
	[CreateAssetMenu(fileName = "Data", menuName = "Detection/ScriptableObjects/CoordinatesSO", order = 1)]
	public class CoordinatesSO : ScriptableObject
	{
		[SerializeField]
		private Vector2Int bottomLeft;

		[SerializeField]
		private Vector2Int bottomRight;

		[SerializeField]
		private decimal downwardOffset;

		[SerializeField]
		private int gameBoardHeight;

		[SerializeField]
		private int gameBoardWidth;

		[SerializeField]
		private int groundTruth;

		[SerializeField]
		private Vector2Int topLeft;

		[SerializeField]
		private Vector2Int topRight;

		public Vector2Int BottomLeft
		{
			get
			{
				return bottomLeft;
			}
			set
			{
				bottomLeft = value;
				HandleVector2IntChanged(this.OnBottomLeftChanged, value);
			}
		}

		public Vector2Int BottomRight
		{
			get
			{
				return bottomRight;
			}
			set
			{
				bottomRight = value;
				HandleVector2IntChanged(this.OnBottomRightChanged, value);
			}
		}

		public decimal DownwardOffset
		{
			get
			{
				return downwardOffset;
			}
			set
			{
				downwardOffset = value;
				HandleDecimalChanged(this.OnDownwardOffsetChanged, value);
			}
		}

		public int GameBoardHeight
		{
			get
			{
				return gameBoardHeight;
			}
			set
			{
				gameBoardHeight = value;
				HandleIntChanged(this.OnGameBoardHeightChanged, value);
			}
		}

		public int GameBoardWidth
		{
			get
			{
				return gameBoardWidth;
			}
			set
			{
				gameBoardWidth = value;
				HandleIntChanged(this.OnGameBoardWidthChanged, value);
			}
		}

		public int GroundTruth
		{
			get
			{
				return groundTruth;
			}
			set
			{
				groundTruth = value;
				HandleIntChanged(this.OnGroundTruthChanged, value);
			}
		}

		public Vector2Int TopLeft
		{
			get
			{
				return topLeft;
			}
			set
			{
				topLeft = value;
				HandleVector2IntChanged(this.OnTopLeftChanged, value);
			}
		}

		public Vector2Int TopRight
		{
			get
			{
				return topRight;
			}
			set
			{
				topRight = value;
				HandleVector2IntChanged(this.OnTopRightChanged, value);
			}
		}

		public event UnityAction<Vector2Int> OnBottomLeftChanged;

		public event UnityAction<Vector2Int> OnBottomRightChanged;

		public event UnityAction<decimal> OnDownwardOffsetChanged;

		public event UnityAction OnChanged;

		public event UnityAction<int> OnGameBoardHeightChanged;

		public event UnityAction<int> OnGameBoardWidthChanged;

		public event UnityAction<int> OnGroundTruthChanged;

		public event UnityAction<Vector2Int> OnTopLeftChanged;

		public event UnityAction<Vector2Int> OnTopRightChanged;

		private void HandleDecimalChanged(UnityAction<decimal> decimalChangedEvent, decimal value)
		{
			decimalChangedEvent?.Invoke(value);
			this.OnChanged?.Invoke();
		}

		private void HandleIntChanged(UnityAction<int> intChangedEvent, int value)
		{
			intChangedEvent?.Invoke(value);
			this.OnChanged?.Invoke();
		}

		private void HandleVector2IntChanged(UnityAction<Vector2Int> vector2IntChangedEvent, Vector2Int value)
		{
			vector2IntChangedEvent?.Invoke(value);
			this.OnChanged?.Invoke();
		}
	}
}
