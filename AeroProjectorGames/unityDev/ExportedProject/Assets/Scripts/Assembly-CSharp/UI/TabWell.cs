using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class TabWell : MonoBehaviour
	{
		private ColorBlock _activeColors;

		private ColorBlock _defaultColors;

		private int _selectedIndex;

		[Header("Tab Setup")]
		[SerializeField]
		private int defaultTabIndex;

		[SerializeField]
		private List<TabButton> tabs;

		[Space]
		[Header("Tab Button GUI")]
		[SerializeField]
		private Color activeTabColor = Color.blue;

		private void Awake()
		{
			_selectedIndex = defaultTabIndex;
			ToggleContentVisibility();
			tabs.ForEach(delegate(TabButton tab)
			{
				tab.ExistingButton.onClick.AddListener(delegate
				{
					HandleTabClick(tab.Index);
				});
			});
			_defaultColors = tabs[_selectedIndex].ExistingButton.colors;
			_activeColors = _defaultColors;
			_activeColors.normalColor = activeTabColor;
			_activeColors.selectedColor = activeTabColor;
			_activeColors.pressedColor = activeTabColor;
			SetActiveButtonHighlight();
		}

		private void HandleTabClick(int tabIndex)
		{
			RemoveCurrentHighlight();
			_selectedIndex = tabIndex;
			SetActiveButtonHighlight();
			ToggleContentVisibility();
		}

		private void RemoveCurrentHighlight()
		{
			tabs[_selectedIndex].ExistingButton.colors = _defaultColors;
		}

		private void SetActiveButtonHighlight()
		{
			tabs[_selectedIndex].ExistingButton.colors = _activeColors;
		}

		private void ToggleContentVisibility()
		{
			tabs.ForEach(delegate(TabButton tab)
			{
				bool isActive = tab.Index == _selectedIndex;
				tab.TabContents.ForEach(delegate(GameObject content)
				{
					if (!tabs[_selectedIndex].TabContents.Contains(content) || tab.Index == _selectedIndex)
					{
						content.SetActive(isActive);
					}
				});
			});
		}
	}
}
