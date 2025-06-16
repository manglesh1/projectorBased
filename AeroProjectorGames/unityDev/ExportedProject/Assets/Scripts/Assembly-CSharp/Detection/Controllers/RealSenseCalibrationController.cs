using System;
using System.Globalization;
using Detection.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class RealSenseCalibrationController : MonoBehaviour
	{
		private const float INCHES_MULTIPLIER = 39.37008f;

		private const float METERS_MULTIPLIER = 0.0254f;

		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private TMP_InputField inputExposureValue;

		[SerializeField]
		private TMP_InputField inputGainValue;

		[SerializeField]
		private TMP_InputField inputLaserPowerValue;

		[SerializeField]
		private TMP_InputField inputMaxDistanceValue;

		[SerializeField]
		private TMP_InputField inputMinDistanceField;

		[SerializeField]
		private Slider sliderExposure;

		[SerializeField]
		private Slider sliderGain;

		[SerializeField]
		private Slider sliderLaserPower;

		[SerializeField]
		private Slider sliderMaxDistance;

		[SerializeField]
		private Slider sliderMinDistance;

		[SerializeField]
		private Toggle toggleUseLaser;

		private void ExposureInputChanged(string value)
		{
			if (!float.TryParse(value, out var result))
			{
				return;
			}
			if (result > 50000f || result < 1f)
			{
				if (inputExposureValue.text.Length > 1)
				{
					UpdateExposureDistance();
				}
			}
			else
			{
				cameraSettings.Exposure = result;
				UpdateExposureDistance();
			}
		}

		private void GainInputChanged(string value)
		{
			if (!float.TryParse(value, out var result))
			{
				Debug.Log("Not a float?");
				Debug.Log(value);
			}
			else if (result > 64f || result < 16f)
			{
				if (inputGainValue.text.Length > 1)
				{
					UpdateGainDistance();
				}
			}
			else
			{
				cameraSettings.Gain = result;
				UpdateGainDistance();
			}
		}

		private void LaserPowerInputChanged(string value)
		{
			if (!float.TryParse(value, out var result))
			{
				return;
			}
			if (result > 160f || result < 1f)
			{
				if (inputLaserPowerValue.text.Length > 1)
				{
					UpdateLaserPowerDistance();
				}
			}
			else
			{
				cameraSettings.LaserPower = result;
				UpdateLaserPowerDistance();
			}
		}

		private void MaxDistanceInputChanged(string value)
		{
			if (!float.TryParse(value, out var result))
			{
				return;
			}
			if (result > 16f || result < 0f)
			{
				if (inputMaxDistanceValue.text.Length > 1)
				{
					UpdateMaxDistance();
				}
			}
			else
			{
				cameraSettings.MaxDistance = InchesToMeters(result);
				UpdateMaxDistance();
			}
		}

		private float MetersToInches(float meters)
		{
			return meters * 39.37008f;
		}

		private float InchesToMeters(float inches)
		{
			return inches * 0.0254f;
		}

		private void MinDistanceInputChanged(string value)
		{
			if (!float.TryParse(value, out var result))
			{
				return;
			}
			if (result > 16f || result < 0f)
			{
				if (inputMinDistanceField.text.Length > 1)
				{
					UpdateMinDistance();
				}
			}
			else
			{
				cameraSettings.MinDistance = InchesToMeters(result);
				UpdateMinDistance();
			}
		}

		private void UpdateExposureDistance()
		{
			inputExposureValue.SetTextWithoutNotify(ConvertFloatToWholeNumberString(cameraSettings.Exposure));
			sliderExposure.SetValueWithoutNotify(cameraSettings.Exposure);
		}

		private void UpdateGainDistance()
		{
			inputGainValue.SetTextWithoutNotify(ConvertFloatToWholeNumberString(cameraSettings.Gain));
			sliderGain.SetValueWithoutNotify(cameraSettings.Gain);
		}

		private void UpdateLaserPowerDistance()
		{
			inputLaserPowerValue.SetTextWithoutNotify(ConvertFloatToWholeNumberString(cameraSettings.LaserPower));
			sliderLaserPower.SetValueWithoutNotify(cameraSettings.LaserPower);
		}

		private void UpdateMaxDistance()
		{
			inputMaxDistanceValue.SetTextWithoutNotify(ConvertFloatToDecimalNumberString(MetersToInches(cameraSettings.MaxDistance)));
			sliderMaxDistance.SetValueWithoutNotify(cameraSettings.MaxDistance);
		}

		private void UpdateMinDistance()
		{
			inputMinDistanceField.text = ConvertFloatToDecimalNumberString(MetersToInches(cameraSettings.MinDistance));
			sliderMinDistance.SetValueWithoutNotify(cameraSettings.MinDistance);
		}

		private void ExposureSliderChanged(float value)
		{
			cameraSettings.Exposure = value;
			UpdateExposureDistance();
		}

		private void GainSliderChanged(float value)
		{
			cameraSettings.Gain = value;
			UpdateGainDistance();
		}

		private void LaserPowerSliderChanged(float value)
		{
			cameraSettings.LaserPower = value;
			UpdateLaserPowerDistance();
		}

		private void MaxDistanceSliderChanged(float value)
		{
			cameraSettings.MaxDistance = value;
			UpdateMaxDistance();
		}

		private void MinDistanceSliderChanged(float value)
		{
			cameraSettings.MinDistance = value;
			UpdateMinDistance();
		}

		private void UseLaserChanged(bool value)
		{
			cameraSettings.EmitterEnabled = value;
		}

		private void OnDisable()
		{
			inputExposureValue.onValueChanged.RemoveAllListeners();
			inputGainValue.onValueChanged.RemoveAllListeners();
			inputLaserPowerValue.onValueChanged.RemoveAllListeners();
			inputMaxDistanceValue.onValueChanged.RemoveAllListeners();
			inputMinDistanceField.onValueChanged.RemoveAllListeners();
			sliderExposure.onValueChanged.RemoveAllListeners();
			sliderGain.onValueChanged.RemoveAllListeners();
			sliderLaserPower.onValueChanged.RemoveAllListeners();
			sliderMaxDistance.onValueChanged.RemoveAllListeners();
			sliderMinDistance.onValueChanged.RemoveAllListeners();
			toggleUseLaser.onValueChanged.RemoveAllListeners();
		}

		private void OnEnable()
		{
			inputExposureValue.text = ConvertFloatToWholeNumberString(cameraSettings.Exposure);
			inputGainValue.text = ConvertFloatToWholeNumberString(cameraSettings.Gain);
			inputLaserPowerValue.text = ConvertFloatToWholeNumberString(cameraSettings.LaserPower);
			inputMaxDistanceValue.text = ConvertFloatToDecimalNumberString(MetersToInches(cameraSettings.MaxDistance));
			inputMinDistanceField.text = ConvertFloatToDecimalNumberString(MetersToInches(cameraSettings.MinDistance));
			sliderExposure.value = cameraSettings.Exposure;
			sliderGain.value = cameraSettings.Gain;
			sliderMaxDistance.value = cameraSettings.MaxDistance;
			sliderMinDistance.value = cameraSettings.MinDistance;
			toggleUseLaser.isOn = cameraSettings.EmitterEnabled;
			UpdateExposureDistance();
			UpdateGainDistance();
			UpdateLaserPowerDistance();
			UpdateMaxDistance();
			UpdateMinDistance();
			inputExposureValue.onValueChanged.AddListener(ExposureInputChanged);
			inputGainValue.onValueChanged.AddListener(GainInputChanged);
			inputLaserPowerValue.onValueChanged.AddListener(LaserPowerInputChanged);
			inputMaxDistanceValue.onValueChanged.AddListener(MaxDistanceInputChanged);
			inputMinDistanceField.onValueChanged.AddListener(MinDistanceInputChanged);
			sliderExposure.onValueChanged.AddListener(ExposureSliderChanged);
			sliderGain.onValueChanged.AddListener(GainSliderChanged);
			sliderLaserPower.onValueChanged.AddListener(LaserPowerSliderChanged);
			sliderMaxDistance.onValueChanged.AddListener(MaxDistanceSliderChanged);
			sliderMinDistance.onValueChanged.AddListener(MinDistanceSliderChanged);
			toggleUseLaser.onValueChanged.AddListener(UseLaserChanged);
		}

		private string ConvertFloatToWholeNumberString(float value)
		{
			return Math.Floor(value).ToString(CultureInfo.InvariantCulture);
		}

		private string ConvertFloatToDecimalNumberString(float value)
		{
			return value.ToString("f3");
		}
	}
}
