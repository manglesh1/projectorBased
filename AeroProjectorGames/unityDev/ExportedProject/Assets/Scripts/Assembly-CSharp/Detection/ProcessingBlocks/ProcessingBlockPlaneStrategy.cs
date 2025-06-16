using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Detection.Commands;
using Detection.Models;
using Intel.RealSense;
using Settings;
using UnityEngine;

namespace Detection.ProcessingBlocks
{
	[ProcessingBlockData(typeof(ProcessingBlockPlaneStrategy))]
	public class ProcessingBlockPlaneStrategy : RsProcessingBlock
	{
		[SerializeField]
		private GetDistanceVectorsCommand getDistanceVectors;

		[SerializeField]
		private VectorEventHandler vectorHandler;

		[SerializeField]
		private VectorOfInterestFactory voiFactory;

		private readonly Queue<Vector3Int[]> _frameSample;

		private bool _objectDetected;

		private bool _okToThrow;

		private System.Timers.Timer _walkAwayTimer;

		private const int MAX_FRAMES_STORED = 10;

		private const int RESET_DURATION = 5000;

		public ProcessingBlockPlaneStrategy()
		{
			_frameSample = new Queue<Vector3Int[]>(10);
			_objectDetected = false;
			_okToThrow = true;
			_walkAwayTimer = new System.Timers.Timer(5000.0);
			_walkAwayTimer.Elapsed += WalkAwayTimerElapsed;
		}

		private void WalkAwayTimerElapsed(object sender, ElapsedEventArgs args)
		{
			_walkAwayTimer.Stop();
			_okToThrow = true;
			vectorHandler.RaiseWalkAwayElapsed();
		}

		private void OnEnable()
		{
			enabled = true;
			_objectDetected = false;
		}

		private Vector3Int GetVectorOfInterest()
		{
			List<Vector3Int[]> list = _frameSample.ToList();
			List<Vector3Int> list2 = new List<Vector3Int>(10);
			for (int i = 0; i < list.Count; i++)
			{
				Vector3Int item = Vector3Int.zero;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				for (int j = 0; j < list[i].Length; j++)
				{
					Vector3Int vector3Int = list[i][j];
					if (num2 != vector3Int.y)
					{
						num2 = vector3Int.y;
						num++;
					}
					int num4 = vector3Int.z + num;
					if (vector3Int.z > 0 && (item.z == 0 || item.z > num4))
					{
						item = vector3Int;
						item.z = num4;
						num3 = num;
					}
				}
				if (item.z > 0)
				{
					item.z += num3;
				}
				list2.Add(item);
			}
			if (!list2.All((Vector3Int v) => v != Vector3Int.zero))
			{
				return Vector3Int.zero;
			}
			return list2.Median();
		}

		private void MaintainQueueSize()
		{
			if (_frameSample.Count == 10)
			{
				_frameSample.Dequeue();
			}
		}

		public override Frame Process(Frame frame, FrameSource frameSource)
		{
			MaintainQueueSize();
			try
			{
				if (frame.IsComposite)
				{
					using (FrameSet frameSet = FrameSet.FromFrame(frame))
					{
						using (DepthFrame frame2 = frameSet.DepthFrame)
						{
							vectorHandler.RaiseFrameReceived(frame2);
							Vector3Int[] item = getDistanceVectors.Execute(frame2);
							_frameSample.Enqueue(item);
							Vector3Int vectorOfInterest = GetVectorOfInterest();
							if (vectorOfInterest == Vector3Int.zero)
							{
								if (_objectDetected)
								{
									_objectDetected = false;
									ResetWalkAwayTimer();
									vectorHandler.RaiseObjectRemoved();
								}
							}
							else if (_okToThrow)
							{
								if (!_objectDetected)
								{
									_objectDetected = true;
									vectorHandler.RaiseObjectDetected(vectorOfInterest);
								}
							}
							else
							{
								ResetWalkAwayTimer();
							}
						}
					}
				}
			}
			catch (DivideByZeroException)
			{
			}
			return frame;
		}

		private void ResetWalkAwayTimer()
		{
			if (SettingsStore.DetectionSettings.ObjectRemovalDelay)
			{
				_okToThrow = false;
				_walkAwayTimer.Start();
			}
			else
			{
				_okToThrow = true;
			}
		}
	}
}
