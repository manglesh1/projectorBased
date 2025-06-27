using System;
using Detection.Models;
using UnityEngine;

namespace Detection.ScriptableObjects
{
	public static class CoordinatesSOExtensions
	{
		public static Vector2Int GetGameBoardLocation(this CoordinatesSO model, Vector2 gameboardSize, Vector2Int realWorldLocation, bool allowOutOfBounds = false)
		{
			return GameBoardQuadrantEnumExtensions.GetFromRealWorldCoordinates(model.GetMidPoint(), realWorldLocation) switch
			{
				GameBoardQuadrantEnum.TopLeft => new Vector2Int((int)(model.GetTopWidthPercent(realWorldLocation.x, allowOutOfBounds) * (decimal)gameboardSize.x), (int)(model.GetLeftHeightPercent(realWorldLocation.y, allowOutOfBounds) * (decimal)gameboardSize.y)), 
				GameBoardQuadrantEnum.TopRight => new Vector2Int((int)(model.GetTopWidthPercent(realWorldLocation.x, allowOutOfBounds) * (decimal)gameboardSize.x), (int)(model.GetRightHeightPercent(realWorldLocation.y, allowOutOfBounds) * (decimal)gameboardSize.y)), 
				GameBoardQuadrantEnum.BottomRight => new Vector2Int((int)(model.GetBottomWidthPercent(realWorldLocation.x, allowOutOfBounds) * (decimal)gameboardSize.x), (int)(model.GetRightHeightPercent(realWorldLocation.y, allowOutOfBounds) * (decimal)gameboardSize.y)), 
				GameBoardQuadrantEnum.BottomLeft => new Vector2Int((int)(model.GetBottomWidthPercent(realWorldLocation.x, allowOutOfBounds) * (decimal)gameboardSize.x), (int)(model.GetLeftHeightPercent(realWorldLocation.y, allowOutOfBounds) * (decimal)gameboardSize.y)), 
				_ => throw new InvalidOperationException("Unknown quadrant"), 
			};
		}

		public static int GetRatioBottomBoundary(this CoordinatesSO model, int x)
		{
			return (int)((decimal)model.GetBottomOffset() * model.GetBottomWidthPercent(x)) + model.BottomLeft.y;
		}

		public static int GetRatioLeftBoundary(this CoordinatesSO model, int y)
		{
			return (int)((decimal)model.GetLeftOffset() * model.GetLeftHeightPercent(y)) + model.TopLeft.x;
		}

		public static int GetRatioRightBoundary(this CoordinatesSO model, int y)
		{
			return (int)((decimal)model.GetRightOffset() * model.GetRightHeightPercent(y)) + model.TopRight.x;
		}

		public static int GetRatioTopBoundary(this CoordinatesSO model, int x)
		{
			return (int)((decimal)model.GetTopOffset() * model.GetTopWidthPercent(x)) + model.TopLeft.y;
		}

		private static int GetLeftHeight(this CoordinatesSO model)
		{
			return model.BottomLeft.y - model.TopLeft.y;
		}

		private static int GetRightHeight(this CoordinatesSO model)
		{
			return model.BottomRight.y - model.TopRight.y;
		}

		private static int GetTopWidth(this CoordinatesSO model)
		{
			return model.TopRight.x - model.TopLeft.x;
		}

		private static int GetBottomWidth(this CoordinatesSO model)
		{
			return model.BottomRight.x - model.BottomLeft.x;
		}

		private static int GetBottomOffset(this CoordinatesSO model)
		{
			return model.BottomRight.y - model.BottomLeft.y;
		}

		private static int GetLeftOffset(this CoordinatesSO model)
		{
			return model.BottomLeft.x - model.TopLeft.x;
		}

		private static int GetRightOffset(this CoordinatesSO model)
		{
			return model.BottomRight.x - model.TopRight.x;
		}

		private static int GetTopOffset(this CoordinatesSO model)
		{
			return model.TopRight.y - model.TopLeft.y;
		}

		private static decimal GetBoundedPercent(decimal unboundedPercent)
		{
			if (unboundedPercent < 0m)
			{
				return 0m;
			}
			if (unboundedPercent > 1m)
			{
				return 1m;
			}
			return unboundedPercent;
		}

		private static decimal GetLeftHeightPercent(this CoordinatesSO model, int y, bool allowOutOfBounds = false)
		{
			if (!allowOutOfBounds)
			{
				return GetBoundedPercent((y - model.TopLeft.y) / model.GetLeftHeight());
			}
			return (y - model.TopLeft.y) / model.GetLeftHeight();
		}

		private static decimal GetRightHeightPercent(this CoordinatesSO model, int y, bool allowOutOfBounds = false)
		{
			if (!allowOutOfBounds)
			{
				return GetBoundedPercent((y - model.TopRight.y) / model.GetRightHeight());
			}
			return (y - model.TopRight.y) / model.GetRightHeight();
		}

		private static decimal GetBottomWidthPercent(this CoordinatesSO model, int x, bool allowOutOfBounds = false)
		{
			if (!allowOutOfBounds)
			{
				return GetBoundedPercent((x - model.BottomLeft.x) / model.GetBottomWidth());
			}
			return (x - model.BottomLeft.x) / model.GetBottomWidth();
		}

		private static decimal GetTopWidthPercent(this CoordinatesSO model, int x, bool allowOutOfBounds = false)
		{
			if (!allowOutOfBounds)
			{
				return GetBoundedPercent((x - model.TopLeft.x) / model.GetTopWidth());
			}
			return (x - model.TopLeft.x) / model.GetTopWidth();
		}

		public static Vector2Int GetMidPoint(this CoordinatesSO model)
		{
			Vector2Int midPointFromBottomLeftToTopRight = model.GetMidPointFromBottomLeftToTopRight();
			Vector2Int midPointFromTopLeftToBottomRight = model.GetMidPointFromTopLeftToBottomRight();
			return new Vector2Int((midPointFromBottomLeftToTopRight.x + midPointFromTopLeftToBottomRight.x) / 2, (midPointFromBottomLeftToTopRight.y + midPointFromTopLeftToBottomRight.y) / 2);
		}

		private static Vector2Int GetMidPointFromBottomLeftToTopRight(this CoordinatesSO model)
		{
			return new Vector2Int((model.TopRight.x + model.BottomLeft.x) / 2, (model.BottomLeft.y + model.TopRight.y) / 2);
		}

		private static Vector2Int GetMidPointFromTopLeftToBottomRight(this CoordinatesSO model)
		{
			return new Vector2Int((model.BottomRight.x + model.TopLeft.x) / 2, (model.BottomRight.y + model.TopLeft.y) / 2);
		}
	}
}
