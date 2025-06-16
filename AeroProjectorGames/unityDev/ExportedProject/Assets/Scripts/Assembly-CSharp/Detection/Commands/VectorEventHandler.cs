using Intel.RealSense;
using UnityEngine;
using UnityEngine.Events;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/VectorEventHandler", order = 3)]
	public class VectorEventHandler : ScriptableObject
	{
		public event UnityAction<DepthFrame> OnFrameReceived;

		public event UnityAction<Vector3Int> OnObjectDetected;

		public event UnityAction OnObjectRemoved;

		public event UnityAction OnWalkAwayElapsed;

		public void RaiseFrameReceived(DepthFrame frame)
		{
			this.OnFrameReceived?.Invoke(frame);
		}

		public void RaiseObjectDetected(Vector3Int vector)
		{
			this.OnObjectDetected?.Invoke(vector);
		}

		public void RaiseObjectRemoved()
		{
			this.OnObjectRemoved?.Invoke();
		}

		public void RaiseWalkAwayElapsed()
		{
			this.OnWalkAwayElapsed?.Invoke();
		}
	}
}
