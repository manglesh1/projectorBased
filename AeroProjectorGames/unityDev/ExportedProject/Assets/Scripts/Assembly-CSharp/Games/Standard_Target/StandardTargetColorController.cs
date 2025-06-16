using System.Collections.Generic;
using Settings;
using Target;
using UnityEngine;

namespace Games.Standard_Target
{
	public class StandardTargetColorController : MonoBehaviour
	{
		private TargetColorTheme _selectedColorTheme;

		private TargetColorSettings _targetColorSettings;

		private bool _useSixRingTarget;

		[SerializeField]
		private List<TargetColorTheme> targetColorthemes;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnEnable()
		{
			gameEvents.OnNewGame += HandleResetValues;
		}

		private void OnDisable()
		{
			gameEvents.OnNewGame -= HandleResetValues;
		}

		public bool CheckIfPlayerChoosesTargetColor()
		{
			GetTargetColorSettings();
			return _targetColorSettings.LetPlayerChooseColor;
		}

		public RingThemeModel GetRingSettings(int scoreValue)
		{
			if (_selectedColorTheme == null)
			{
				GetTargetColorSettings();
				StoreSelectedColor();
			}
			_useSixRingTarget = SettingsStore.Target.UseSixRingTarget;
			bool alternateColors = _selectedColorTheme.AlternateColors;
			switch (scoreValue)
			{
			case 1:
				if (_useSixRingTarget || !alternateColors)
				{
					return _selectedColorTheme.RingValue1;
				}
				return _selectedColorTheme.RingValue4;
			case 2:
				if (_useSixRingTarget || !alternateColors)
				{
					return _selectedColorTheme.RingValue2;
				}
				return _selectedColorTheme.RingValue1;
			case 3:
				if (_useSixRingTarget || !alternateColors)
				{
					return _selectedColorTheme.RingValue3;
				}
				return _selectedColorTheme.RingValue2;
			case 4:
				if (_useSixRingTarget || !alternateColors)
				{
					return _selectedColorTheme.RingValue4;
				}
				return _selectedColorTheme.RingValue3;
			case 5:
				return _selectedColorTheme.RingValue5;
			case 6:
				return _selectedColorTheme.RingValue6;
			case 8:
				if (_useSixRingTarget || !alternateColors)
				{
					return _selectedColorTheme.RingValue8;
				}
				return _selectedColorTheme.RingValue1;
			default:
				return new RingThemeModel();
			}
		}

		private void GetTargetColorSettings()
		{
			_targetColorSettings = SettingsStore.TargetColor;
		}

		private void HandleResetValues()
		{
			_selectedColorTheme = null;
		}

		public void SetPlayerSelectedTargetColor(TargetColorEnum playerSelectedColor)
		{
			GetTargetColorSettings();
			_targetColorSettings.ChosenTargetColor = playerSelectedColor;
			_targetColorSettings.Save();
		}

		private void StoreSelectedColor()
		{
			TargetColorEnum chosenTargetColor = _targetColorSettings.ChosenTargetColor;
			foreach (TargetColorTheme targetColortheme in targetColorthemes)
			{
				if (targetColortheme.TargetColorThemeName == chosenTargetColor)
				{
					_selectedColorTheme = targetColortheme;
				}
			}
		}
	}
}
