using System.Linq;
using Detection.ScriptableObjects;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using UnityEngine;

namespace Assets.Scripts.Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/GetTransformedPointCommand")]
	public class GetTransformedPointCommand : ScriptableObject
	{
		[SerializeField]
		private CoordinatesSO boundaryCoordinates;

		private MatOfPoint2f BuildDestinationMatrix(Vector2 gameBoardSize)
		{
			return new MatOfPoint2f(new Point(0.0, 0.0), new Point(gameBoardSize.x, 0.0), new Point(gameBoardSize.x, gameBoardSize.y), new Point(0.0, gameBoardSize.y));
		}

		private MatOfPoint2f BuildSourceMatrix()
		{
			return new MatOfPoint2f(new Point(boundaryCoordinates.TopLeft.x, boundaryCoordinates.TopLeft.y), new Point(boundaryCoordinates.TopRight.x, boundaryCoordinates.TopRight.y), new Point(boundaryCoordinates.BottomRight.x, boundaryCoordinates.BottomRight.y), new Point(boundaryCoordinates.BottomLeft.x, boundaryCoordinates.BottomLeft.y));
		}

		public Vector2Int Execute(Vector2 gameBoardSize, Vector2Int realWorldPoint)
		{
			Mat src = new MatOfPoint2f(new Point(realWorldPoint.x, realWorldPoint.y));
			MatOfPoint2f matOfPoint2f = new MatOfPoint2f();
			MatOfPoint2f srcPoints = BuildSourceMatrix();
			MatOfPoint2f dstPoints = BuildDestinationMatrix(gameBoardSize);
			Mat m = Calib3d.findHomography(srcPoints, dstPoints);
			Core.perspectiveTransform(src, matOfPoint2f, m);
			Point[] array = matOfPoint2f.toArray();
			if (array == null || !array.Any())
			{
				return Vector2Int.zero;
			}
			return new Vector2Int((int)array[0].x, (int)array[0].y);
		}
	}
}
