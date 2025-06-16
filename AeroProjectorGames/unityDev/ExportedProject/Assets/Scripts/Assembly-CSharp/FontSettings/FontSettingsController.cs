using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FontSettings
{
	public class FontSettingsController : MonoBehaviour
	{
		[SerializeField]
		private FontSettingsSO fontSettings;

		[SerializeField]
		private TMP_Text fontSizeText;

		[SerializeField]
		private TMP_Dropdown fontTypeDropdown;

		[SerializeField]
		private TMP_Text fontTypeText;

		private void OnEnable()
		{
			FontSettingsSO fontSettingsSO = fontSettings;
			fontSettingsSO.OnFontSizeChange = (UnityAction)Delegate.Combine(fontSettingsSO.OnFontSizeChange, new UnityAction(UpdateFontSizeText));
			FontSettingsSO fontSettingsSO2 = fontSettings;
			fontSettingsSO2.OnFontTypeChange = (UnityAction)Delegate.Combine(fontSettingsSO2.OnFontTypeChange, new UnityAction(SetCurrentFontType));
			fontSizeText.text = fontSettings.FontSize.ToString();
			SetCurrentFontType();
		}

		private void OnDisable()
		{
			FontSettingsSO fontSettingsSO = fontSettings;
			fontSettingsSO.OnFontSizeChange = (UnityAction)Delegate.Remove(fontSettingsSO.OnFontSizeChange, new UnityAction(UpdateFontSizeText));
			FontSettingsSO fontSettingsSO2 = fontSettings;
			fontSettingsSO2.OnFontTypeChange = (UnityAction)Delegate.Remove(fontSettingsSO2.OnFontTypeChange, new UnityAction(SetCurrentFontType));
		}

		public void DecrementFontSize()
		{
			fontSettings.DecrementFontSize();
		}

		public void IncrementFontSize()
		{
			fontSettings.IncrementFontSize();
		}

		private void SetCurrentFontType()
		{
			fontTypeDropdown.value = fontTypeDropdown.options.FindIndex((TMP_Dropdown.OptionData option) => option.text == fontSettings.CurrentFontType.faceInfo.familyName);
			UpdateCurrentFontType();
		}

		private void UpdateCurrentFontType()
		{
			fontTypeText.font = fontSettings.CurrentFontType;
		}

		public void UpdateFontSizeText()
		{
			fontSizeText.text = fontSettings.FontSize.ToString();
		}

		public void SetNewFontType()
		{
			string text = fontTypeDropdown.options[fontTypeDropdown.value].text;
			fontSettings.SetFontFromName(text);
		}
	}
}
