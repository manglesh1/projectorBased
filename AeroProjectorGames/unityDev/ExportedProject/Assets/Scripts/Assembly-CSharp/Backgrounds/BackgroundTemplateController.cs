using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Backgrounds
{
	public class BackgroundTemplateController : MonoBehaviour
	{
		private const string DEFAULT_NAME = "";

		private const string NO_GAMES_APPLIED = "None";

		[SerializeField]
		private Slider brightnessSlider;

		[SerializeField]
		private Button button;

		[SerializeField]
		private Image backgroundImagePreview;

		[SerializeField]
		private TMP_Text backgroundTitle;

		[SerializeField]
		private TMP_Text gamesAppliedToBackground;

		private void OnDestroy()
		{
			button.onClick.RemoveAllListeners();
			brightnessSlider.onValueChanged.RemoveAllListeners();
		}

		private void OnEnable()
		{
		}

		private void HandleAlphaChange(float alpha)
		{
			backgroundImagePreview.color = new Color(backgroundImagePreview.color.r, backgroundImagePreview.color.g, backgroundImagePreview.color.b, alpha / 100f);
		}

		public void SetupImage(string title, bool gamesApplied, Texture2D texture2D, float brightness, Action onClick, Action<float> brightnessOnChange)
		{
			backgroundTitle.text = title;
			UpdateInUseText(gamesApplied);
			backgroundImagePreview.color = new Color(1f, 1f, 1f, brightness);
			backgroundImagePreview.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
			brightnessSlider.SetValueWithoutNotify(brightness * 100f);
			brightnessSlider.onValueChanged.AddListener(brightnessOnChange.Invoke);
			brightnessSlider.onValueChanged.AddListener(HandleAlphaChange);
			button.onClick.AddListener(onClick.Invoke);
		}

		public void SetupColor(string title, bool gamesApplied, Color color, Action onClick, Action<float> brightnessOnChange)
		{
			backgroundTitle.text = title;
			UpdateInUseText(gamesApplied);
			backgroundImagePreview.color = color;
			brightnessSlider.SetValueWithoutNotify(color.a * 100f);
			brightnessSlider.onValueChanged.AddListener(brightnessOnChange.Invoke);
			brightnessSlider.onValueChanged.AddListener(HandleAlphaChange);
			button.onClick.AddListener(onClick.Invoke);
		}

		public void UpdateInUseText(bool gamesApplied)
		{
			gamesAppliedToBackground.text = (gamesApplied ? "In Use" : "Not in Use");
		}
	}
}
