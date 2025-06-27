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
			return fromRealWorldCoordinates switch
			{
				GameBoardQuadrantEnum.TopLeft => realWorldCoordinates.x >= ratioLeftBoundary && realWorldCoordinates.y >= num, 
				GameBoardQuadrantEnum.TopRight => realWorldCoordinates.x <= ratioRightBoundary && realWorldCoordinates.y >= num, 
				GameBoardQuadrantEnum.BottomRight => realWorldCoordinates.x <= ratioRightBoundary && realWorldCoordinates.y <= ratioBottomBoundary, 
				GameBoardQuadrantEnum.BottomLeft => realWorldCoordinates.x >= ratioLeftBoundary && realWorldCoordinates.y <= ratioBottomBoundary, 
				_ => throw new InvalidOperationException("Unknown GameBoardQuadrant"), 
			};
		}
	}
}
