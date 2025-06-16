using System;
using System.Collections.Generic;
using System.Linq;
using Games;
using Games.Models;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameAccessSettingsController : MonoBehaviour
{
	private List<GameObject> _gameButtonList = new List<GameObject>();

	private List<GameSO> _licensedGames = new List<GameSO>();

	private Paginator<GameSO> _paginator;

	private int _previousGameCount;

	private Dictionary<int, bool> _viewableGamesDictionary = new Dictionary<int, bool>();

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private GameObject gameAccessViewGameObject;

	[SerializeField]
	private GameEventsSO gameEvents;

	[SerializeField]
	private ViewableGamesSO viewableGames;

	[Header("Sprites")]
	[SerializeField]
	private Sprite enabledSprite;

	[SerializeField]
	private Sprite disabledSprite;

	[Header("Sprite Colors")]
	[SerializeField]
	private Color enabledSpriteColor;

	[SerializeField]
	private Color disabledSpriteColor;

	[Header("Pagination")]
	[SerializeField]
	[Range(1f, 12f)]
	private int pageSize;

	[SerializeField]
	private Button nextPageButton;

	[SerializeField]
	private Button previousPageButton;

	private void OnEnable()
	{
		LoadGameList();
	}

	private void OnDisable()
	{
	}

	private void AreGameButtonsInteractable(bool isInteractable)
	{
		foreach (GameObject gameButton in _gameButtonList)
		{
			gameButton.GetComponent<Button>().interactable = isInteractable;
		}
	}

	private bool GetCurrentVisibilityStatus(GameSO game)
	{
		return _viewableGamesDictionary.ContainsKey(game.GameId) && _viewableGamesDictionary[game.GameId];
	}

	private void LoadGameList()
	{
		_licensedGames = viewableGames.GetLicensedGames().ToList();
		_viewableGamesDictionary = viewableGames.GetViewableGamesDictionary();
		if (_previousGameCount != _licensedGames.Count)
		{
			if (_paginator != null)
			{
				Paginator<GameSO> paginator = _paginator;
				paginator.OnPagedChanged = (UnityAction)Delegate.Remove(paginator.OnPagedChanged, new UnityAction(LoadGameList));
			}
			nextPageButton.onClick.RemoveAllListeners();
			previousPageButton.onClick.RemoveAllListeners();
			_previousGameCount = _licensedGames.Count;
			_paginator = new Paginator<GameSO>(pageSize, _licensedGames);
			nextPageButton.onClick.AddListener(_paginator.NextPage);
			previousPageButton.onClick.AddListener(_paginator.PreviousPage);
			Paginator<GameSO> paginator2 = _paginator;
			paginator2.OnPagedChanged = (UnityAction)Delegate.Combine(paginator2.OnPagedChanged, new UnityAction(LoadGameList));
		}
		foreach (GameObject gameButton in _gameButtonList)
		{
			UnityEngine.Object.Destroy(gameButton);
		}
		_gameButtonList.Clear();
		foreach (GameSO game in _paginator.CurrentPage)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(buttonPrefab, gameAccessViewGameObject.transform);
			gameObject.name = game.GameName;
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<Image>().sprite = game.GameIcon;
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = game.GameName;
			GameObject gameObject2 = new GameObject(game.GameName);
			gameObject2.transform.SetParent(gameObject.transform);
			Image _image = gameObject2.AddComponent<Image>();
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				ToggleEnableAndDisabled(game, _image);
			});
			RectTransform component = gameObject2.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.offsetMin = Vector2.zero;
			component.offsetMax = Vector2.zero;
			if (GetCurrentVisibilityStatus(game))
			{
				_image.sprite = enabledSprite;
				_image.color = enabledSpriteColor;
			}
			else
			{
				_image.sprite = disabledSprite;
				_image.color = disabledSpriteColor;
			}
			_gameButtonList.Add(gameObject);
			gameObject.SetActive(value: true);
		}
	}

	private void ToggleEnableAndDisabled(GameSO game, Image accessImage)
	{
		if (GetCurrentVisibilityStatus(game))
		{
			viewableGames.AddViewableGamesToSettings(new Dictionary<int, bool> { { game.GameId, false } });
			accessImage.sprite = disabledSprite;
			accessImage.color = disabledSpriteColor;
		}
		else
		{
			viewableGames.AddViewableGamesToSettings(new Dictionary<int, bool> { { game.GameId, true } });
			accessImage.sprite = enabledSprite;
			accessImage.color = enabledSpriteColor;
		}
		gameEvents.RaiseGameLicensedListUpdated();
	}
}
