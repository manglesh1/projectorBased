using Detection.ScriptableObjects;
using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/VectorOfInterestFactory")]
	public class VectorOfInterestFactory : ScriptableObject
	{
		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		[SerializeField]
		private GetCenterPointVectorStrategy getCenterPoint;

		[SerializeField]
		private GetHighPointVectorStrategy getHighPoint;

		public Vector3Int GetVector(Vector3Int[] vectors)
		{
			if (gameBoardCoordinates.GroundTruth != 0)
			{
				return getHighPoint.GetVector(vectors);
			}
			return getCenterPoint.GetVector(vectors);
		}
	}
}
