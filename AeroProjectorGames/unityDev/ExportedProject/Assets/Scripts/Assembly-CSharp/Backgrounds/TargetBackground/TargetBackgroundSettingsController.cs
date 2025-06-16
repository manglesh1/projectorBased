using ResizingAndMoving;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Backgrounds.TargetBackground
{
	public class TargetBackgroundSettingsController : MonoBehaviour
	{
		[Header("Brightness Slider")]
		[SerializeField]
		private Slider brightnessSlider;

		[Header("Large Increments?")]
		[SerializeField]
		private Toggle useLargeIncrementsToggle;

		[Header("ScriptableObject Refs")]
		[SerializeField]
		private BackgroundEventsSO backgroundEvents;

		[SerializeField]
		private SizeAndPositionStateSO targetDimmerSizeAndPositionState;

		private void OnDisable()
		{
			SettingsStore.Backgrounds.Save();
			brightnessSlider.onValueChanged.RemoveListener(HandleBrightnessChanged);
			useLargeIncrementsToggle.onValueChanged.RemoveListener(HandleUseLargeIncrementsToggle);
		}

		private void OnEnable()
		{
			targetDimmerSizeAndPositionState.SetLargeIncrements(large: false);
			useLargeIncrementsToggle.SetIsOnWithoutNotify(value: false);
			brightnessSlider.SetValueWithoutNotify(SettingsStore.Backgrounds.TargetDimmer.Alpha * 100f);
			brightnessSlider.onValueChanged.AddListener(HandleBrightnessChanged);
			useLargeIncrementsToggle.onValueChanged.AddListener(HandleUseLargeIncrementsToggle);
		}

		public void DecrementHeight()
		{
			targetDimmerSizeAndPositionState.DecrementHeight();
			SettingsStore.Backgrounds.TargetDimmer.Height = targetDimmerSizeAndPositionState.Height;
		}

		public void IncrementHeight()
		{
			targetDimmerSizeAndPositionState.IncrementHeight();
			SettingsStore.Backgrounds.TargetDimmer.Height = targetDimmerSizeAndPositionState.Height;
		}

		public void DecrementWidth()
		{
			targetDimmerSizeAndPositionState.DecrementWidth();
			SettingsStore.Backgrounds.TargetDimmer.Width = targetDimmerSizeAndPositionState.Width;
		}

		public void IncrementWidth()
		{
			targetDimmerSizeAndPositionState.IncrementWidth();
			SettingsStore.Backgrounds.TargetDimmer.Width = targetDimmerSizeAndPositionState.Width;
		}

		public void MoveDown()
		{
			targetDimmerSizeAndPositionState.MoveDown();
			SettingsStore.Backgrounds.TargetDimmer.PositionY = targetDimmerSizeAndPositionState.PositionY;
		}

		public void MoveUp()
		{
			targetDimmerSizeAndPositionState.MoveUp();
			SettingsStore.Backgrounds.TargetDimmer.PositionY = targetDimmerSizeAndPositionState.PositionY;
		}

		public void MoveLeft()
		{
			targetDimmerSizeAndPositionState.MoveLeft();
			SettingsStore.Backgrounds.TargetDimmer.PositionX = targetDimmerSizeAndPositionState.PositionX;
		}

		public void MoveRight()
		{
			targetDimmerSizeAndPositionState.MoveRight();
			SettingsStore.Backgrounds.TargetDimmer.PositionX = targetDimmerSizeAndPositionState.PositionX;
		}

		public void Reset()
		{
			SettingsStore.Backgrounds.TargetDimmer.SetDefaults();
			backgroundEvents.RaiseTargetDimmerChanged();
			targetDimmerSizeAndPositionState.RaiseReset();
			brightnessSlider.SetValueWithoutNotify(0f);
		}

		private void HandleBrightnessChanged(float newValue)
		{
			SettingsStore.Backgrounds.TargetDimmer.Alpha = newValue / 100f;
			backgroundEvents.RaiseTargetDimmerChanged();
		}

		private void HandleUseLargeIncrementsToggle(bool largeIncrements)
		{
			targetDimmerSizeAndPositionState.SetLargeIncrements(largeIncrements);
		}
	}
}
