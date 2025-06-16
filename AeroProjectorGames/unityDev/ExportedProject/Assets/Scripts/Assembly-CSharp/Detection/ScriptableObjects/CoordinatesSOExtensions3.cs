using Unity.Mathematics;
using UnityEngine;

namespace Detection.ScriptableObjects
{
	public static class CoordinatesSOExtensions3
	{
		public static int GetMaxBottom(this CoordinatesSO model)
		{
			if (model.BottomLeft.y <= model.BottomRight.y)
			{
				return model.BottomRight.y;
			}
			return model.BottomLeft.y;
		}

		public static int GetMaxLeft(this CoordinatesSO model)
		{
			if (model.TopLeft.x <= model.BottomLeft.x)
			{
				return model.BottomLeft.x;
			}
			return model.TopLeft.x;
		}

		public static int GetMaxRight(this CoordinatesSO model)
		{
			if (model.TopRight.x <= model.BottomRight.x)
			{
				return model.BottomRight.x;
			}
			return model.TopRight.x;
		}

		public static int GetMinLeft(this CoordinatesSO model)
		{
			if (model.TopLeft.x >= model.BottomLeft.x)
			{
				return model.BottomLeft.x;
			}
			return model.TopLeft.x;
		}

		public static int GetMinRight(this CoordinatesSO model)
		{
			if (model.TopRight.x >= model.BottomRight.x)
			{
				return model.BottomRight.x;
			}
			return model.TopRight.x;
		}

		public static int GetMinTop(this CoordinatesSO model)
		{
			if (model.TopLeft.y >= model.TopRight.y)
			{
				return model.TopRight.y;
			}
			return model.TopLeft.y;
		}

		private static float GetSlope(Vector2Int point1, Vector2Int point2)
		{
			float num = point2.x - point1.x;
			return (float)(point2.y - point1.y) / num;
		}

		public static float GetBottomSlope(this CoordinatesSO model)
		{
			return GetSlope(model.BottomLeft, model.BottomRight);
		}

		public static float GetLeftSlope(this CoordinatesSO model)
		{
			return GetSlope(model.TopLeft, model.BottomLeft);
		}

		public static float GetRightSlope(this CoordinatesSO model)
		{
			return GetSlope(model.TopRight, model.BottomRight);
		}

		public static float GetTopSlope(this CoordinatesSO model)
		{
			return GetSlope(model.TopLeft, model.TopRight);
		}

		public static int GetSlopeBottomBoundaryOfX(this CoordinatesSO model, int x)
		{
			return (int)math.ceil(model.GetBottomSlope() * (float)(x - model.BottomLeft.x) + (float)model.BottomLeft.y);
		}

		public static int GetSlopeLeftBoundaryOfX(this CoordinatesSO model, int x)
		{
			float leftSlope = model.GetLeftSlope();
			int num = x - model.TopLeft.x;
			return (int)math.floor(leftSlope * (float)num + (float)model.TopLeft.y);
		}

		public static int GetSlopeLeftBoundaryOfY(this CoordinatesSO model, int y)
		{
			if (y <= model.TopLeft.y)
			{
				return model.TopLeft.x;
			}
			if (y >= model.BottomLeft.y)
			{
				return model.BottomLeft.x;
			}
			return (int)math.floor((float)(y - model.TopLeft.y) / model.GetLeftSlope() + (float)model.TopLeft.x);
		}

		public static int GetSlopeRightBoundaryOfX(this CoordinatesSO model, int x)
		{
			float rightSlope = model.GetRightSlope();
			int num = x - model.TopRight.x;
			return (int)math.floor(rightSlope * (float)num + (float)model.TopRight.y);
		}

		public static int GetSlopeRightBoundaryOfY(this CoordinatesSO model, int y)
		{
			if (y <= model.TopRight.y)
			{
				return model.TopRight.x;
			}
			if (y >= model.BottomRight.y)
			{
				return model.BottomRight.x;
			}
			return (int)math.ceil((float)(y - model.TopRight.y) / model.GetRightSlope() + (float)model.TopRight.x);
		}

		public static int GetMaxDepthAtX(this CoordinatesSO model, int x)
		{
			int maxLeft = model.GetMaxLeft();
			model.GetMaxRight();
			model.GetMinLeft();
			int minRight = model.GetMinRight();
			if (x < maxLeft)
			{
				return model.GetSlopeLeftBoundaryOfX(x);
			}
			if (x > minRight)
			{
				return model.GetSlopeRightBoundaryOfX(x);
			}
			return model.GetSlopeBottomBoundaryOfX(x);
		}
	}
}
