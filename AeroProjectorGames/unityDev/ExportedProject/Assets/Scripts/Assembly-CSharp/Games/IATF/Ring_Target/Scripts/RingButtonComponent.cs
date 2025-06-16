using System;
using ResizingAndMoving;
using Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Games.IATF.Ring_Target.Scripts
{
	public class RingButtonComponent : MonoBehaviour
	{
		private const float ALPHA_THRESHOLD = 0.5f;

		private const int BACKGROUND_PADDING = 10;

		private RingSettings _settings;

		[SerializeField]
		private Image background;

		[SerializeField]
		private IATFComponent component;

		[SerializeField]
		private SizeAndPositionStateSO sizeAndPositionState;

		private void OnEnable()
		{
			_settings = SettingsStore.IATFTarget.RingSettingsMap[component];
			background.alphaHitTestMinimumThreshold = 0.5f;
			LoadFromSettings();
			SetBackgroundOffset();
			SizeAndPositionStateSO sizeAndPositionStateSO = sizeAndPositionState;
			sizeAndPositionStateSO.OnSizeChange = (UnityAction)Delegate.Combine(sizeAndPositionStateSO.OnSizeChange, new UnityAction(HandleChange));
			SizeAndPositionStateSO sizeAndPositionStateSO2 = sizeAndPositionState;
			sizeAndPositionStateSO2.OnPositionChange = (UnityAction)Delegate.Combine(sizeAndPositionStateSO2.OnPositionChange, new UnityAction(HandleChange));
		}

		private void OnDisable()
		{
			SizeAndPositionStateSO sizeAndPositionStateSO = sizeAndPositionState;
			sizeAndPositionStateSO.OnSizeChange = (UnityAction)Delegate.Remove(sizeAndPositionStateSO.OnSizeChange, new UnityAction(HandleChange));
			SizeAndPositionStateSO sizeAndPositionStateSO2 = sizeAndPositionState;
			sizeAndPositionStateSO2.OnPositionChange = (UnityAction)Delegate.Remove(sizeAndPositionStateSO2.OnPositionChange, new UnityAction(HandleChange));
		}

		private void LoadFromSettings()
		{
			sizeAndPositionState.Height = _settings.Height;
			sizeAndPositionState.Width = _settings.Width;
			sizeAndPositionState.SetPosition(_settings.PositionX, _settings.PositionY);
			SettingsStore.IATFTarget.Save();
		}

		private void HandleChange()
		{
			_settings.Height = sizeAndPositionState.Height;
			_settings.Width = sizeAndPositionState.Width;
			_settings.PositionX = sizeAndPositionState.PositionX;
			_settings.PositionY = sizeAndPositionState.PositionY;
			SetBackgroundOffset();
			SettingsStore.IATFTarget.Save();
		}

		private void SetBackgroundOffset()
		{
			RectTransform rectTransform = background.GetComponent<RectTransform>();
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeAndPositionState.Width - 10f);
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeAndPositionState.Height - 10f);
		}
	}
}
