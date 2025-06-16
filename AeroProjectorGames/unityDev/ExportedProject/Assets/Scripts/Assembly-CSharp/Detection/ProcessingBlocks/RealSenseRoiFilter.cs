using System.Collections.Generic;
using Detection.ScriptableObjects;
using Intel.RealSense;
using UnityEngine;

namespace Detection.ProcessingBlocks
{
	[ProcessingBlockData(typeof(RealSenseRoiFilter))]
	public class RealSenseRoiFilter : RsProcessingBlock
	{
		[SerializeField]
		private bool allowAllYAxis;

		[SerializeField]
		private CoordinatesSO boundaryCoordinates;

		[SerializeField]
		private RoiCoordinatesSO roiCoordinates;

		private void OnDisable()
		{
		}

		private Frame ApplyFilter(DepthFrame depth, FrameSource frameSource)
		{
			int width = depth.Width;
			int num = width * depth.Height;
			ushort[] array = new ushort[num];
			int num2 = ((!allowAllYAxis) ? (width * roiCoordinates.TopLeft.y + boundaryCoordinates.TopLeft.x) : 0);
			int num3 = (allowAllYAxis ? num : (width * roiCoordinates.BottomRight.y + boundaryCoordinates.BottomRight.x));
			int maxRight = boundaryCoordinates.GetMaxRight();
			int minLeft = boundaryCoordinates.GetMinLeft();
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			depth.CopyTo(array);
			for (int i = 0; i < num; i++)
			{
				if (i % width == 0)
				{
					int num7 = i / width;
					num6 = 0;
					num4 = width * num7 + minLeft;
					num5 = width * num7 + maxRight;
				}
				if (i < num2 || i > num3)
				{
					array[i] = 0;
				}
				else if (i < num4 || i > num5)
				{
					array[i] = 0;
				}
				else if (array[i] > boundaryCoordinates.GetMaxDepthAtX(num6))
				{
					array[i] = 0;
				}
				num6++;
			}
			using (StreamProfile profile = depth.Profile)
			{
				DepthFrame depthFrame = frameSource.AllocateVideoFrame<DepthFrame>(profile, depth, depth.BitsPerPixel, depth.Width, depth.Height, depth.Stride, Extension.DepthFrame);
				depthFrame.CopyFrom(array);
				return depthFrame;
			}
		}

		public override Frame Process(Frame frame, FrameSource frameSource)
		{
			if (frame.IsComposite)
			{
				using (FrameSet frameSet = FrameSet.FromFrame(frame))
				{
					using (DepthFrame depth = frameSet.DepthFrame)
					{
						List<Frame> list = new List<Frame>(frameSet.Count);
						foreach (Frame item in frameSet)
						{
							using (StreamProfile streamProfile = item.Profile)
							{
								if (streamProfile.Stream == Stream.Depth && streamProfile.Format == Format.Z16)
								{
									item.Dispose();
									continue;
								}
							}
							list.Add(item);
						}
						list.Add(ApplyFilter(depth, frameSource));
						FrameSet frameSet2 = frameSource.AllocateCompositeFrame(list);
						list.ForEach(delegate(Frame f)
						{
							f.Dispose();
						});
						using (frameSet2)
						{
							return frameSet2.AsFrame();
						}
					}
				}
			}
			return frame;
		}
	}
}
