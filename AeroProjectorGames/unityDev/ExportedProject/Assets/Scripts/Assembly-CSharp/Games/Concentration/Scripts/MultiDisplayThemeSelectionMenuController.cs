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
	public class MultiDisplayThemeSelectionMenuController : MonoBehaviour
	{
		private GameObject _spacerObject;

		private int _themesCount;

		[Header("Theme Button Groups")]
		[SerializeField]
		private List<GameObject> buttons;

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
			buttons.ForEach(delegate(GameObject btn)
			{
				btn.GetComponent<Button>().onClick.RemoveAllListeners();
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
			GameObject original = buttons[0];
			for (int i = 0; i < amountToAdd; i++)
			{
				UnityEngine.Object.Instantiate(original, buttonsMainGroup.transform);
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
			int count = buttons.Count;
			if (count != _themesCount)
			{
				if (count > _themesCount)
				{
					RemoveButtons(count - _themesCount);
				}
				else
				{
					AddButtons(_themesCount - count);
				}
			}
		}

		private void RemoveButtons(int amountToRemove)
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				if (amountToRemove <= 0)
				{
					return;
				}
				UnityEngine.Object.DestroyImmediate(buttons[i]);
				amountToRemove--;
			}
			if (amountToRemove > 0)
			{
				StartGame(ConcentrationThemeNames.Planes);
			}
		}

		private void SetThemesToTheButtons()
		{
			int themesCount = _themesCount;
			for (int i = 0; i < themesCount; i++)
			{
				int themeIndex = i;
				buttons[i].GetComponent<Button>().onClick.AddListener(delegate
				{
					ThemeSelectionButtonClicked(themeIndex);
				});
				TMP_Text componentInChildren = buttons[i].gameObject.transform.GetComponentInChildren<TMP_Text>();
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
			yield return null;
			SetThemesToTheButtons();
			buttonsMainGroup.SetActive(value: true);
			headerLabelGroup.SetActive(value: true);
		}
	}
}
