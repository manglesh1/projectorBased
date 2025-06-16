using Detection.Models;
using Intel.RealSense;

namespace Detection.Factories
{
	public static class DetectedCameraFactory
	{
		public static DetectedCameraEnum DetectCamera()
		{
			if (UseRealSense())
			{
				return DetectedCameraEnum.RealSense;
			}
			if (UseOakD())
			{
				return DetectedCameraEnum.OakD;
			}
			return DetectedCameraEnum.None;
		}

		private static bool UseOakD()
		{
			return false;
		}

		private static bool UseRealSense()
		{
			using (Context context = new Context())
			{
				return context.QueryDevices().Count > 0;
			}
		}
	}
}
