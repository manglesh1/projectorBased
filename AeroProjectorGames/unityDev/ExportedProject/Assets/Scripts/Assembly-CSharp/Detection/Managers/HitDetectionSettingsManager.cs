using System.Collections;
using System.Collections.Generic;
using Detection.Factories;
using Detection.ScriptableObjects;
using Settings;
using UnityEngine;
using UnityEngine.Events;

namespace Detection.Managers
{
	public class HitDetectionSettingsManager : MonoBehaviour
	{
		private const float DEBOUNCE_TIME = 5f;

		private const int DEFAULT_GAIN = 16;

		private const int DEFAULT_LASER_POWER = 1;

		private float _lastInvokedTime;

		private bool _saveInProcess;

		private readonly Vector2Int GAMEBOARD_OLD_BOTTOM_LEFT = new Vector2Int(588, 737);

		private readonly Vector2Int GAMEBOARD_OLD_BOTTOM_RIGHT = new Vector2Int(716, 738);

		private readonly Vector2Int GAMEBOARD_OLD_TOP_LEFT = new Vector2Int(572, 598);

		private readonly Vector2Int GAMEBOARD_OLD_TOP_RIGHT = new Vector2Int(730, 598);

		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private CoordinatesSO gameboardCoordinates;

		[SerializeField]
		private RoiCoordinatesSO roiCoordinates;

		[SerializeField]
		private SettingsEventsSO settingsEvents;

		private List<object> _eventHandlers;

		private List<UnityAction> _events;

		private void Awake()
		{
			Load();
		}

		private void OnDisable()
		{
			cameraSettings.OnEmitterEnabledChanged -= Save;
			cameraSettings.OnExposureChanged -= Save;
			cameraSettings.OnGainChanged -= Save;
			cameraSettings.OnLaserPowerChanged -= Save;
			cameraSettings.OnMaxDistanceChanged -= Save;
			cameraSettings.OnMinDistanceChanged -= Save;
			gameboardCoordinates.OnChanged -= Save;
			roiCoordinates.OnChanged -= Save;
			settingsEvents.OnSettingsReloaded -= Load;
		}

		private void OnEnable()
		{
			cameraSettings.OnEmitterEnabledChanged += Save;
			cameraSettings.OnExposureChanged += Save;
			cameraSettings.OnGainChanged += Save;
			cameraSettings.OnLaserPowerChanged += Save;
			cameraSettings.OnMaxDistanceChanged += Save;
			cameraSettings.OnMinDistanceChanged += Save;
			gameboardCoordinates.OnChanged += Save;
			roiCoordinates.OnChanged += Save;
			settingsEvents.OnSettingsReloaded += Load;
		}

		private void Save()
		{
			_lastInvokedTime = Time.time;
			if (!_saveInProcess)
			{
				_saveInProcess = true;
				StartCoroutine(DebouncedSaveRoutine());
			}
		}

		private IEnumerator DebouncedSaveRoutine()
		{
			while (Time.time < _lastInvokedTime + 5f)
			{
				yield return null;
			}
			DetectionSettings detectionSettings = SettingsStore.DetectionSettings;
			detectionSettings.Gain = cameraSettings.Gain;
			detectionSettings.Exposure = cameraSettings.Exposure;
			detectionSettings.EmitterEnabled = cameraSettings.EmitterEnabled;
			detectionSettings.LaserPower = cameraSettings.LaserPower;
			detectionSettings.MaxDistance = cameraSettings.MaxDistance;
			detectionSettings.MinDistance = cameraSettings.MinDistance;
			detectionSettings.DownwardOffset = gameboardCoordinates.DownwardOffset;
			detectionSettings.GameboardBottomRight = gameboardCoordinates.BottomRight;
			detectionSettings.GameboardBottomLeft = gameboardCoordinates.BottomLeft;
			detectionSettings.GameboardTopLeft = gameboardCoordinates.TopLeft;
			detectionSettings.GameboardTopRight = gameboardCoordinates.TopRight;
			detectionSettings.BottomRightROI = roiCoordinates.BottomRight;
			detectionSettings.TopLeftROI = roiCoordinates.TopLeft;
			detectionSettings.Save();
			_saveInProcess = false;
		}

		private bool IsOldDefaults(DetectionSettings settings)
		{
			return settings.GameboardBottomLeft == GAMEBOARD_OLD_BOTTOM_LEFT && settings.GameboardBottomRight == GAMEBOARD_OLD_BOTTOM_RIGHT && settings.GameboardTopLeft == GAMEBOARD_OLD_TOP_LEFT && settings.GameboardTopRight == GAMEBOARD_OLD_TOP_RIGHT;
		}

		private void Load()
		{
			DetectionSettings detectionSettings = SettingsStore.DetectionSettings;
			if (IsOldDefaults(detectionSettings))
			{
				detectionSettings.SetDefaults();
				detectionSettings.Save();
			}
			detectionSettings.DetectedCamera = DetectedCameraFactory.DetectCamera();
			cameraSettings.Gain = detectionSettings.Gain;
			cameraSettings.Exposure = detectionSettings.Exposure;
			cameraSettings.EmitterEnabled = detectionSettings.EmitterEnabled;
			cameraSettings.LaserPower = detectionSettings.LaserPower;
			cameraSettings.MaxDistance = detectionSettings.MaxDistance;
			cameraSettings.MinDistance = detectionSettings.MinDistance;
			gameboardCoordinates.DownwardOffset = detectionSettings.DownwardOffset;
			gameboardCoordinates.BottomRight = detectionSettings.GameboardBottomRight;
			gameboardCoordinates.BottomLeft = detectionSettings.GameboardBottomLeft;
			gameboardCoordinates.TopLeft = detectionSettings.GameboardTopLeft;
			gameboardCoordinates.TopRight = detectionSettings.GameboardTopRight;
			roiCoordinates.BottomRight = detectionSettings.BottomRightROI;
			roiCoordinates.TopLeft = detectionSettings.TopLeftROI;
		}
	}
}
