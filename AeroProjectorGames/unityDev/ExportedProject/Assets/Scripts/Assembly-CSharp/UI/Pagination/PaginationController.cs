using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pagination
{
	public class PaginationController : MonoBehaviour
	{
		private int _currentPageIndex;

		[SerializeField]
		private Button backPageButton;

		[SerializeField]
		private Button nextPageButton;

		[SerializeField]
		private List<GameObject> pages;

		private void OnDisable()
		{
			nextPageButton.onClick.RemoveListener(HandlePageNext);
			backPageButton.onClick.RemoveListener(HandlePageBack);
		}

		private void OnEnable()
		{
			SetPageIndex(_currentPageIndex);
			nextPageButton.onClick.AddListener(HandlePageNext);
			backPageButton.onClick.AddListener(HandlePageBack);
		}

		private void SetPageIndex(int newPageIndex)
		{
			_currentPageIndex = newPageIndex;
			for (int i = 0; i < pages.Count; i++)
			{
				pages[i].SetActive(i == newPageIndex);
			}
		}

		private void HandlePageNext()
		{
			if (_currentPageIndex >= pages.Count - 1)
			{
				SetPageIndex(0);
			}
			else
			{
				SetPageIndex(_currentPageIndex + 1);
			}
		}

		private void HandlePageBack()
		{
			if (_currentPageIndex <= 0)
			{
				SetPageIndex(pages.Count - 1);
			}
			else
			{
				SetPageIndex(_currentPageIndex - 1);
			}
		}
	}
}
