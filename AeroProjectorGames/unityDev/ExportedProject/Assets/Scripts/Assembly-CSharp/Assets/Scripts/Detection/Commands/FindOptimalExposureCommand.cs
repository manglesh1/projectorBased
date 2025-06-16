using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using ConfirmationModal;
using Detection.Commands;
using Detection.ScriptableObjects;
using Settings;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Detection.Commands
{
	[RequireComponent(typeof(Button))]
	public class FindOptimalExposureCommand : MonoBehaviour
	{
		[Header("Camera Stuff")]
		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private VectorEventHandler depthEventHandler;

		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		[Header("UI Elements")]
		[SerializeField]
		private ConfirmationModalEventsSO confirmationModal;

		[SerializeField]
		private TMP_InputField exposureField;

		[SerializeField]
		private Slider exposureSlider;

		[SerializeField]
		private TMP_InputField laserField;

		[SerializeField]
		private Slider laserSlider;

		[SerializeField]
		private Toggle laserToggle;

		[SerializeField]
		private TMP_Text statusText;

		[Header("Touch Points")]
		[SerializeField]
		private GameObject dotBottomLeft;

		private bool _captureInterrupted;

		private readonly ConcurrentBag<Vector3Int> _concurrentVectors;

		private float _originalExposure;

		private bool _originalLaserEnabled;

		private float _originalLaserPower;

		private int _maxExposure;

		private int _minExposure;

		private Button _myButton;

		private bool _running;

		private const int DELAY_BEFORE_START = 10;

		private const int DELAY_INTERVAL = 1;

		private const float DISTANCE_THRESHOLD = 3f;

		private const int MAX_ALLOWED_EXPOSURE = 50000;

		private const int MAX_LASER_POWER = 160;

		private const int MIN_ALLOWED_EXPOSURE = 1;

		private const int MIN_LASER_POWER = 0;

		private const string NUMBER_FORMAT = "F0";

		public FindOptimalExposureCommand()
		{
			_concurrentVectors = new ConcurrentBag<Vector3Int>();
		}

		private void DepthEventHandler_OnObjectDetected(Vector3Int vector)
		{
			ConcurrentBag<Vector3Int> concurrentVectors = _concurrentVectors;
			lock (concurrentVectors)
			{
				_concurrentVectors.Add(vector);
			}
		}

		private void DepthEventHandler_OnObjectRemoved()
		{
			_captureInterrupted = true;
		}

		public void Execute()
		{
			StartCoroutine(MainLoop());
		}

		private void HandleConfirmation()
		{
			if (_running)
			{
				StopAllCoroutines();
				_running = false;
				statusText.text = "Cancelled by user";
				UpdateExposureValue(_originalExposure);
				UpdateLaserValues(_originalLaserEnabled, _originalLaserPower);
				dotBottomLeft.SetActive(value: false);
			}
			else if (confirmationModal == null)
			{
				Execute();
			}
			else
			{
				confirmationModal.RaiseConfirmationModal(Execute, "Continuing this process will overwrite your existing exposure setting. Are you sure that you want to proceed?");
			}
		}

		private bool HasObject()
		{
			bool result = false;
			ConcurrentBag<Vector3Int> concurrentVectors = _concurrentVectors;
			lock (concurrentVectors)
			{
				bool flag = !_captureInterrupted && _concurrentVectors.Any((Vector3Int v) => v.z > 0);
				result = flag;
			}
			return result;
		}

		private void InitializeVariables()
		{
			_captureInterrupted = false;
			_maxExposure = 50000;
			_minExposure = 1;
			_originalExposure = cameraSettings.Exposure;
			_originalLaserEnabled = SettingsStore.DetectionSettings.EmitterEnabled;
			_originalLaserPower = SettingsStore.DetectionSettings.LaserPower;
			_running = true;
			cameraSettings.LaserPower = 0f;
			cameraSettings.EmitterEnabled = false;
		}

		private bool IsObjectStable()
		{
			bool result = false;
			ConcurrentBag<Vector3Int> concurrentVectors = _concurrentVectors;
			lock (concurrentVectors)
			{
				if (!_captureInterrupted && _concurrentVectors.Count > 1)
				{
					Vector3Int first = _concurrentVectors.First();
					result = _concurrentVectors.All(delegate(Vector3Int v)
					{
						int num = first.x - v.x;
						int num2 = first.z - v.z;
						return math.sqrt(num * num + num2 * num2) <= 3f;
					});
				}
			}
			return result;
		}

		private IEnumerator MainLoop()
		{
			int currentExposure = 50000;
			int foundExposure = 50000;
			bool success = true;
			dotBottomLeft.SetActive(value: true);
			yield return StartCoroutine(RunStartupDelayTimer());
			InitializeVariables();
			while (currentExposure > _minExposure)
			{
				_captureInterrupted = false;
				cameraSettings.Exposure = currentExposure;
				statusText.text = $"Current exposure {currentExposure}";
				StartCapturing();
				yield return new WaitForSeconds(1f);
				StopCapturing();
				if (HasObject())
				{
					if (IsObjectStable())
					{
						foundExposure = currentExposure;
						UpdateCurrentExposure(moveForward: false, ref currentExposure);
					}
					else
					{
						UpdateCurrentExposure(moveForward: true, ref currentExposure);
					}
				}
				else if (currentExposure == 50000)
				{
					if (cameraSettings.EmitterEnabled)
					{
						statusText.text = "Failed to find optimal exposure. An external IR flood light is probably necessary.";
						success = false;
						break;
					}
					UpdateLaserValues(laserEnabled: true, 160f);
					statusText.text = "Fire the laser!";
					yield return new WaitForSeconds(1f);
				}
				else
				{
					UpdateCurrentExposure(moveForward: true, ref currentExposure);
				}
				ConcurrentBag<Vector3Int> concurrentVectors = _concurrentVectors;
				lock (concurrentVectors)
				{
					Vector3Int result;
					while (_concurrentVectors.TryTake(out result))
					{
					}
				}
			}
			_running = false;
			dotBottomLeft.SetActive(value: false);
			if (success)
			{
				statusText.text = $"Final exposure applied is {foundExposure}";
			}
			UpdateExposureValue(foundExposure);
		}

		private IEnumerator RunStartupDelayTimer()
		{
			for (int waitCounter = 10; waitCounter > 0; waitCounter--)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Processing will start in ");
				stringBuilder.Append(waitCounter);
				stringBuilder.Append((waitCounter == 1) ? " second" : " seconds");
				statusText.text = stringBuilder.ToString();
				yield return new WaitForSeconds(1f);
			}
		}

		private void StartCapturing()
		{
			depthEventHandler.OnObjectDetected += DepthEventHandler_OnObjectDetected;
			depthEventHandler.OnObjectRemoved += DepthEventHandler_OnObjectRemoved;
		}

		private void StopCapturing()
		{
			depthEventHandler.OnObjectDetected -= DepthEventHandler_OnObjectDetected;
			depthEventHandler.OnObjectRemoved -= DepthEventHandler_OnObjectRemoved;
		}

		private void UpdateCurrentExposure(bool moveForward, ref int currentExposure)
		{
			if (moveForward)
			{
				_minExposure = currentExposure;
			}
			else
			{
				_maxExposure = currentExposure;
			}
			currentExposure = _minExposure + (_maxExposure - _minExposure) / 2;
		}

		private void UpdateExposureValue(float exposureValue)
		{
			cameraSettings.Exposure = exposureValue;
			SettingsStore.DetectionSettings.Exposure = exposureValue;
			if (exposureField != null)
			{
				exposureField.text = exposureValue.ToString("F0");
			}
			if (exposureSlider != null)
			{
				exposureSlider.value = exposureValue;
			}
		}

		private void UpdateLaserValues(bool laserEnabled, float laserPower)
		{
			cameraSettings.EmitterEnabled = laserEnabled;
			cameraSettings.LaserPower = laserPower;
			SettingsStore.DetectionSettings.EmitterEnabled = laserEnabled;
			SettingsStore.DetectionSettings.LaserPower = laserPower;
			if (laserField != null)
			{
				laserField.text = laserPower.ToString("F0");
			}
			if (laserToggle != null)
			{
				laserToggle.isOn = laserEnabled;
			}
			if (laserSlider != null)
			{
				laserSlider.value = laserPower;
			}
		}

		private void OnDisable()
		{
			_running = true;
			StopCapturing();
			_myButton.onClick.RemoveAllListeners();
		}

		private void OnEnable()
		{
			_running = false;
			_myButton = GetComponent<Button>();
			_myButton.onClick.AddListener(HandleConfirmation);
		}
	}
}
