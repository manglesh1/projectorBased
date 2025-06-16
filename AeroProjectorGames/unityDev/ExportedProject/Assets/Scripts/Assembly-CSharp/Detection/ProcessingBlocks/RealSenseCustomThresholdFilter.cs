using Detection.Commands;
using Detection.ScriptableObjects;
using Intel.RealSense;
using UnityEngine;

namespace Detection.ProcessingBlocks
{
	[ProcessingBlockData(typeof(RealSenseCustomThresholdFilter))]
	public class RealSenseCustomThresholdFilter : RsProcessingBlock
	{
		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private GetDownwardScaleCommand getDownwardOffset;

		private ThresholdFilter _pb;

		private IOption minOption;

		private IOption maxOption;

		private void OnDisable()
		{
			if (_pb != null)
			{
				_pb.Dispose();
			}
		}

		public void Init()
		{
			_pb = new ThresholdFilter();
			minOption = _pb.Options[Option.MinDistance];
			maxOption = _pb.Options[Option.MaxDistance];
		}

		public override Frame Process(Frame frame, FrameSource frameSource)
		{
			if (_pb == null)
			{
				Init();
			}
			UpdateOptions();
			return _pb.Process(frame);
		}

		private void UpdateOptions()
		{
			minOption.Value = cameraSettings.MinDistance - (float)getDownwardOffset.Execute() / 1000f;
			maxOption.Value = cameraSettings.MaxDistance;
		}
	}
}
