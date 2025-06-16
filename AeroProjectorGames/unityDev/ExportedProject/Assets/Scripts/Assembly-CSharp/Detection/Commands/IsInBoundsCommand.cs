using System;
using Detection.Models;
using Detection.ScriptableObjects;
using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/IsInBoundsCommand")]
	public class IsInBoundsCommand : ScriptableObject
	{
		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		[SerializeField]
		private GetDownwardScaleCommand getDownwardScale;

		public bool Execute(Vector2Int realWorldCoordinates)
		{
			GameBoardQuadrantEnum fromRealWorldCoordinates = GameBoardQuadrantEnumExtensions.GetFromRealWorldCoordinates(gameBoardCoordinates.GetMidPoint(), realWorldCoordinates);
			int ratioBottomBoundary = gameBoardCoordinates.GetRatioBottomBoundary(realWorldCoordinates.x);
			int ratioLeftBoundary = gameBoardCoordinates.GetRatioLeftBoundary(realWorldCoordinates.y);
			int ratioRightBoundary = gameBoardCoordinates.GetRatioRightBoundary(realWorldCoordinates.y);
			int num = gameBoardCoordinates.GetMinTop() - getDownwardScale.Execute();
			switch (fromRealWorldCoordinates)
			{
			case GameBoardQuadrantEnum.TopLeft:
				return realWorldCoordinates.x >= ratioLeftBoundary && realWorldCoordinates.y >= num;
			case GameBoardQuadrantEnum.TopRight:
				return realWorldCoordinates.x <= ratioRightBoundary && realWorldCoordinates.y >= num;
			case GameBoardQuadrantEnum.BottomRight:
				return realWorldCoordinates.x <= ratioRightBoundary && realWorldCoordinates.y <= ratioBottomBoundary;
			case GameBoardQuadrantEnum.BottomLeft:
				return realWorldCoordinates.x >= ratioLeftBoundary && realWorldCoordinates.y <= ratioBottomBoundary;
			default:
				throw new InvalidOperationException("Unknown GameBoardQuadrant");
			}
		}
	}
}
