using Detection.Models;
using Detection.ScriptableObjects;
using Intel.RealSense;
using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/GetDistanceVectorsCommand")]
	public class GetDistanceVectorsCommand : ScriptableObject
	{
		[SerializeField]
		private RoiCoordinatesSO roi;

		public Vector3Int[] Execute(DepthFrame frame)
		{
			Vector3Int[] array = new Vector3Int[(roi.Width() + 1) * (roi.Height() + 1)];
			int num = 0;
			for (int i = 0; i <= roi.Height(); i++)
			{
				int y = i + roi.TopLeft.y;
				for (int j = 0; j <= roi.Width(); j++)
				{
					int x = j + roi.TopLeft.x;
					int distanceInMillimeters = frame.GetDistanceInMillimeters(x, y);
					array[num++] = new Vector3Int(x, y, distanceInMillimeters);
				}
			}
			return array;
		}
	}
}
