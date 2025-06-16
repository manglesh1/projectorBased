using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Detection.Models;
using Detection.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Detection.Commands
{
	public class SetBoundaryCornerCommand : MonoBehaviour
	{
		[SerializeField]
		private VectorEventHandler depthEventHandler;

		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		[SerializeField]
		private RoiCoordinatesSO roiCoordinates;

		[SerializeField]
		private TMP_Text statusText;

		[Header("Touch Points")]
		[SerializeField]
		private GameObject dotBottomLeft;

		[SerializeField]
		private GameObject dotBottomRight;

		[SerializeField]
		private GameObject dotTopLeft;

		[SerializeField]
		private GameObject dotTopRight;

		private bool _captureInterrupted;

		private ConcurrentBag<Vector3Int> _concurrentVectors;

		private bool _failureTimeout;

		private int _negativeStatusIndex;

		private int _noFrameCount;

		private int _positiveStatusIndex;

		private int _processedSeconds;

		private readonly List<Vector3Int> _receivedVectors;

		private bool _running;

		private BoundaryCornerEnum _workingCorner;

		private string[] _badStatus = new string[6] { "I don't see anything", "Hello!", "Is there anybody out there?", "Just nod if you can hear me", "Is there any one home?", "Giving up in 5 seconds" };

		private string[] _goodStatus = new string[5] { "I see something", "Looking good", "Keep holding", "Just a moment longer", "5 more seconds" };

		private const int DELAY_IN_SECONDS = 5;

		private const int MAX_CAPTURED_VECTORS = 250;

		private const int MAX_PROCESSING_SECONDS = 30;

		private const string MESSAGE_CANCELLED = "Cancelled by user";

		private const string MESSAGE_COMPLETE_GOOD = "Detection complete";

		private const string MESSAGE_COMPLETE_INTERRUPTED = "Object detection was interrupted. The process recovered, but it could be indicative of a problem.";

		private const string MESSAGE_OUT_OF_RANGE = "The object detected is outside of the expected range. Please make sure the object detected is at the expected touch point.";

		private const string MESSAGE_TIMED_OUT = "I didn't see anything, so I quit.";

		public SetBoundaryCornerCommand()
		{
			_concurrentVectors = new ConcurrentBag<Vector3Int>();
			_receivedVectors = new List<Vector3Int>(250);
		}

		private void DepthEventHandler_OnObjectDetected(Vector3Int vector)
		{
			_concurrentVectors.Add(vector);
		}

		private void DepthEventHandler_OnObjectRemoved()
		{
			_captureInterrupted = true;
		}

		private void CaptureFrames()
		{
			ConcurrentBag<Vector3Int> concurrentVectors = _concurrentVectors;
			lock (concurrentVectors)
			{
				_receivedVectors.AddRange(_concurrentVectors);
				Vector3Int result;
				while (_concurrentVectors.TryTake(out result))
				{
				}
			}
		}

		private void DisplayNegativeStatus()
		{
			if (++_negativeStatusIndex >= _badStatus.Length)
			{
				_negativeStatusIndex = _badStatus.Length - 1;
			}
			SetStatusText(_badStatus[_negativeStatusIndex]);
		}

		private void DisplayPositiveStatus()
		{
			if (++_positiveStatusIndex >= _goodStatus.Length)
			{
				_positiveStatusIndex = _goodStatus.Length - 1;
			}
			SetStatusText(_goodStatus[_positiveStatusIndex]);
		}

		public void Execute(BoundaryCornerEnum corner)
		{
			if (_running)
			{
				SetStatusText("Cancelled by user");
				HideAllDots();
				StopAllCoroutines();
				_running = false;
			}
			else
			{
				HideAllDots();
				InitializeVariables(corner);
				StartCoroutine(MainLoop());
			}
		}

		public void Execute(BoundaryCornerEnum corner, Vector2Int location)
		{
			switch (corner)
			{
			case BoundaryCornerEnum.BottomLeft:
				gameBoardCoordinates.BottomLeft = location;
				break;
			case BoundaryCornerEnum.BottomRight:
				gameBoardCoordinates.BottomRight = location;
				break;
			case BoundaryCornerEnum.TopLeft:
				gameBoardCoordinates.TopLeft = location;
				roiCoordinates.TopLeft = new Vector2Int(location.x - 5, roiCoordinates.TopLeft.y);
				break;
			case BoundaryCornerEnum.TopRight:
				gameBoardCoordinates.TopRight = location;
				roiCoordinates.BottomRight = new Vector2Int(location.x + 5, roiCoordinates.BottomRight.y);
				break;
			}
		}

		private int FindMostFrequent(IEnumerable<int> numbers)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num = 0;
			int result = 0;
			foreach (int number in numbers)
			{
				if (dictionary.ContainsKey(number))
				{
					dictionary[number]++;
				}
				else
				{
					dictionary[number] = 1;
				}
				if (dictionary[number] > num)
				{
					num = dictionary[number];
					result = number;
				}
			}
			return result;
		}

		private void HandleNoFrames()
		{
			_noFrameCount++;
			if (_noFrameCount > 1)
			{
				if (_noFrameCount > 6)
				{
					SetStatusText("I didn't see anything, so I quit.");
					_failureTimeout = true;
				}
				else
				{
					DisplayNegativeStatus();
				}
			}
		}

		private void HideAllDots()
		{
			dotBottomLeft.SetActive(value: false);
			dotBottomRight.SetActive(value: false);
			dotTopLeft.SetActive(value: false);
			dotTopRight.SetActive(value: false);
		}

		private void InitializeVariables(BoundaryCornerEnum corner)
		{
			SetStatusText(string.Empty);
			_captureInterrupted = false;
			_failureTimeout = false;
			_negativeStatusIndex = -1;
			_noFrameCount = 0;
			_positiveStatusIndex = -1;
			_processedSeconds = 0;
			_running = true;
			_workingCorner = corner;
			Vector3Int result;
			while (_concurrentVectors.TryTake(out result))
			{
			}
			_receivedVectors.Clear();
			ShowTouchDot(corner);
		}

		private IEnumerator MainLoop()
		{
			depthEventHandler.OnObjectDetected += DepthEventHandler_OnObjectDetected;
			depthEventHandler.OnObjectRemoved += DepthEventHandler_OnObjectRemoved;
			while (_processedSeconds < 30 && !_failureTimeout)
			{
				yield return new WaitForSeconds(5f);
				if (_concurrentVectors.IsEmpty)
				{
					HandleNoFrames();
					continue;
				}
				_processedSeconds += 5;
				CaptureFrames();
				DisplayPositiveStatus();
				if (_positiveStatusIndex == 1)
				{
					_captureInterrupted = false;
				}
			}
			depthEventHandler.OnObjectDetected -= DepthEventHandler_OnObjectDetected;
			depthEventHandler.OnObjectRemoved -= DepthEventHandler_OnObjectRemoved;
			if (!_failureTimeout)
			{
				SetStatusText(_captureInterrupted ? "Object detection was interrupted. The process recovered, but it could be indicative of a problem." : "Detection complete");
				ProcessReceivedVectors();
			}
			HideAllDots();
			_running = false;
		}

		private void ProcessReceivedVectors()
		{
			int x = FindMostFrequent(_receivedVectors.Select(delegate(Vector3Int v)
			{
				Vector3Int vector3Int = v;
				return vector3Int.x;
			}));
			int y = FindMostFrequent(_receivedVectors.Select(delegate(Vector3Int v)
			{
				Vector3Int vector3Int = v;
				return vector3Int.z;
			}));
			Vector2Int vector2Int = new Vector2Int(x, y);
			if (!(vector2Int != Vector2Int.zero))
			{
				return;
			}
			switch (_workingCorner)
			{
			case BoundaryCornerEnum.BottomLeft:
				if (vector2Int.x < gameBoardCoordinates.TopRight.x && vector2Int.x < gameBoardCoordinates.BottomRight.x && vector2Int.y > gameBoardCoordinates.TopLeft.y && vector2Int.y > gameBoardCoordinates.TopRight.y)
				{
					Execute(_workingCorner, vector2Int);
				}
				else
				{
					SetStatusText("The object detected is outside of the expected range. Please make sure the object detected is at the expected touch point.");
				}
				break;
			case BoundaryCornerEnum.BottomRight:
				if (vector2Int.x > gameBoardCoordinates.TopLeft.x && vector2Int.x > gameBoardCoordinates.BottomLeft.x && vector2Int.y > gameBoardCoordinates.TopLeft.y && vector2Int.y > gameBoardCoordinates.TopRight.y)
				{
					Execute(_workingCorner, vector2Int);
				}
				else
				{
					SetStatusText("The object detected is outside of the expected range. Please make sure the object detected is at the expected touch point.");
				}
				break;
			case BoundaryCornerEnum.TopLeft:
				if (vector2Int.x < gameBoardCoordinates.TopRight.x && vector2Int.x < gameBoardCoordinates.BottomRight.x && vector2Int.y < gameBoardCoordinates.BottomLeft.y && vector2Int.y < gameBoardCoordinates.BottomRight.y)
				{
					Execute(_workingCorner, vector2Int);
				}
				else
				{
					SetStatusText("The object detected is outside of the expected range. Please make sure the object detected is at the expected touch point.");
				}
				break;
			case BoundaryCornerEnum.TopRight:
				if (vector2Int.x > gameBoardCoordinates.TopLeft.x && vector2Int.x > gameBoardCoordinates.BottomLeft.x && vector2Int.y < gameBoardCoordinates.BottomLeft.y && vector2Int.y < gameBoardCoordinates.BottomRight.y)
				{
					Execute(_workingCorner, vector2Int);
				}
				else
				{
					SetStatusText("The object detected is outside of the expected range. Please make sure the object detected is at the expected touch point.");
				}
				break;
			}
		}

		private void SetStatusText(string text)
		{
			statusText.text = text;
		}

		private void ShowTouchDot(BoundaryCornerEnum corner)
		{
			switch (corner)
			{
			case BoundaryCornerEnum.BottomLeft:
				dotBottomLeft.SetActive(value: true);
				break;
			case BoundaryCornerEnum.BottomRight:
				dotBottomRight.SetActive(value: true);
				break;
			case BoundaryCornerEnum.TopLeft:
				dotTopLeft.SetActive(value: true);
				break;
			case BoundaryCornerEnum.TopRight:
				dotTopRight.SetActive(value: true);
				break;
			}
		}

		protected void OnDisabled()
		{
			depthEventHandler.OnObjectDetected -= DepthEventHandler_OnObjectDetected;
			depthEventHandler.OnObjectRemoved -= DepthEventHandler_OnObjectRemoved;
			SetStatusText(string.Empty);
			dotBottomLeft.SetActive(value: false);
			dotBottomRight.SetActive(value: false);
			dotTopLeft.SetActive(value: false);
			dotTopRight.SetActive(value: false);
		}
	}
}
