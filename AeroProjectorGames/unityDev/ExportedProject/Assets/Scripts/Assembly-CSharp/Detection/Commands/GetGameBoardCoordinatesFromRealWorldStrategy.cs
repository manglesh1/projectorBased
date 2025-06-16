using Assets.Scripts.Detection.Commands;
using Detection.Models;
using Detection.ScriptableObjects;
using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/GetGameBoardCoordinatesFromRealWorldStrategy")]
	public class GetGameBoardCoordinatesFromRealWorldStrategy : ScriptableObject
	{
		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		[SerializeField]
		private GetAdjustedRealWorldCoordinatesCommand getDownwardOffset;

		[SerializeField]
		private GetTransformedPointCommand getPoint;

		[SerializeField]
		private IsInBoundsCommand isInBounds;

		public GameBoardCoordinateResponse Execute(Vector2 gameBoardSize, Vector2Int realWorldCoordinates, bool allowOutOfBounds = false)
		{
			GameBoardCoordinateResponse gameBoardCoordinateResponse = new GameBoardCoordinateResponse
			{
				RealWorldCoordinates = realWorldCoordinates
			};
			if (isInBounds.Execute(realWorldCoordinates))
			{
				gameBoardCoordinateResponse.Status = GameBoardStatusEnum.Hit;
				gameBoardCoordinateResponse.GameBoardCoordinates = getDownwardOffset.Execute(gameBoardCoordinates.GetGameBoardLocationByScale(gameBoardSize, realWorldCoordinates, allowOutOfBounds));
			}
			else if (realWorldCoordinates == default(Vector2Int))
			{
				gameBoardCoordinateResponse.Status = GameBoardStatusEnum.ObjectNotDetected;
			}
			else
			{
				gameBoardCoordinateResponse.Status = GameBoardStatusEnum.Miss;
				gameBoardCoordinateResponse.GameBoardCoordinates = (allowOutOfBounds ? getDownwardOffset.Execute(gameBoardCoordinates.GetGameBoardLocationByScale(gameBoardSize, realWorldCoordinates, allowOutOfBounds)) : Vector2Int.zero);
			}
			return gameBoardCoordinateResponse;
		}
	}
}
