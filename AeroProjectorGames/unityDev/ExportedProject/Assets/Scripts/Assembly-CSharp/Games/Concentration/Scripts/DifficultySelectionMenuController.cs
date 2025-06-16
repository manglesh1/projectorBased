using Games.Concentration.SO;
using Games.Concentration.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Concentration.Scripts
{
	public class DifficultySelectionMenuController : MonoBehaviour
	{
		[Header("Menu Elements")]
		[SerializeField]
		private Toggle samePlayerAfterMatchToggle;

		[Header("Scriptable Objects")]
		[SerializeField]
		private ConcentrationGameEventsSO concentrationGameEvents;

		[SerializeField]
		private ConcentrationGameSettingsSO gameSettings;

		private void OnEnable()
		{
			samePlayerAfterMatchToggle.isOn = gameSettings.SamePlayerAfterMatch;
		}

		private void ShowThemeSelectionMenu(GameDifficulties selectedGameDifficulty)
		{
			gameSettings.GameDifficulty = selectedGameDifficulty;
			concentrationGameEvents.RaiseChangeGameState(ConcentrationGameStates.ThemeSelection);
		}

		public void EasyDifficultyButtonClicked()
		{
			ShowThemeSelectionMenu(GameDifficulties.Easy);
		}

		public void HardDifficultyButtonClicked()
		{
			ShowThemeSelectionMenu(GameDifficulties.Hard);
		}

		public void SamePlayerAfterMatchToggleValueChange()
		{
			gameSettings.SamePlayerAfterMatch = samePlayerAfterMatchToggle.isOn;
		}
	}
}
