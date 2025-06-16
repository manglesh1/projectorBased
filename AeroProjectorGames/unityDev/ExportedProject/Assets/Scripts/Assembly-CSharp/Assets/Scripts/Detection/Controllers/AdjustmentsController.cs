using Detection.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Detection.Controllers
{
	public class AdjustmentsController : MonoBehaviour
	{
		[Header("Scriptable Objects")]
		[SerializeField]
		private CoordinatesSO gameBoardCoordinates;

		[Header("UI Elements")]
		[SerializeField]
		private Slider downwardOffsetSlider;

		[SerializeField]
		private TMP_InputField downwardOffsetValue;

		private void HandleSliderChange(float value)
		{
			gameBoardCoordinates.DownwardOffset = (decimal)value;
			downwardOffsetValue.text = value.ToString("0.0");
		}

		private void OnDisable()
		{
			downwardOffsetSlider.onValueChanged.RemoveListener(HandleSliderChange);
		}

		private void OnEnable()
		{
			downwardOffsetValue.text = gameBoardCoordinates.DownwardOffset.ToString("0.0");
			downwardOffsetSlider.SetValueWithoutNotify((float)gameBoardCoordinates.DownwardOffset);
			downwardOffsetSlider.onValueChanged.AddListener(HandleSliderChange);
		}
	}
}
