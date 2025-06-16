using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(VerticalLayoutGroup))]
	public class VerticalTabController : MonoBehaviour
	{
		private ColorBlock _activeColors;

		private ColorBlock _defaultColors;

		private int _selectedIndex;

		private Dictionary<int, GameObject> _tabButtonMap;

		[Header("Tab Setup")]
		[SerializeField]
		private int defaultTabIndex;

		[SerializeField]
		private List<TabButton> tabs;

		[Space]
		[Header("Tab Button GUI")]
		[SerializeField]
		private Color activeTabColor = Color.blue;

		[SerializeField]
		private Button tabButtonPrefab;

		private void Awake()
		{
			_selectedIndex = defaultTabIndex;
			_tabButtonMap = new Dictionary<int, GameObject>();
			ToggleContentVisibility();
			tabs.ForEach(delegate(TabButton tab)
			{
				if (!tab.WindowsOnly || Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
				{
					Button button = Object.Instantiate(tabButtonPrefab, base.transform);
					button.name = tab.Name;
					button.GetComponentInChildren<TMP_Text>().text = tab.Name;
					button.onClick.AddListener(delegate
					{
						HandleTabClick(tab.Index);
					});
					_tabButtonMap.Add(tab.Index, button.gameObject);
				}
			});
			_defaultColors = _tabButtonMap[_selectedIndex].GetComponent<Button>().colors;
			_activeColors = _defaultColors;
			_activeColors.normalColor = activeTabColor;
			_activeColors.selectedColor = activeTabColor;
			_activeColors.pressedColor = activeTabColor;
			SetActiveButtonHighlight();
		}

		private void OnDestroy()
		{
			foreach (KeyValuePair<int, GameObject> item in _tabButtonMap)
			{
				Object.DestroyImmediate(item.Value);
			}
			_tabButtonMap.Clear();
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
			_tabButtonMap[_selectedIndex].GetComponent<Button>().colors = _defaultColors;
		}

		private void SetActiveButtonHighlight()
		{
			_tabButtonMap[_selectedIndex].GetComponent<Button>().colors = _activeColors;
		}

		private void ToggleContentVisibility()
		{
			tabs.ForEach(delegate(TabButton tab)
			{
				bool isActive = tab.Index == _selectedIndex;
				tab.TabContents.ForEach(delegate(GameObject content)
				{
					content.SetActive(isActive);
				});
			});
		}
	}
}
