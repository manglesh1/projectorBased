using ResizingAndMoving;
using UnityEngine;

namespace Games.IATF.Ring_Target.Scripts
{
	public class IATFResizeAndPositionController : MonoBehaviour
	{
		public SizeAndPositionStateSO selectedSizeAndPositionState;

		public void Done()
		{
			selectedSizeAndPositionState.RaiseDoneEditing();
		}

		public void Decrement()
		{
			selectedSizeAndPositionState.Decrement();
		}

		public void DecrementHeight()
		{
			selectedSizeAndPositionState.DecrementHeight();
		}

		public void DecrementWidth()
		{
			selectedSizeAndPositionState.DecrementWidth();
		}

		public void Increment()
		{
			selectedSizeAndPositionState.Increment();
		}

		public void IncrementHeight()
		{
			selectedSizeAndPositionState.IncrementHeight();
		}

		public void IncrementWidth()
		{
			selectedSizeAndPositionState.IncrementWidth();
		}

		public void MoveDown()
		{
			selectedSizeAndPositionState.MoveDown();
		}

		public void MoveLeft()
		{
			selectedSizeAndPositionState.MoveLeft();
		}

		public void MoveRight()
		{
			selectedSizeAndPositionState.MoveRight();
		}

		public void MoveUp()
		{
			selectedSizeAndPositionState.MoveUp();
		}
	}
}
