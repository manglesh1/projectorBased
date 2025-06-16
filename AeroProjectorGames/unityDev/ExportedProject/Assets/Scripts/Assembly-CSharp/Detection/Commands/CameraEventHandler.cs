using UnityEngine;
using UnityEngine.Events;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/CameraEventHandler", order = 2)]
	public class CameraEventHandler : ScriptableObject
	{
		public event UnityAction<Vector2Int> OnClicked;

		public void RaiseOnClicked(Vector2Int point)
		{
			this.OnClicked?.Invoke(point);
		}
	}
}
