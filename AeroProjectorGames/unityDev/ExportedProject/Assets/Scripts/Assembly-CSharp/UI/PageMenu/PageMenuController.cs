using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PageMenu
{
	public class PageMenuController : MonoBehaviour
	{
		[SerializeField]
		private List<PageMenuContent> pageMenuContent;

		private void OnDisable()
		{
			pageMenuContent.ForEach(delegate(PageMenuContent content)
			{
				content.CloseButton.onClick.RemoveListener(delegate
				{
					HandleCloseButtonClicked(content.CloseButton.GetInstanceID());
				});
			});
			pageMenuContent.ForEach(delegate(PageMenuContent content)
			{
				content.MenuButton.onClick.RemoveListener(delegate
				{
					HandleMenuButtonClicked(content.MenuButton.GetInstanceID());
				});
			});
		}

		private void OnEnable()
		{
			pageMenuContent.ForEach(delegate(PageMenuContent menu)
			{
				menu.MenuContent.ForEach(delegate(GameObject content)
				{
					content.SetActive(value: false);
				});
			});
			pageMenuContent.ForEach(delegate(PageMenuContent content)
			{
				if (content.MenuButton == null)
				{
					Debug.Log("Menu button not wired up in PageMenuController script");
				}
				else
				{
					content.MenuButton.onClick.AddListener(delegate
					{
						HandleMenuButtonClicked(content.MenuButton.GetInstanceID());
					});
				}
			});
			pageMenuContent.ForEach(delegate(PageMenuContent content)
			{
				if (content.CloseButton == null)
				{
					Debug.Log("Close button not wired up in PageMenuController script");
				}
				else
				{
					content.CloseButton.onClick.AddListener(delegate
					{
						HandleCloseButtonClicked(content.CloseButton.GetInstanceID());
					});
				}
			});
		}

		private void HandleCloseButtonClicked(int clickedButtonInstanceId)
		{
			pageMenuContent.Where(delegate(PageMenuContent menu)
			{
				Button closeButton = menu.CloseButton;
				return closeButton != null && closeButton.GetInstanceID() == clickedButtonInstanceId;
			});
			pageMenuContent.Where((PageMenuContent menu) => menu.CloseButton != null && menu.CloseButton.GetInstanceID() == clickedButtonInstanceId).ToList().ForEach(delegate(PageMenuContent menu)
			{
				menu.MenuContent.ForEach(delegate(GameObject content)
				{
					content.SetActive(value: false);
				});
			});
		}

		private void HandleMenuButtonClicked(int clickedButtonInstanceId)
		{
			pageMenuContent.Where((PageMenuContent menu) => menu.MenuButton.GetInstanceID() != clickedButtonInstanceId).ToList().ForEach(delegate(PageMenuContent menu)
			{
				menu.MenuContent.ForEach(delegate(GameObject content)
				{
					content.SetActive(value: false);
				});
			});
			pageMenuContent.Where((PageMenuContent menu) => menu.MenuButton.GetInstanceID() == clickedButtonInstanceId).ToList().ForEach(delegate(PageMenuContent menu)
			{
				menu.MenuContent.ForEach(delegate(GameObject content)
				{
					content.SetActive(value: true);
				});
			});
		}
	}
}
