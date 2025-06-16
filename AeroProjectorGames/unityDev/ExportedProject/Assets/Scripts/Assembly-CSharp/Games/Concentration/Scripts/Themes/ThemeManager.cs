using System.Collections.Generic;
using Games.Concentration.Scripts.Themes.Models;
using UnityEngine;

namespace Games.Concentration.Scripts.Themes
{
	public class ThemeManager : MonoBehaviour
	{
		private const int DEFAULT_FALLBACK_THEME_INDEX = 0;

		private const int TOKEN_MATCH_SCORE_VALUE = 1;

		private int _selectedGameThemeIndex;

		private GameThemeModel _selectedGameThemeModel;

		[Header("Game Themes")]
		[SerializeField]
		private List<GameThemeModel> gameboardThemesList = new List<GameThemeModel>();

		public Material SelectedCardBackMatrial => _selectedGameThemeModel.CardBackMaterial;

		public Material SelectedStealCardMaterial => _selectedGameThemeModel.StealCardMaterial;

		public Material SelectedWildCardMaterial => _selectedGameThemeModel.WildCardMaterial;

		public int StandardCardCount => _selectedGameThemeModel.StandardCardMatchCount;

		public int StealCardCount => _selectedGameThemeModel.StealCardMatchCount;

		public int TokenMatchScoreValue => 1;

		public List<Material> GetStandardCardFrontMaterials(GameDifficulties selectGameDifficulty)
		{
			return _selectedGameThemeModel.GetCardFrontMaterials(selectGameDifficulty);
		}

		public int GetWildCardCount(GameDifficulties selectGameDifficulty)
		{
			return _selectedGameThemeModel.GetWildCardCount(selectGameDifficulty);
		}

		public void SetGameTheme(int selectedGameThemeIndex)
		{
			_selectedGameThemeIndex = selectedGameThemeIndex;
			try
			{
				_selectedGameThemeModel = gameboardThemesList[_selectedGameThemeIndex];
			}
			catch
			{
				_selectedGameThemeModel = gameboardThemesList[0];
			}
		}
	}
}
