namespace Detection.ScriptableObjects
{
	public static class RoiCoordinatesSOExtensions
	{
		private const int SCALE_DOWN_MULTIPLIER = 2;

		public static int Height(this RoiCoordinatesSO model)
		{
			return model.BottomRight.y - model.TopLeft.y;
		}

		public static int Width(this RoiCoordinatesSO model)
		{
			return model.BottomRight.x - model.TopLeft.x;
		}

		public static int GetScaledDownHeight(this RoiCoordinatesSO model)
		{
			int num = model.Height();
			if (num != 0)
			{
				return num / 2;
			}
			return 0;
		}

		public static int GetScaledDownWidth(this RoiCoordinatesSO model)
		{
			int num = model.Width();
			if (num != 0)
			{
				return num / 2;
			}
			return 0;
		}
	}
}
