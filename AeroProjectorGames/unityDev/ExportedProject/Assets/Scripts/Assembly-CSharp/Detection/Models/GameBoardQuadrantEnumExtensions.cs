using UnityEngine;

namespace Detection.Models
{
	public static class GameBoardQuadrantEnumExtensions
	{
		public static GameBoardQuadrantEnum GetFromRealWorldCoordinates(Vector2Int realWorldMidPoint, Vector2Int realWorldLocation)
		{
			if (realWorldLocation.x <= realWorldMidPoint.x)
			{
				if (realWorldLocation.y > realWorldMidPoint.y)
				{
					return GameBoardQuadrantEnum.BottomLeft;
				}
				return GameBoardQuadrantEnum.TopLeft;
			}
			if (realWorldLocation.y > realWorldMidPoint.y)
			{
				return GameBoardQuadrantEnum.BottomRight;
			}
			return GameBoardQuadrantEnum.TopRight;
		}
	}
}
