using System;
using System.Collections.Generic;
using Games;
using Games.Models;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Detection.Controllers
{
	public class PaginatedDetectionGamesController : MonoBehaviour
	{
		private int _currentPageIndex;

		private int _totalPages;

		private List<GameObject> _gameButtonList;

		private List<List<GameSO>> _pagedGamesList;

		[SerializeField]
		[Range(1f, 4f)]
		private int pageSize;

		[Header("Pagination Buttons")]
		[SerializeField]
		private Button nextPageButton;

		[SerializeField]
		private Button previousPageButton;

		[SerializeField]
		private GameObject gameButtonView;

		[Header("Button References")]
		[SerializeField]
		private GameObject gameButtonPrefab;

		[Header("Game Enable/Disable Assets")]
		[SerializeField]
		private Color enabledColor;

		[SerializeField]
		private Sprite enabledSprite;

		[SerializeField]
		private Color disabledColor;

		[SerializeField]
		private Sprite disabledSprite;

		[Header("Game References")]
		[SerializeField]
		private ViewableGamesSO availableGames;

		private void OnDisable()
		{
			nextPageButton.onClick.RemoveListener(NextPage);
			previousPageButton.onClick.RemoveListener(PreviousPage);
			SettingsStore.GameAccessVisibility.DetectionEnabledForGameId.Clear();
			foreach (GameSO viewableGame in availableGames.GetViewableGames())
			{
				if (viewableGame.DetectionEnabled)
				{
					SettingsStore.GameAccessVisibility.DetectionEnabledForGameId.Add(viewableGame.GameId);
				}
			}
			SettingsStore.GameAccessVisibility.Save();
		}

		private void OnEnable()
		{
			nextPageButton.onClick.AddListener(NextPage);
			previousPageButton.onClick.AddListener(PreviousPage);
			if (_gameButtonList == null)
			{
				_gameButtonList = new List<GameObject>();
			}
			_pagedGamesList = new List<List<GameSO>>();
			List<GameSO> viewableGames = availableGames.GetViewableGames();
			_totalPages = (int)Math.Ceiling((float)viewableGames.Count / (float)pageSize);
			for (int i = 1; i <= _totalPages; i++)
			{
				int num = (i - 1) * pageSize;
				int count = pageSize;
				if (viewableGames.Count - num < pageSize)
				{
					count = viewableGames.Count - num;
				}
				_pagedGamesList.Add(viewableGames.GetRange(num, count));
			}
			LoadGameList();
		}

		private void NextPage()
		{
			_currentPageIndex++;
			if (_currentPageIndex >= _totalPages)
			{
				_currentPageIndex = 0;
			}
			LoadGameList();
		}

		private void PreviousPage()
		{
			_currentPageIndex--;
			if (_currentPageIndex < 0)
			{
				_currentPageIndex = _totalPages - 1;
			}
			LoadGameList();
		}

		private void LoadGameList()
		{
			_gameButtonList.ForEach(UnityEngine.Object.Destroy);
			_gameButtonList.Clear();
			foreach (GameSO game in _pagedGamesList[_currentPageIndex])
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(gameButtonPrefab, gameButtonView.transform);
				gameObject.name = game.GameName;
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<Image>().sprite = game.GameIcon;
				gameObject.GetComponentInChildren<TextMeshProUGUI>().text = game.GameName;
				GameObject gameObject2 = new GameObject(game.GameName);
				gameObject2.layer = LayerMask.NameToLayer("UI");
				gameObject2.transform.SetParent(gameObject.transform);
				Image image = gameObject2.AddComponent<Image>();
				gameObject.GetComponent<Button>().onClick.AddListener(delegate
				{
					ToggleGameDetection(game, image);
				});
				RectTransform component = gameObject2.GetComponent<RectTransform>();
				component.localScale = Vector3.one;
				component.anchorMin = Vector2.zero;
				component.anchorMax = Vector2.one;
				component.offsetMin = Vector2.zero;
				component.offsetMax = Vector2.zero;
				if (game.DetectionEnabled)
				{
					image.sprite = enabledSprite;
					image.color = enabledColor;
				}
				else
				{
					image.sprite = disabledSprite;
					image.color = disabledColor;
				}
				_gameButtonList.Add(gameObject);
				gameObject.SetActive(value: true);
			}
		}

		private void ToggleGameDetection(GameSO game, Image image)
		{
			game.DetectionEnabled = !game.DetectionEnabled;
			if (game.DetectionEnabled)
			{
				image.sprite = enabledSprite;
				image.color = enabledColor;
			}
			else
			{
				image.sprite = disabledSprite;
				image.color = disabledColor;
			}
		}
	}
}
