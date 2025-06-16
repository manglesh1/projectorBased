using System;
using System.Collections.Generic;
using System.Linq;
using ResizingAndMoving;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FontSettings
{
	[CreateAssetMenu(menuName = "Font/Font Settings")]
	public class FontSettingsSO : ScriptableObject
	{
		private TMP_FontAsset _currentFontType;

		private FontSettingsStore _fontSettingStore;

		[SerializeField]
		private int fontSize = 28;

		[SerializeField]
		private int fontSizeIncrementAmount = 1;

		[SerializeField]
		private int maxFontSize = 28;

		[SerializeField]
		private int minFontSize = 10;

		[Header("Available Fonts")]
		[SerializeField]
		private List<TMP_FontAsset> fontSelectionList;

		[SerializeField]
		private SizeAndPositionStateSO scoreboardSizeAndPositionState;

		public UnityAction OnFontSizeChange;

		public UnityAction OnFontTypeChange;

		public TMP_FontAsset CurrentFontType
		{
			get
			{
				return _currentFontType;
			}
			set
			{
				_currentFontType = value;
				ExecuteFontTypeChange();
				SaveFontSettings();
			}
		}

		public int FontSize
		{
			get
			{
				return fontSize;
			}
			set
			{
				if (value < minFontSize)
				{
					fontSize = minFontSize;
				}
				else if (value > maxFontSize)
				{
					fontSize = maxFontSize;
				}
				else
				{
					fontSize = value;
				}
				ExecuteFontSizeChange();
				SaveFontSettings();
			}
		}

		private void OnEnable()
		{
			SizeAndPositionStateSO sizeAndPositionStateSO = scoreboardSizeAndPositionState;
			sizeAndPositionStateSO.OnReset = (UnityAction)Delegate.Combine(sizeAndPositionStateSO.OnReset, new UnityAction(ResetFontSettings));
			GetFontSettings();
		}

		public void IncrementFontSize()
		{
			FontSize += fontSizeIncrementAmount;
		}

		public void DecrementFontSize()
		{
			FontSize -= fontSizeIncrementAmount;
		}

		public void SetFontFromName(string fontFamily)
		{
			CurrentFontType = fontSelectionList.Find((TMP_FontAsset fa) => fa.faceInfo.familyName == fontFamily) ?? fontSelectionList.First();
		}

		private void ExecuteFontSizeChange()
		{
			OnFontSizeChange?.Invoke();
		}

		private void ExecuteFontTypeChange()
		{
			OnFontTypeChange?.Invoke();
		}

		private void GetFontSettings()
		{
			_fontSettingStore = SettingsStore.Font;
			fontSize = _fontSettingStore.FontSize;
			SetFontFromName(_fontSettingStore.FontType);
		}

		private void ResetFontSettings()
		{
			FontSize = _fontSettingStore.DefaultFontSize;
			CurrentFontType = fontSelectionList.First();
		}

		private void SaveFontSettings()
		{
			_fontSettingStore.FontSize = FontSize;
			_fontSettingStore.FontType = _currentFontType.faceInfo.familyName;
			_fontSettingStore.Save();
		}
	}
}
