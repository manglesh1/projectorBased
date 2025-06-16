using System.Linq;
using ConfirmationModal;
using Detection.ScriptableObjects;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Detection.Controllers
{
	public class ResetController : MonoBehaviour
	{
		[Header("Scriptable Objects")]
		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		[SerializeField]
		private RealSenseCameraSettingsSO realSenseCameraSettings;

		[SerializeField]
		private RoiCoordinatesSO roi;

		[Header("UI")]
		[SerializeField]
		private ConfirmationModalEventsSO confirmationModal;

		[SerializeField]
		private Button resetButton;

		[SerializeField]
		private Toggle toggleAll;

		[Header("Toggles - Calibration")]
		[SerializeField]
		private Toggle toggleCalibration;

		[SerializeField]
		private Toggle toggleCalibrationExposure;

		[SerializeField]
		private Toggle toggleCalibrationMinDistance;

		[SerializeField]
		private Toggle toggleCalibrationMaxDistance;

		[Header("Toggles - ROI")]
		[SerializeField]
		private Toggle toggleROI;

		[Header("Toggles - Laser")]
		[SerializeField]
		private Toggle toggleLaser;

		[SerializeField]
		private Toggle toggleLaserEnabled;

		[SerializeField]
		private Toggle toggleLaserPower;

		[Header("Toggles - Boundary")]
		[SerializeField]
		private Toggle toggleBoundary;

		[Header("Toggles - Adjustments")]
		[SerializeField]
		private Toggle toggleAdjustmentsOffset;

		private const string CONFIRMATION_MESSAGE = "Are you sure?";

		private void resetButton_OnClick()
		{
			confirmationModal.RaiseConfirmationModal(ResetValues, "Are you sure?");
		}

		private void toggleAll_OnValueChanged(bool active)
		{
			foreach (Toggle item in from t in GetComponentsInChildren<Toggle>()
				where t.name != toggleAll.name
				select t)
			{
				item.SetIsOnWithoutNotify(active);
			}
		}

		private void toggleCalibration_OnValueChanged(bool active)
		{
			toggleCalibrationExposure.SetIsOnWithoutNotify(active);
			toggleCalibrationMaxDistance.SetIsOnWithoutNotify(active);
			toggleCalibrationMinDistance.SetIsOnWithoutNotify(active);
			UpdateToggleAll();
		}

		private void toggleCalibrationExposure_OnValueChanged(bool active)
		{
			toggleCalibration.SetIsOnWithoutNotify(toggleCalibrationExposure.isOn && toggleCalibrationMaxDistance.isOn && toggleCalibrationMinDistance.isOn);
			UpdateToggleAll();
		}

		private void toggleCalibrationMinDistance_OnValueChanged(bool active)
		{
			toggleCalibration.SetIsOnWithoutNotify(toggleCalibrationExposure.isOn && toggleCalibrationMaxDistance.isOn && toggleCalibrationMinDistance.isOn);
			UpdateToggleAll();
		}

		private void toggleCalibrationMaxDistance_OnValueChanged(bool active)
		{
			toggleCalibration.SetIsOnWithoutNotify(toggleCalibrationExposure.isOn && toggleCalibrationMaxDistance.isOn && toggleCalibrationMinDistance.isOn);
			UpdateToggleAll();
		}

		private void toggleROI_OnValueChanged(bool active)
		{
			UpdateToggleAll();
		}

		private void toggleLaser_OnValueChanged(bool active)
		{
			toggleLaserEnabled.SetIsOnWithoutNotify(active);
			toggleLaserPower.SetIsOnWithoutNotify(active);
			UpdateToggleAll();
		}

		private void toggleLaserEnabled_OnValueChanged(bool active)
		{
			toggleLaser.SetIsOnWithoutNotify(toggleLaserEnabled.isOn && toggleLaserPower.isOn);
			UpdateToggleAll();
		}

		private void toggleLaserPower_OnValueChanged(bool active)
		{
			toggleLaser.SetIsOnWithoutNotify(toggleLaserEnabled.isOn && toggleLaserPower.isOn);
			UpdateToggleAll();
		}

		private void toggleBoundary_OnValueChanged(bool active)
		{
			UpdateToggleAll();
		}

		private void toggleAdjustmentsOffset_OnValueChanged(bool active)
		{
			UpdateToggleAll();
		}

		private void ResetValues()
		{
			SetAdjustments();
			SetBoundaryValues();
			SetCalibrationValues();
			SetLaserValues();
			SetROI();
			SetSettingStoreValues();
		}

		private void SetAdjustments()
		{
			if (toggleAdjustmentsOffset.isOn)
			{
				gameBoardCoordinates.DownwardOffset = 0m;
			}
		}

		private void SetBoundaryValues()
		{
			if (toggleBoundary.isOn)
			{
				gameBoardCoordinates.BottomLeft = DetectionConstants.DEFAULT_GAME_BOARD_BOTTOM_LEFT;
				gameBoardCoordinates.BottomRight = DetectionConstants.DEFAULT_GAME_BOARD_BOTTOM_RIGHT;
				gameBoardCoordinates.TopLeft = DetectionConstants.DEFAULT_GAME_BOARD_TOP_LEFT;
				gameBoardCoordinates.TopRight = DetectionConstants.DEFAULT_GAME_BOARD_TOP_RIGHT;
			}
		}

		private void SetCalibrationValues()
		{
			if (toggleCalibrationExposure.isOn)
			{
				realSenseCameraSettings.Exposure = 50000f;
			}
			if (toggleCalibrationMaxDistance.isOn)
			{
				realSenseCameraSettings.MaxDistance = 2.4f;
			}
			if (toggleCalibrationMinDistance.isOn)
			{
				realSenseCameraSettings.MinDistance = 1f;
			}
		}

		private void SetLaserValues()
		{
			if (toggleLaserEnabled.isOn)
			{
				realSenseCameraSettings.EmitterEnabled = false;
			}
			if (toggleLaserPower.isOn)
			{
				realSenseCameraSettings.LaserPower = 50f;
			}
		}

		private void SetROI()
		{
			if (toggleROI.isOn)
			{
				roi.BottomRight = DetectionConstants.DEFAULT_ROI_BOTTOM_RIGHT;
				roi.TopLeft = DetectionConstants.DEFAULT_ROI_TOP_LEFT;
			}
		}

		private void SetSettingStoreValues()
		{
			SettingsStore.DetectionSettings.EmitterEnabled = realSenseCameraSettings.EmitterEnabled;
			SettingsStore.DetectionSettings.Exposure = realSenseCameraSettings.Exposure;
			SettingsStore.DetectionSettings.Gain = 16f;
			SettingsStore.DetectionSettings.LaserPower = realSenseCameraSettings.LaserPower;
			SettingsStore.DetectionSettings.MaxDistance = realSenseCameraSettings.MaxDistance;
			SettingsStore.DetectionSettings.MinDistance = realSenseCameraSettings.MinDistance;
			SettingsStore.DetectionSettings.DownwardOffset = gameBoardCoordinates.DownwardOffset;
			SettingsStore.DetectionSettings.GameboardBottomLeft = gameBoardCoordinates.BottomLeft;
			SettingsStore.DetectionSettings.GameboardTopRight = gameBoardCoordinates.TopRight;
			SettingsStore.DetectionSettings.GameboardTopLeft = gameBoardCoordinates.TopLeft;
			SettingsStore.DetectionSettings.GameboardBottomRight = gameBoardCoordinates.BottomRight;
			SettingsStore.DetectionSettings.BottomRightROI = roi.BottomRight;
			SettingsStore.DetectionSettings.TopLeftROI = roi.TopLeft;
			SettingsStore.DetectionSettings.Save();
		}

		private void UpdateToggleAll()
		{
			toggleAll.SetIsOnWithoutNotify((from t in GetComponentsInChildren<Toggle>()
				where t.name != toggleAll.name
				select t).All((Toggle t) => t.isOn));
		}

		private void OnDisable()
		{
			resetButton.onClick.RemoveAllListeners();
			toggleAll.onValueChanged.RemoveAllListeners();
			toggleCalibration.onValueChanged.RemoveAllListeners();
			toggleCalibrationExposure.onValueChanged.RemoveAllListeners();
			toggleCalibrationMaxDistance.onValueChanged.RemoveAllListeners();
			toggleCalibrationMinDistance.onValueChanged.RemoveAllListeners();
			toggleROI.onValueChanged.RemoveAllListeners();
			toggleLaser.onValueChanged.RemoveAllListeners();
			toggleLaserEnabled.onValueChanged.RemoveAllListeners();
			toggleLaserPower.onValueChanged.RemoveAllListeners();
			toggleBoundary.onValueChanged.RemoveAllListeners();
			toggleAdjustmentsOffset.onValueChanged.RemoveAllListeners();
		}

		private void OnEnable()
		{
			resetButton.onClick.AddListener(resetButton_OnClick);
			toggleAll.onValueChanged.AddListener(toggleAll_OnValueChanged);
			toggleCalibration.onValueChanged.AddListener(toggleCalibration_OnValueChanged);
			toggleCalibrationExposure.onValueChanged.AddListener(toggleCalibrationExposure_OnValueChanged);
			toggleCalibrationMaxDistance.onValueChanged.AddListener(toggleCalibrationMaxDistance_OnValueChanged);
			toggleCalibrationMinDistance.onValueChanged.AddListener(toggleCalibrationMinDistance_OnValueChanged);
			toggleROI.onValueChanged.AddListener(toggleROI_OnValueChanged);
			toggleLaser.onValueChanged.AddListener(toggleLaser_OnValueChanged);
			toggleLaserEnabled.onValueChanged.AddListener(toggleLaserEnabled_OnValueChanged);
			toggleLaserPower.onValueChanged.AddListener(toggleLaserPower_OnValueChanged);
			toggleBoundary.onValueChanged.AddListener(toggleBoundary_OnValueChanged);
			toggleAdjustmentsOffset.onValueChanged.AddListener(toggleAdjustmentsOffset_OnValueChanged);
		}
	}
}
