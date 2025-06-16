using Settings;
using Target;
using UnityEngine;
using UnityEngine.UI;

namespace Admin_Panel
{
	public class AdminStandardTargetColorController : MonoBehaviour
	{
		[SerializeField]
		private Toggle defaultColorToggle;

		[SerializeField]
		private Toggle letPlayerChooseColorToggle;

		[SerializeField]
		private Toggle neonColorToggle;

		[SerializeField]
		private Toggle redAndWhiteColorToggle;

		[Header("Settings Elements")]
		private TargetColorSettings _targetColorSettings;

		private void OnEnable()
		{
			GetTargetColorSettings();
			SetProperToggleFromSettings();
		}

		private void GetTargetColorSettings()
		{
			_targetColorSettings = SettingsStore.TargetColor;
		}

		private void SetNewColorSetting(TargetColorEnum newColor, bool letPlayerChooseColor = false)
		{
			_targetColorSettings.ChosenTargetColor = newColor;
			_targetColorSettings.LetPlayerChooseColor = letPlayerChooseColor;
			_targetColorSettings.Save();
		}

		private void SetProperToggleFromSettings()
		{
			if (_targetColorSettings.LetPlayerChooseColor)
			{
				letPlayerChooseColorToggle.isOn = true;
				return;
			}
			switch (_targetColorSettings.ChosenTargetColor)
			{
			case TargetColorEnum.Neon:
				neonColorToggle.isOn = true;
				break;
			case TargetColorEnum.RedAndWhite:
				redAndWhiteColorToggle.isOn = true;
				break;
			default:
				defaultColorToggle.isOn = true;
				break;
			}
		}

		public void DefaultColorValueChange(Toggle newValue)
		{
			SetNewColorSetting(TargetColorEnum.Default);
		}

		public void LetPlayerChooseColorValueChange(Toggle newValue)
		{
			SetNewColorSetting(TargetColorEnum.Default, letPlayerChooseColor: true);
		}

		public void NeonColorValueChange(Toggle newValue)
		{
			SetNewColorSetting(TargetColorEnum.Neon);
		}

		public void RedAndWhiteColorValueChange(Toggle newValue)
		{
			SetNewColorSetting(TargetColorEnum.RedAndWhite);
		}
	}
}
