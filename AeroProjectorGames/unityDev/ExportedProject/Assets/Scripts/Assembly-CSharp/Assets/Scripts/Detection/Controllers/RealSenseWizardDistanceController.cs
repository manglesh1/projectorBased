using Assets.Scripts.Detection.Models;
using Assets.Scripts.Detection.ScriptableObjects;
using Detection.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Detection.Controllers
{
	public class RealSenseWizardDistanceController : MonoBehaviour
	{
		[Header("UI Components")]
		[SerializeField]
		private Slider valueSlider;

		[SerializeField]
		private TMP_InputField valueTextField;

		[Header("Events")]
		[SerializeField]
		private RealSenseWizardEventsSO events;

		[Header("Scriptable Objects")]
		[SerializeField]
		private RealSenseCameraSettingsSO cameraSettings;

		[SerializeField]
		private CameraDistanceEnum distance;

		private const string FORMAT_NUMBER = "N0";

		private const float INCHES_MULTIPLIER = 39.37008f;

		private const float METERS_MULTIPLIER = 0.0254f;

		private const int MAX_VALUE = 120;

		private const int MIN_VALUE = 1;

		private void valueTextField_onValueChanged(string value)
		{
			if (int.TryParse(value, out var result) && IsInRange(result))
			{
				valueSlider.SetValueWithoutNotify(result);
				RaiseValue(result);
			}
			else
			{
				valueSlider.SetValueWithoutNotify(1f);
				RaiseValue(1f);
			}
		}

		private void valueSlider_onValueChanged(float value)
		{
			valueTextField.SetTextWithoutNotify(value.ToString("N0"));
			RaiseValue(value);
		}

		private float InchesToMeters(float inches)
		{
			return inches * 0.0254f;
		}

		private void Initialize()
		{
			valueSlider.maxValue = 120f;
			valueSlider.minValue = 1f;
			int num = (int)MetersToInches((distance == CameraDistanceEnum.Maximum) ? cameraSettings.MaxDistance : cameraSettings.MinDistance);
			valueSlider.SetValueWithoutNotify(num);
			valueTextField.SetTextWithoutNotify(num.ToString("N0"));
		}

		private bool IsInRange(int numericValue)
		{
			return numericValue >= 1 && numericValue <= 120;
		}

		private float MetersToInches(float meters)
		{
			return meters * 39.37008f;
		}

		private void RaiseValue(float value)
		{
			float value2 = InchesToMeters(value);
			switch (distance)
			{
			case CameraDistanceEnum.Maximum:
				events.RaiseOnMaxValueChanged(value2);
				break;
			case CameraDistanceEnum.Minimum:
				events.RaiseOnMinValueChanged(value2);
				break;
			}
		}

		private void OnDisable()
		{
			valueSlider.onValueChanged.RemoveAllListeners();
			valueTextField.onValueChanged.RemoveAllListeners();
		}

		private void OnEnable()
		{
			Initialize();
			valueSlider.onValueChanged.AddListener(valueSlider_onValueChanged);
			valueTextField.onValueChanged.AddListener(valueTextField_onValueChanged);
		}
	}
}
