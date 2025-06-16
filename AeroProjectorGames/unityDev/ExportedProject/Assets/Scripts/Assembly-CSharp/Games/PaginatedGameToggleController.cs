using System;
using System.Collections.Generic;
using System.Linq;
using Games.Models;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Games
{
	public class PaginatedGameToggleController : MonoBehaviour
	{
		private List<GameToggleController> _gameToggles;

		private Paginator<GameToggleController> _paginator;

		[Header("Pagination Controls")]
		[SerializeField]
		private Button pageBackButton;

		[SerializeField]
		private Button pageForwardButton;

		[Header("UI Specific")]
		[SerializeField]
		private Button closeButton;

		[SerializeField]
		private GameObject gamePaginationPanel;

		[SerializeField]
		private GameToggleController gameToggleControllerBase;

		[Header("References")]
		[SerializeField]
		private ViewableGamesSO availableGames;

		[Header("Games To Exclude From Using Backgrounds")]
		[Tooltip("List of game SO's to exclude from using backgrounds")]
		[SerializeField]
		private List<GameSO> excludeGamesBySO = new List<GameSO>();

		public List<int> SelectedGameIds => (from gt in _gameToggles
			where gt.IsOn
			select gt.GameId).ToList();

		public event UnityAction<int, bool> OnGameToggleChanged;

		public event UnityAction OnCloseRequest;

		private void OnDisable()
		{
			if (_paginator != null)
			{
				Paginator<GameToggleController> paginator = _paginator;
				paginator.OnPagedChanged = (UnityAction)Delegate.Remove(paginator.OnPagedChanged, new UnityAction(PaginateGames));
			}
			List<GameToggleController> gameToggles = _gameToggles;
			if (gameToggles != null && gameToggles.Count > 0)
			{
				_gameToggles.ForEach(delegate(GameToggleController gt)
				{
					gt.OnToggleChanged -= RaiseGameToggleChanged;
					UnityEngine.Object.DestroyImmediate(gt.gameObject);
				});
				_gameToggles.Clear();
			}
			closeButton.onClick.RemoveListener(RaiseCloseRequest);
		}

		private void OnEnable()
		{
			closeButton.onClick.AddListener(RaiseCloseRequest);
		}

		private bool CheckForExcludedGames(GameSO selectedGameSO)
		{
			foreach (GameSO item in excludeGamesBySO)
			{
				if (selectedGameSO.GameName == item.GameName)
				{
					return true;
				}
			}
			return false;
		}

		public void Load(List<int> selectedGames)
		{
			_gameToggles = new List<GameToggleController>();
			foreach (GameSO viewableGame in availableGames.GetViewableGames())
			{
				if (!CheckForExcludedGames(viewableGame))
				{
					GameToggleController gameToggleController = UnityEngine.Object.Instantiate(gameToggleControllerBase, gamePaginationPanel.transform);
					gameToggleController.Setup(viewableGame.GameName, viewableGame.GameIcon, viewableGame.GameId, selectedGames.Contains(viewableGame.GameId));
					gameToggleController.OnToggleChanged += RaiseGameToggleChanged;
					_gameToggles.Add(gameToggleController);
				}
			}
			_paginator = new Paginator<GameToggleController>(3, _gameToggles);
			Paginator<GameToggleController> paginator = _paginator;
			paginator.OnPagedChanged = (UnityAction)Delegate.Combine(paginator.OnPagedChanged, new UnityAction(PaginateGames));
			pageBackButton.onClick.AddListener(_paginator.PreviousPage);
			pageForwardButton.onClick.AddListener(_paginator.NextPage);
			PaginateGames();
		}

		private void PaginateGames()
		{
			foreach (GameToggleController gameToggle in _gameToggles)
			{
				gameToggle.gameObject.SetActive(value: false);
			}
			_paginator.CurrentPage.ForEach(delegate(GameToggleController toggle)
			{
				toggle.gameObject.SetActive(value: true);
			});
		}

		private void RaiseGameToggleChanged(int gameId, bool selected)
		{
			this.OnGameToggleChanged?.Invoke(gameId, selected);
		}

		private void RaiseCloseRequest()
		{
			this.OnCloseRequest?.Invoke();
		}
	}
}
