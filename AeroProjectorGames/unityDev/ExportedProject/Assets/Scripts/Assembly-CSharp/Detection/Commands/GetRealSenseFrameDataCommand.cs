using System;
using System.Collections.Generic;
using Detection.Models;
using Intel.RealSense;
using UnityEngine;

namespace Detection.Commands
{
	public class GetRealSenseFrameDataCommand : MonoBehaviour
	{
		private const int TIMEOUT_DURATION = 10000;

		[SerializeField]
		private VectorEventHandler vectorEventHandler;

		private List<DepthFrame> _frames;

		private int _maxFrames;

		public IEnumerable<DepthFrame> Execute(int frameCount)
		{
			if (frameCount <= 0)
			{
				return Array.Empty<DepthFrame>();
			}
			_maxFrames = frameCount;
			_frames = new List<DepthFrame>(_maxFrames);
			DateTime dateTime = DateTime.Now.AddMilliseconds(10000.0);
			StartListeningForFrames();
			while (_frames.Count < _maxFrames && !(DateTime.Now > dateTime))
			{
			}
			StopListeningForFrames();
			return _frames;
		}

		public IEnumerable<DepthFrame> Execute()
		{
			return Execute(1);
		}

		private void HandleFrameReceived(DepthFrame frameData)
		{
			if (_frames.Count < _maxFrames)
			{
				_frames.Add(frameData.Copy());
			}
			else
			{
				StopListeningForFrames();
			}
		}

		private void StartListeningForFrames()
		{
			if (vectorEventHandler != null)
			{
				vectorEventHandler.OnFrameReceived += HandleFrameReceived;
			}
		}

		private void StopListeningForFrames()
		{
			if (vectorEventHandler != null)
			{
				vectorEventHandler.OnFrameReceived -= HandleFrameReceived;
			}
		}
	}
}
