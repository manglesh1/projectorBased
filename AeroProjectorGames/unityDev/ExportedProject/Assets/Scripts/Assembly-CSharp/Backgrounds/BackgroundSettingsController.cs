using System;
using System.Collections.Generic;
using System.Linq;
using Games;
using Settings;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Backgrounds
{
	public class BackgroundSettingsController : MonoBehaviour
	{
		private Dictionary<string, BackgroundTemplateController> _backgrounds;

		private Paginator<BackgroundTemplateController> _paginator;

		private string _selectedBackground;

		[Header("Games Paginator")]
		[SerializeField]
		private PaginatedGameToggleController paginatedGameToggleController;

		[SerializeField]
		private GameObject paginatedGamesPanel;

		[Header("Pagination Controls")]
		[SerializeField]
		private Button pageBackButton;

		[SerializeField]
		private Button pageForwardButton;

		[Header("Background Refs")]
		[SerializeField]
		private GameObject backgroundPanel;

		[SerializeField]
		private BackgroundsSO backgroundStore;

		[SerializeField]
		private BackgroundTemplateController backgroundTemplatePrefab;

		private void OnDisable()
		{
			SettingsStore.Backgrounds.Save();
			if (_paginator != null)
			{
				Paginator<BackgroundTemplateController> paginator = _paginator;
				paginator.OnPagedChanged = (UnityAction)Delegate.Remove(paginator.OnPagedChanged, new UnityAction(PaginateBackgrounds));
			}
			pageBackButton.onClick.RemoveAllListeners();
			pageForwardButton.onClick.RemoveAllListeners();
			foreach (KeyValuePair<string, BackgroundTemplateController> background in _backgrounds)
			{
				UnityEngine.Object.DestroyImmediate(background.Value.gameObject);
			}
			_backgrounds?.Clear();
			paginatedGameToggleController.OnCloseRequest -= HandlePaginatedGameTogglePanelCloseRequest;
			paginatedGameToggleController.OnGameToggleChanged -= HandleGameToggleBackgroundChanged;
		}

		private void OnEnable()
		{
			paginatedGamesPanel.SetActive(value: false);
			if (backgroundStore.LoadedBackgroundImages.Count > 0 && backgroundStore.LoadedBackgroundColors.Count > 0)
			{
				Load();
			}
			paginatedGameToggleController.OnCloseRequest += HandlePaginatedGameTogglePanelCloseRequest;
			paginatedGameToggleController.OnGameToggleChanged += HandleGameToggleBackgroundChanged;
		}

		private void HandleBackgroundAlphaChanged(string backgroundKey, float alpha)
		{
			float alpha2 = ((alpha <= 0f || alpha > 100f) ? 1f : (alpha / 100f));
			BackgroundSetting backgroundSetting = SettingsStore.Backgrounds.Backgrounds.FirstOrDefault((BackgroundSetting b) => b.Name == backgroundKey);
			if (backgroundSetting != null)
			{
				backgroundSetting.Alpha = alpha2;
			}
		}

		private void HandleGameToggleBackgroundChanged(int gameId, bool selected)
		{
			if (!selected)
			{
				SettingsStore.Backgrounds.GameBackgrounds.Remove(gameId);
			}
			else
			{
				SettingsStore.Backgrounds.GameBackgrounds[gameId] = _selectedBackground;
			}
			SettingsStore.Backgrounds.Save();
		}

		private void HandlePaginatedGameTogglePanelCloseRequest()
		{
			foreach (KeyValuePair<string, BackgroundTemplateController> pair in _backgrounds)
			{
				pair.Value.UpdateInUseText(SettingsStore.Backgrounds.GameBackgrounds.Any((KeyValuePair<int, string> b) => b.Value == pair.Key));
			}
			_selectedBackground = string.Empty;
			paginatedGamesPanel.SetActive(value: false);
		}

		private void Load()
		{
			_backgrounds = new Dictionary<string, BackgroundTemplateController>();
			foreach (KeyValuePair<string, Color> background in backgroundStore.LoadedBackgroundColors)
			{
				BackgroundTemplateController backgroundTemplateController = UnityEngine.Object.Instantiate(backgroundTemplatePrefab, backgroundPanel.transform);
				bool gamesApplied = SettingsStore.Backgrounds.GameBackgrounds.Any((KeyValuePair<int, string> b) => b.Value == background.Key);
				float a = (from b in SettingsStore.Backgrounds.Backgrounds
					where b.Name == background.Key
					select b.Alpha).First();
				backgroundTemplateController.SetupColor(color: new Color(background.Value.r, background.Value.g, background.Value.b, a), title: background.Key, gamesApplied: gamesApplied, onClick: delegate
				{
					ShowPaginatedGamesList(background.Key);
				}, brightnessOnChange: delegate(float brightness)
				{
					HandleBackgroundAlphaChanged(background.Key, brightness);
				});
				backgroundTemplateController.gameObject.SetActive(value: false);
				_backgrounds.Add(background.Key, backgroundTemplateController);
			}
			foreach (KeyValuePair<string, Texture2D> background2 in backgroundStore.LoadedBackgroundImages)
			{
				BackgroundSetting backgroundSetting = SettingsStore.Backgrounds.Backgrounds.Find((BackgroundSetting bg) => bg.Name == background2.Key);
				BackgroundTemplateController backgroundTemplateController2 = UnityEngine.Object.Instantiate(backgroundTemplatePrefab, backgroundPanel.transform);
				bool gamesApplied2 = SettingsStore.Backgrounds.GameBackgrounds.Any((KeyValuePair<int, string> b) => b.Value == background2.Key);
				(from b in SettingsStore.Backgrounds.Backgrounds
					where b.Name == background2.Key
					select b.Alpha).First();
				string title = ((backgroundSetting.BackgroundStyle == BackgroundStyleEnum.CustomImage) ? "Custom" : backgroundSetting.Name);
				backgroundTemplateController2.SetupImage(title, gamesApplied2, background2.Value, backgroundSetting.Alpha, delegate
				{
					ShowPaginatedGamesList(background2.Key);
				}, delegate(float brightness)
				{
					HandleBackgroundAlphaChanged(background2.Key, brightness);
				});
				backgroundTemplateController2.gameObject.SetActive(value: false);
				_backgrounds.Add(background2.Key, backgroundTemplateController2);
			}
			_paginator = new Paginator<BackgroundTemplateController>(1, _backgrounds.Values.ToList());
			Paginator<BackgroundTemplateController> paginator = _paginator;
			paginator.OnPagedChanged = (UnityAction)Delegate.Combine(paginator.OnPagedChanged, new UnityAction(PaginateBackgrounds));
			pageBackButton.onClick.AddListener(_paginator.PreviousPage);
			pageForwardButton.onClick.AddListener(_paginator.NextPage);
			PaginateBackgrounds();
		}

		private void PaginateBackgrounds()
		{
			foreach (KeyValuePair<string, BackgroundTemplateController> background in _backgrounds)
			{
				background.Value.gameObject.SetActive(value: false);
			}
			_paginator.CurrentPage[0].gameObject.SetActive(value: true);
		}

		private void ShowPaginatedGamesList(string backgroundName)
		{
			_selectedBackground = backgroundName;
			List<int> selectedGames = SettingsStore.Backgrounds.GameBackgrounds.Where(delegate(KeyValuePair<int, string> kv)
			{
				KeyValuePair<int, string> keyValuePair = kv;
				return keyValuePair.Value == backgroundName;
			}).Select(delegate(KeyValuePair<int, string> kv)
			{
				KeyValuePair<int, string> keyValuePair = kv;
				return keyValuePair.Key;
			}).ToList();
			paginatedGamesPanel.SetActive(value: true);
			paginatedGameToggleController.Load(selectedGames);
		}
	}
}
