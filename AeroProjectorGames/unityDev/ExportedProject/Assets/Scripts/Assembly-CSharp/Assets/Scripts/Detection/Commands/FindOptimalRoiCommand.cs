using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using ConfirmationModal;
using Detection.Commands;
using Detection.ScriptableObjects;
using Intel.RealSense;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Detection.Commands
{
	[RequireComponent(typeof(Button))]
	public class FindOptimalRoiCommand : MonoBehaviour
	{
		[Header("Camera Stuff")]
		[SerializeField]
		private ConfirmationModalEventsSO confirmationModal;

		[SerializeField]
		private VectorEventHandler depthEventHandler;

		[SerializeField]
		private GetDistanceVectorsCommand getDistanceVectors;

		[SerializeField]
		private RoiCoordinatesSO roi;

		[Header("UI")]
		[Space]
		[SerializeField]
		private TMP_Text statusText;

		private readonly ConcurrentBag<Vector3Int[]> _capturedFrames;

		private bool _faulted;

		private Button _myButton;

		private bool _objectDetected;

		private Vector2Int _originalBottomRight;

		private Vector2Int _originalTopLeft;

		private bool _running;

		private const int DELAY_INTERVAL = 1;

		private const int INCREMENT_LARGE = 10;

		private const int INCREMENT_SMALL = 5;

		private const int MAX_DELAY = 10;

		private const int MAX_Y = 700;

		private const int PIXELS_BETWEEN = 5;

		public FindOptimalRoiCommand()
		{
			_capturedFrames = new ConcurrentBag<Vector3Int[]>();
		}

		private void DepthEventHandler_OnFrameReceived(DepthFrame frame)
		{
			ConcurrentBag<Vector3Int[]> capturedFrames = _capturedFrames;
			lock (capturedFrames)
			{
				_capturedFrames.Add(getDistanceVectors.Execute(frame));
			}
		}

		private void DepthEventHandler_OnObjectDetected(Vector3Int _)
		{
			_objectDetected = true;
		}

		private void AdjustRoiWidth()
		{
			roi.BottomRight = new Vector2Int(SettingsStore.DetectionSettings.GameboardTopRight.x + 5, roi.BottomRight.y);
			roi.TopLeft = new Vector2Int(SettingsStore.DetectionSettings.GameboardTopLeft.x - 5, roi.TopLeft.y);
		}

		private void CleanUpForReset()
		{
			_running = false;
			roi.BottomRight = _originalBottomRight;
			roi.TopLeft = _originalTopLeft;
			StopAllCoroutines();
		}

		private void Execute()
		{
			InitializeVariables();
			AdjustRoiWidth();
			MoveRoiToMaxY();
			StartCoroutine(ProcessingLogic());
		}

		private IEnumerator FindEmptySpace()
		{
			StartListeningToCameraEvents();
			int delayCounter = 10;
			while (delayCounter > 0)
			{
				statusText.text = $"Moving out until nothing is found. Finalizing in {delayCounter} seconds.";
				yield return new WaitForSeconds(1f);
				delayCounter--;
				if (_objectDetected)
				{
					_objectDetected = false;
					delayCounter = 10;
					MoveRoiDown();
				}
				if (roi.BottomRight.y > 700)
				{
					CleanUpForReset();
					statusText.text = "Could not find an unobstructed area. Resetting to the original location.";
					_faulted = true;
					break;
				}
			}
			StopListeningToCameraEvents();
		}

		private IEnumerator FindWall()
		{
			StartCapturingFrames();
			statusText.text = "Searching for the wall";
			while (true)
			{
				ConcurrentBag<Vector3Int[]> capturedFrames = _capturedFrames;
				lock (capturedFrames)
				{
					if (!_capturedFrames.IsEmpty)
					{
						Vector3Int[] result;
						if (_capturedFrames.First().Any((Vector3Int v) => v.z > 0))
						{
							while (_capturedFrames.TryTake(out result))
							{
							}
							break;
						}
						while (_capturedFrames.TryTake(out result))
						{
						}
					}
				}
				MoveRoiUp();
				if (roi.TopLeft.y >= 0)
				{
					yield return new WaitForEndOfFrame();
					continue;
				}
				CleanUpForReset();
				statusText.text = "Could not find the wall. Resetting to the original location.";
				_faulted = true;
				break;
			}
			StopCapturingFrames();
		}

		private void HandleConfirmation()
		{
			if (_running)
			{
				CleanUpForReset();
			}
			else if (confirmationModal == null)
			{
				Execute();
			}
			else
			{
				confirmationModal.RaiseConfirmationModal(Execute, "Are you sure?");
			}
		}

		private void InitializeVariables()
		{
			_faulted = false;
			_running = true;
			_originalBottomRight = roi.BottomRight;
			_originalTopLeft = roi.TopLeft;
		}

		private IEnumerator ProcessingLogic()
		{
			yield return StartCoroutine(FindWall());
			if (!_faulted)
			{
				yield return StartCoroutine(FindEmptySpace());
			}
			if (!_faulted)
			{
				statusText.text = "Done!";
			}
			_running = false;
		}

		private void MoveRoiDown()
		{
			roi.BottomRight = new Vector2Int(roi.BottomRight.x, roi.BottomRight.y + 5);
			roi.TopLeft = new Vector2Int(roi.TopLeft.x, roi.TopLeft.y + 5);
		}

		private void MoveRoiToMaxY()
		{
			roi.BottomRight = new Vector2Int(roi.BottomRight.x, 700);
			roi.TopLeft = new Vector2Int(roi.TopLeft.x, 695);
		}

		private void MoveRoiUp()
		{
			roi.BottomRight = new Vector2Int(roi.BottomRight.x, roi.BottomRight.y - 10);
			roi.TopLeft = new Vector2Int(roi.TopLeft.x, roi.TopLeft.y - 10);
		}

		private void StartCapturingFrames()
		{
			depthEventHandler.OnFrameReceived += DepthEventHandler_OnFrameReceived;
		}

		private void StopCapturingFrames()
		{
			depthEventHandler.OnFrameReceived -= DepthEventHandler_OnFrameReceived;
		}

		private void StartListeningToCameraEvents()
		{
			depthEventHandler.OnObjectDetected += DepthEventHandler_OnObjectDetected;
		}

		private void StopListeningToCameraEvents()
		{
			depthEventHandler.OnObjectDetected -= DepthEventHandler_OnObjectDetected;
		}

		private void OnDisable()
		{
			_myButton.onClick.RemoveAllListeners();
		}

		private void OnEnable()
		{
			_myButton = GetComponent<Button>();
			_myButton.onClick.AddListener(HandleConfirmation);
		}
	}
}
