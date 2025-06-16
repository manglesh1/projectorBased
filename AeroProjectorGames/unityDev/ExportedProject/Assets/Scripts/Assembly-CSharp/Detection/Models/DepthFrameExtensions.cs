using Intel.RealSense;
using UnityEngine;

namespace Detection.Models
{
	public static class DepthFrameExtensions
	{
		public static DepthFrame Copy(this DepthFrame model)
		{
			byte[] array = new byte[model.DataSize];
			model.CopyTo(array);
			DepthFrame depthFrame = Frame.Create<DepthFrame>(model);
			depthFrame.CopyFrom(array);
			return depthFrame;
		}

		public static int GetDistanceInMillimeters(this DepthFrame frame, int x, int y)
		{
			return (int)(frame.GetDistance(x, y) * 1000f);
		}

		public static Vector3Int[] ToVector3IntArray(this DepthFrame depth)
		{
			int width = depth.Width;
			int num = width * depth.Height;
			ushort[] array = new ushort[num];
			Vector3Int[] array2 = new Vector3Int[num];
			int num2 = 0;
			int y = 0;
			depth.CopyTo(array);
			for (int i = 0; i < num; i++)
			{
				if (i % width == 0)
				{
					num2 = 0;
					y = i / width;
				}
				array2[i] = new Vector3Int(num2, y, array[i]);
				num2++;
			}
			return array2;
		}
	}
}
