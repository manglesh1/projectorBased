using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/GetAdjustedRealWorldCoordinatesCommand")]
	public class GetAdjustedRealWorldCoordinatesCommand : ScriptableObject
	{
		[SerializeField]
		private GetDownwardScaleCommand getScaledOffset;

		public Vector2Int Execute(Vector2Int realWorldCoordinates)
		{
			return new Vector2Int
			{
				x = realWorldCoordinates.x,
				y = realWorldCoordinates.y + getScaledOffset.Execute()
			};
		}
	}
}
