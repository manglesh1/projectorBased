using UnityEngine;

namespace Detection.Models
{
	public class GameBoardCoordinateResponse
	{
		public Vector2Int GameBoardCoordinates { get; set; }

		public Vector2Int RealWorldCoordinates { get; set; }

		public GameBoardStatusEnum Status { get; set; }
	}
}
