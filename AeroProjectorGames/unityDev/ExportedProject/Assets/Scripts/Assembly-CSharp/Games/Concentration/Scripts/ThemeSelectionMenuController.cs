using System;
using System.Collections;
using System.Collections.Generic;
using Games.Concentration.SO;
using Games.Concentration.Scripts.Enums;
using Games.Concentration.Scripts.Themes;
using Games.Concentration.Scripts.Themes.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Concentration.Scripts
{
	public class ThemeSelectionMenuController : MonoBehaviour
	{
		private GameObject _spacerObject;

		private int _themesCount;

		private List<Button> _themeMenuButtons = new List<Button>();

		[Header("Theme Button Groups")]
		[SerializeField]
		private GameObject leftColumnButtonsGroup;

		[SerializeField]
		private GameObject rightColumnButtonsGroup;

		[Header("Theme Menu Main Groups")]
		[SerializeField]
		private GameObject buttonsMainGroup;

		[SerializeField]
		private GameObject headerLabelGroup;

		[Header("Scriptable Objects")]
		[SerializeField]
		private ConcentrationGameEventsSO concentrationGameEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private ConcentrationGameSettingsSO gameSettings;

		[Header("External References")]
		[SerializeField]
		private ThemeManager themeManager;

		private void OnDisable()
		{
			_themeMenuButtons.ForEach(delegate(Button btn)
			{
				btn.onClick.RemoveAllListeners();
			});
		}

		private void OnEnable()
		{
			buttonsMainGroup.SetActive(value: false);
			headerLabelGroup.SetActive(value: false);
			if (CheckIfThemeMenuShouldBeSkipped())
			{
				StartCoroutine(HandleThemeButtonsSetUp());
			}
		}

		private void AddButtons(int amountToAdd)
		{
			GameObject original = leftColumnButtonsGroup.transform.GetChild(0).gameObject;
			for (int i = 0; i < amountToAdd; i++)
			{
				UnityEngine.Object.Instantiate(original, leftColumnButtonsGroup.transform);
			}
		}

		private void AddSpacerObjectIfNeeded()
		{
			int childCount = leftColumnButtonsGroup.transform.childCount;
			int childCount2 = rightColumnButtonsGroup.transform.childCount;
			if (childCount != childCount2)
			{
				if (_spacerObject != null)
				{
					UnityEngine.Object.DestroyImmediate(_spacerObject);
				}
				_spacerObject = UnityEngine.Object.Instantiate(new GameObject(), rightColumnButtonsGroup.transform);
				_spacerObject.AddComponent<RectTransform>();
			}
		}

		private void BalanceColumnCount()
		{
			int childCount = leftColumnButtonsGroup.transform.childCount;
			int childCount2 = rightColumnButtonsGroup.transform.childCount;
			if (childCount == childCount2 || childCount == childCount2 + 1)
			{
				return;
			}
			if (childCount > childCount2)
			{
				float num = childCount - childCount2;
				num = Mathf.FloorToInt(num * 0.5f);
				for (int i = 0; (float)i < num; i++)
				{
					leftColumnButtonsGroup.transform.GetChild(0).transform.SetParent(rightColumnButtonsGroup.transform);
				}
			}
			else
			{
				float num2 = childCount2 - childCount;
				num2 = Mathf.CeilToInt(num2 * 0.5f);
				for (int j = 0; (float)j < num2; j++)
				{
					rightColumnButtonsGroup.transform.GetChild(0).transform.SetParent(leftColumnButtonsGroup.transform);
				}
			}
		}

		public bool CheckIfThemeMenuShouldBeSkipped()
		{
			_themesCount = Enum.GetValues(typeof(ConcentrationThemeNames)).Length;
			if (_themesCount < 2)
			{
				StartGame(ConcentrationThemeNames.Planes);
				return false;
			}
			return true;
		}

		private void CompareButtonAndThemeCount()
		{
			int childCount = leftColumnButtonsGroup.transform.childCount;
			int childCount2 = rightColumnButtonsGroup.transform.childCount;
			int num = childCount + childCount2;
			if (num != _themesCount)
			{
				if (num > _themesCount)
				{
					RemoveButtons(num - _themesCount);
				}
				else
				{
					AddButtons(_themesCount - num);
				}
			}
		}

		private void CreateListOfMenuButtons()
		{
			Button[] componentsInChildren = leftColumnButtonsGroup.transform.GetComponentsInChildren<Button>();
			Button[] componentsInChildren2 = rightColumnButtonsGroup.transform.GetComponentsInChildren<Button>();
			_themeMenuButtons = new List<Button>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				_themeMenuButtons.Add(componentsInChildren[i]);
				if (i < componentsInChildren2.Length)
				{
					_themeMenuButtons.Add(componentsInChildren2[i]);
				}
			}
		}

		private void RemoveButtons(int amountToRemove)
		{
			int childCount = rightColumnButtonsGroup.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (amountToRemove <= 0)
				{
					return;
				}
				UnityEngine.Object.DestroyImmediate(rightColumnButtonsGroup.transform.GetChild(0).gameObject);
				amountToRemove--;
			}
			int childCount2 = leftColumnButtonsGroup.transform.childCount;
			for (int j = 0; j < childCount2; j++)
			{
				if (amountToRemove <= 0)
				{
					return;
				}
				UnityEngine.Object.DestroyImmediate(leftColumnButtonsGroup.transform.GetChild(0).gameObject);
				amountToRemove--;
			}
			if (amountToRemove > 0)
			{
				StartGame(ConcentrationThemeNames.Planes);
			}
		}

		private void SetThemesToTheButtons()
		{
			int count = _themeMenuButtons.Count;
			for (int i = 0; i < count; i++)
			{
				int themeIndex = i;
				_themeMenuButtons[i].onClick.AddListener(delegate
				{
					ThemeSelectionButtonClicked(themeIndex);
				});
				TMP_Text componentInChildren = _themeMenuButtons[i].gameObject.transform.GetComponentInChildren<TMP_Text>();
				ConcentrationThemeNames concentrationThemeNames = (ConcentrationThemeNames)i;
				componentInChildren.text = concentrationThemeNames.ToString();
			}
		}

		private void StartGame(ConcentrationThemeNames selectedGameTheme)
		{
			themeManager.SetGameTheme((int)selectedGameTheme);
			concentrationGameEvents.RaiseChangeGameState(ConcentrationGameStates.Playing);
			gameEvents.RaiseRemoveObjectFromGameFlexSpace(base.gameObject);
		}

		public void ThemeSelectionButtonClicked(int selectedGameThemeIndex)
		{
			StartGame((ConcentrationThemeNames)selectedGameThemeIndex);
		}

		private IEnumerator HandleThemeButtonsSetUp()
		{
			CompareButtonAndThemeCount();
			BalanceColumnCount();
			yield return null;
			AddSpacerObjectIfNeeded();
			CreateListOfMenuButtons();
			SetThemesToTheButtons();
			buttonsMainGroup.SetActive(value: true);
			headerLabelGroup.SetActive(value: true);
		}
	}
}
