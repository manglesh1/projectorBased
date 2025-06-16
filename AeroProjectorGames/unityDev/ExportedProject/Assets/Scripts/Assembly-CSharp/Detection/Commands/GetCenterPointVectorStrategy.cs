using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/GetCenterPointVectorStrategy")]
	public class GetCenterPointVectorStrategy : ScriptableObject
	{
		public Vector3Int GetVector(Vector3Int[] vectors)
		{
			return vectors[vectors.Length / 2];
		}
	}
}
