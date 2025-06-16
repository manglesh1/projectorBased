using System;
using Detection.Models;
using UnityEngine;

namespace Detection.ScriptableObjects
{
	public static class CoordinateSOExtensions2
	{
		public static Vector2Int GetGameBoardLocationByScale(this CoordinatesSO model, Vector2 gameboardSize, Vector2Int realWorldLocation, bool allowOutOfBounds = false)
		{
			GameBoardQuadrantEnum fromRealWorldCoordinates = GameBoardQuadrantEnumExtensions.GetFromRealWorldCoordinates(model.GetMidPoint(), realWorldLocation);
			return new Vector2Int(model.GetGameBoardX(fromRealWorldCoordinates, gameboardSize, realWorldLocation), model.GetGameBoardY(fromRealWorldCoordinates, gameboardSize, realWorldLocation));
		}

		private static int GetGameBoardX(this CoordinatesSO model, GameBoardQuadrantEnum quadrant, Vector2 gameBoardSize, Vector2Int realWorldLocation)
		{
			decimal percentOfHeight = model.GetPercentOfHeight(quadrant, realWorldLocation);
			int leftOffset = model.GetLeftOffset();
			int rightOffset = model.GetRightOffset();
			decimal num = (decimal)leftOffset * percentOfHeight;
			decimal num2 = (decimal)rightOffset * percentOfHeight;
			decimal num3 = (decimal)model.TopLeft.x + num;
			decimal num4 = (decimal)model.TopRight.x + num2 - num3;
			decimal num5 = ((decimal)realWorldLocation.x - num3) / num4;
			return (int)Math.Round((decimal)gameBoardSize.x * num5, 0);
		}

		private static int GetGameBoardY(this CoordinatesSO model, GameBoardQuadrantEnum quadrant, Vector2 gameBoardSize, Vector2Int realWorldLocation)
		{
			decimal percentOfWidth = model.GetPercentOfWidth(quadrant, realWorldLocation);
			int topOffset = model.GetTopOffset();
			int bottomOffset = model.GetBottomOffset();
			decimal num = (decimal)topOffset * percentOfWidth;
			decimal num2 = (decimal)bottomOffset * percentOfWidth;
			decimal num3 = (decimal)model.TopLeft.y + num;
			decimal num4 = (decimal)model.BottomLeft.y + num2 - num3;
			decimal num5 = ((decimal)realWorldLocation.y - num3) / num4;
			return (int)Math.Round((decimal)gameBoardSize.y * num5, 0);
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

		private static int GetHeightOfLeft(this CoordinatesSO model)
		{
			return model.BottomLeft.y - model.TopLeft.y;
		}

		private static int GetHeightOfRight(this CoordinatesSO model)
		{
			return model.BottomRight.y - model.TopRight.y;
		}

		private static int GetWidthOfBottom(this CoordinatesSO model)
		{
			return model.BottomRight.x - model.BottomLeft.x;
		}

		private static int GetWidthOfTop(this CoordinatesSO model)
		{
			return model.TopRight.x - model.TopLeft.x;
		}

		private static decimal GetPercentOfHeight(this CoordinatesSO model, GameBoardQuadrantEnum quadrant, Vector2Int realWorldLocation)
		{
			switch (quadrant)
			{
			case GameBoardQuadrantEnum.TopLeft:
				return (realWorldLocation.y - model.TopLeft.y) / model.GetHeightOfLeft();
			case GameBoardQuadrantEnum.TopRight:
				return (realWorldLocation.y - model.TopRight.y) / model.GetHeightOfRight();
			case GameBoardQuadrantEnum.BottomRight:
				return (realWorldLocation.y - model.TopRight.y) / model.GetHeightOfRight();
			case GameBoardQuadrantEnum.BottomLeft:
				return (realWorldLocation.y - model.TopLeft.y) / model.GetHeightOfLeft();
			default:
				throw new InvalidOperationException("Unknown quadrant");
			}
		}

		private static decimal GetPercentOfWidth(this CoordinatesSO model, GameBoardQuadrantEnum quadrant, Vector2Int realWorldLocation)
		{
			switch (quadrant)
			{
			case GameBoardQuadrantEnum.TopLeft:
				return (realWorldLocation.x - model.TopLeft.x) / model.GetWidthOfTop();
			case GameBoardQuadrantEnum.TopRight:
				return (realWorldLocation.x - model.TopLeft.x) / model.GetWidthOfTop();
			case GameBoardQuadrantEnum.BottomRight:
				return (realWorldLocation.x - model.BottomLeft.x) / model.GetWidthOfBottom();
			case GameBoardQuadrantEnum.BottomLeft:
				return (realWorldLocation.x - model.BottomLeft.x) / model.GetWidthOfBottom();
			default:
				throw new InvalidOperationException("Unknown quadrant");
			}
		}
	}
}
