using System;
using System.Collections.Generic;
using FontSettings;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FontSettingComponent : MonoBehaviour
{
	[SerializeField]
	private bool applyResizing = true;

	[SerializeField]
	private FontSettingsSO fontSettings;

	[SerializeField]
	private List<TMP_Text> textList;

	[Header("Font Modifiers")]
	[SerializeField]
	private bool useFontModifiers;

	[SerializeField]
	private int fontSizeMultiplier;

	private void OnEnable()
	{
		FontSettingsSO fontSettingsSO = fontSettings;
		fontSettingsSO.OnFontSizeChange = (UnityAction)Delegate.Combine(fontSettingsSO.OnFontSizeChange, new UnityAction(UpdateFontSizeText));
		FontSettingsSO fontSettingsSO2 = fontSettings;
		fontSettingsSO2.OnFontTypeChange = (UnityAction)Delegate.Combine(fontSettingsSO2.OnFontTypeChange, new UnityAction(UpdateFontType));
		UpdateFontSizeText();
		UpdateFontType();
	}

	private void OnDisable()
	{
		FontSettingsSO fontSettingsSO = fontSettings;
		fontSettingsSO.OnFontSizeChange = (UnityAction)Delegate.Remove(fontSettingsSO.OnFontSizeChange, new UnityAction(UpdateFontSizeText));
		FontSettingsSO fontSettingsSO2 = fontSettings;
		fontSettingsSO2.OnFontTypeChange = (UnityAction)Delegate.Remove(fontSettingsSO2.OnFontTypeChange, new UnityAction(UpdateFontType));
	}

	private void UpdateFontSizeText()
	{
		if (!applyResizing)
		{
			return;
		}
		foreach (TMP_Text text in textList)
		{
			text.fontSize = (useFontModifiers ? (fontSettings.FontSize * fontSizeMultiplier) : fontSettings.FontSize);
		}
	}

	private void UpdateFontType()
	{
		foreach (TMP_Text text in textList)
		{
			text.font = fontSettings.CurrentFontType;
		}
	}
}
