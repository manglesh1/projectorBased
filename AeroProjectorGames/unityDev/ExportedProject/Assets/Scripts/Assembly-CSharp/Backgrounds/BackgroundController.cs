using System.Collections.Generic;
using System.Linq;
using Colors;
using Games;
using Games.GameState;
using Games.Models;
using Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Backgrounds
{
	public class BackgroundController : MonoBehaviour
	{
		[SerializeField]
		private Color defaultBackgroundColor;

		[SerializeField]
		private GameObject backgroundObject;

		[SerializeField]
		private Image colorBackground;

		[SerializeField]
		private Image imageBackground;

		[Header("References")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private BackgroundsSO backgrounds;

		[Header("Games To Exclude From Using Backgrounds")]
		[Tooltip("List of game SO's to exclude from using backgrounds")]
		[SerializeField]
		private List<GameSO> excludeGamesBySO = new List<GameSO>();

		private void OnDisable()
		{
			gameEvents.OnGameLoaded -= LoadBackground;
			gameEvents.OnMainMenu -= UnloadBackground;
		}

		private void OnEnable()
		{
			gameEvents.OnGameLoaded += LoadBackground;
			gameEvents.OnMainMenu += UnloadBackground;
		}

		private bool CheckForExcludedGames(int gameIdToCheck)
		{
			foreach (GameSO item in excludeGamesBySO)
			{
				if (gameIdToCheck == item.GameId)
				{
					return true;
				}
			}
			return false;
		}

		private void LoadDefaultColor()
		{
			colorBackground.color = defaultBackgroundColor;
			ShowColorBackground();
		}

		private void LoadBackground()
		{
			if (CheckForExcludedGames(gameState.LoadedGame.GameId))
			{
				return;
			}
			SettingsStore.Backgrounds.GameBackgrounds.TryGetValue(gameState.LoadedGame.GameId, out var backgroundKey);
			BackgroundSetting backgroundSetting = SettingsStore.Backgrounds.Backgrounds.FirstOrDefault((BackgroundSetting b) => b.Name == backgroundKey);
			if (backgroundKey == null || backgroundSetting == null)
			{
				LoadDefaultColor();
			}
			if (backgroundSetting != null && backgroundSetting.BackgroundStyle == BackgroundStyleEnum.Color)
			{
				colorBackground.color = Hex.ToColor(backgroundSetting.ColorHexValue);
				ShowColorBackground();
			}
			if (backgroundSetting != null && backgroundSetting.BackgroundStyle == BackgroundStyleEnum.PredefinedColor)
			{
				Color color = backgrounds.LoadedBackgroundColors[backgroundKey];
				colorBackground.color = new Color(color.r, color.g, color.b, backgroundSetting.Alpha);
				ShowColorBackground();
			}
			if ((backgroundSetting != null && backgroundSetting.BackgroundStyle == BackgroundStyleEnum.PredefinedImage) || (backgroundSetting != null && backgroundSetting.BackgroundStyle == BackgroundStyleEnum.CustomImage))
			{
				if (!backgrounds.LoadedBackgroundImages.ContainsKey(backgroundKey))
				{
					imageBackground.color = new Color(0f, 0f, 0f, 1f);
					imageBackground.sprite = null;
					return;
				}
				Texture2D texture2D = backgrounds.LoadedBackgroundImages[backgroundKey];
				imageBackground.color = new Color(1f, 1f, 1f, backgroundSetting.Alpha);
				imageBackground.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				ShowImageBackground();
			}
			backgroundObject.SetActive(value: true);
		}

		private void ShowColorBackground()
		{
			colorBackground.gameObject.SetActive(value: true);
			imageBackground.gameObject.SetActive(value: false);
		}

		private void ShowImageBackground()
		{
			colorBackground.gameObject.SetActive(value: false);
			imageBackground.gameObject.SetActive(value: true);
		}

		private void UnloadBackground()
		{
			backgroundObject.SetActive(value: false);
		}
	}
}
