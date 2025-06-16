using System;
using Detection.ScriptableObjects;
using UnityEngine;

namespace Detection.Commands
{
	[CreateAssetMenu(menuName = "Detection/Commands/GetDownwardScaleCommand")]
	public class GetDownwardScaleCommand : ScriptableObject
	{
		[SerializeField]
		private CoordinatesSO gameBoard;

		private const decimal MILLIMETERS_TO_INCH_CONVERSION = 25.4m;

		public int Execute()
		{
			return (int)Math.Round(gameBoard.DownwardOffset * 25.4m, 0);
		}
	}
}
