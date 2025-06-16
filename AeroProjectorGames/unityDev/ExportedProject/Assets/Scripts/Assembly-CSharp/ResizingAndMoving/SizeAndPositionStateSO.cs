using System;
using UnityEngine;
using UnityEngine.Events;

namespace ResizingAndMoving
{
	[CreateAssetMenu(menuName = "Resizing/Resizable and Position State")]
	public class SizeAndPositionStateSO : ScriptableObject
	{
		[SerializeField]
		protected float largeSizeIncrementAmount = 20f;

		[SerializeField]
		protected float sizeIncrementAmount = 2f;

		[SerializeField]
		protected float largePositionIncrementAmount = 20f;

		[SerializeField]
		protected float positionIncrementAmount = 2f;

		[SerializeField]
		protected bool useLargeIncrements;

		[SerializeField]
		private float height;

		[SerializeField]
		private float width;

		[SerializeField]
		private float maxHeight = 1200f;

		[SerializeField]
		private float maxWidth = 2000f;

		[SerializeField]
		private float minHeight;

		[SerializeField]
		private float minWidth;

		[SerializeField]
		private float positionX;

		[SerializeField]
		private float positionY;

		public UnityAction OnDoneEditing;

		public UnityAction OnEditing;

		public UnityAction OnPositionChange;

		public UnityAction OnReset;

		public UnityAction OnSizeChange;

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				if (value < minWidth)
				{
					width = minWidth;
				}
				else if (value > maxWidth)
				{
					width = maxWidth;
				}
				else
				{
					width = value;
				}
			}
		}

		public float Height
		{
			get
			{
				return height;
			}
			set
			{
				if (value < minHeight)
				{
					height = minHeight;
				}
				else if (value > maxHeight)
				{
					height = maxHeight;
				}
				else
				{
					height = value;
				}
			}
		}

		public float PositionY => positionY;

		public float PositionX => positionX;

		public void Decrement()
		{
			ExecuteSizeChange(delegate
			{
				Width -= GetSizeIncrementAmount();
				Height -= GetSizeIncrementAmount();
			});
		}

		public void DecrementHeight()
		{
			ExecuteSizeChange(delegate
			{
				Height -= GetSizeIncrementAmount();
			});
		}

		public void DecrementWidth()
		{
			ExecuteSizeChange(delegate
			{
				Width -= GetSizeIncrementAmount();
			});
		}

		public void IncrementHeight()
		{
			ExecuteSizeChange(delegate
			{
				Height += GetSizeIncrementAmount();
			});
		}

		public void IncrementWidth()
		{
			ExecuteSizeChange(delegate
			{
				Width += GetSizeIncrementAmount();
			});
		}

		public void Increment()
		{
			ExecuteSizeChange(delegate
			{
				Width += GetSizeIncrementAmount();
				Height += GetSizeIncrementAmount();
			});
		}

		public void MoveDown()
		{
			ExecutePositionChange(delegate
			{
				positionY -= GetPositionIncrementAmount();
			});
		}

		public void MoveLeft()
		{
			ExecutePositionChange(delegate
			{
				positionX -= GetPositionIncrementAmount();
			});
		}

		public void MoveRight()
		{
			ExecutePositionChange(delegate
			{
				positionX += GetPositionIncrementAmount();
			});
		}

		public void MoveUp()
		{
			ExecutePositionChange(delegate
			{
				positionY += GetPositionIncrementAmount();
			});
		}

		public void RaiseEditing()
		{
			OnEditing?.Invoke();
		}

		public void RaiseDoneEditing()
		{
			OnDoneEditing?.Invoke();
		}

		public void RaiseReset()
		{
			OnReset?.Invoke();
			OnSizeChange?.Invoke();
			OnPositionChange?.Invoke();
		}

		public void SetLargeIncrements(bool large)
		{
			useLargeIncrements = large;
		}

		public void SetPosition(float x, float y)
		{
			ExecutePositionChange(delegate
			{
				positionX = x;
				positionY = y;
			});
		}

		public void SetSize(float x, float y)
		{
			ExecuteSizeChange(delegate
			{
				Width = x;
				Height = y;
			});
		}

		private void ExecutePositionChange(Action action)
		{
			action?.Invoke();
			OnPositionChange?.Invoke();
		}

		private void ExecuteSizeChange(Action action)
		{
			action?.Invoke();
			OnSizeChange?.Invoke();
		}

		private float GetPositionIncrementAmount()
		{
			if (!useLargeIncrements)
			{
				return positionIncrementAmount;
			}
			return largePositionIncrementAmount;
		}

		private float GetSizeIncrementAmount()
		{
			if (!useLargeIncrements)
			{
				return sizeIncrementAmount;
			}
			return largeSizeIncrementAmount;
		}
	}
}
