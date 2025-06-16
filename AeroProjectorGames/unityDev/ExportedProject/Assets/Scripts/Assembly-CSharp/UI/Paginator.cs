using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	public class Paginator<T> : IDisposable
	{
		private int _currentPageIndex;

		private bool _initialized;

		private List<List<T>> _pagedCollection;

		private int _pageSize;

		private int _totalPages;

		[Header("Pagination Buttons")]
		[SerializeField]
		private Button nextPageButton;

		[SerializeField]
		private Button previousPageButton;

		public UnityAction OnPagedChanged;

		public List<T> CurrentPage => _pagedCollection[_currentPageIndex];

		public int CurrentPageIndex => _currentPageIndex;

		public int PageSize => _pageSize;

		public int TotalPages => _totalPages;

		public Paginator(int pageSize, List<T> listItems)
		{
			int count = listItems.Count;
			_totalPages = (int)Math.Ceiling((float)count / (float)pageSize);
			_pagedCollection = new List<List<T>>();
			for (int i = 1; i <= _totalPages; i++)
			{
				int num = (i - 1) * pageSize;
				int count2 = pageSize;
				if (count - num < pageSize)
				{
					count2 = count - num;
				}
				_pagedCollection.Add(listItems.GetRange(num, count2));
			}
			_initialized = true;
		}

		public void Dispose()
		{
			_pagedCollection.Clear();
			_pagedCollection = null;
		}

		public void NextPage()
		{
			IsInitialized();
			_currentPageIndex++;
			if (_currentPageIndex >= _totalPages)
			{
				_currentPageIndex = 0;
			}
			OnPagedChanged?.Invoke();
		}

		public void PreviousPage()
		{
			IsInitialized();
			_currentPageIndex--;
			if (_currentPageIndex < 0)
			{
				_currentPageIndex = _totalPages - 1;
			}
			OnPagedChanged?.Invoke();
		}

		private void IsInitialized()
		{
			if (!_initialized)
			{
				throw new Exception("Paginator not initialized");
			}
		}
	}
}
