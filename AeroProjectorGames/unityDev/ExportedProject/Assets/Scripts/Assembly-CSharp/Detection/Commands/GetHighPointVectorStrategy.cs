using System.Linq;
using Detection.ScriptableObjects;
using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/GetHighPointVectorStrategy")]
	public class GetHighPointVectorStrategy : ScriptableObject
	{
		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		private const int FLOOR_OFFSET = 300;

		public Vector3Int GetVector(Vector3Int[] vectors)
		{
			return vectors.Where(delegate(Vector3Int v)
			{
				Vector3Int vector3Int = v;
				int result;
				if (vector3Int.z > 0)
				{
					vector3Int = v;
					result = ((vector3Int.z < gameBoardCoordinates.GroundTruth - 300) ? 1 : 0);
				}
				else
				{
					result = 0;
				}
				return (byte)result != 0;
			}).OrderBy(delegate(Vector3Int v)
			{
				Vector3Int vector3Int = v;
				return vector3Int.z;
			}).FirstOrDefault();
		}
	}
}
