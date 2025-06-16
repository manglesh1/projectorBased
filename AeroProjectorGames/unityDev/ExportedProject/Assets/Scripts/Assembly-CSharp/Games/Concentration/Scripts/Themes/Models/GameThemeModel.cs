using System;
using System.Collections.Generic;
using Games.Concentration.Scripts.Themes.Enums;
using UnityEngine;

namespace Games.Concentration.Scripts.Themes.Models
{
	[Serializable]
	public class GameThemeModel
	{
		[SerializeField]
		[Tooltip("Name shown to the players when selecting a theme")]
		private ConcentrationThemeNames themeName;

		[SerializeField]
		[Tooltip("Image seen across the back of all the cards")]
		private Material cardBackMaterial;

		[SerializeField]
		[Tooltip("Card fronts the player will match")]
		private StandardMatchCardsModel standardCardFrontMaterials;

		[SerializeField]
		[Tooltip("Special match card fronts of the special cards, ex. wild, bonus, etc.")]
		private SpecialMatchCardsModel specialCardFrontMaterials;

		public Material CardBackMaterial => cardBackMaterial;

		public int StandardCardMatchCount => standardCardFrontMaterials.StandardTokenCardCount;

		public int StealCardMatchCount => specialCardFrontMaterials.StealCardCount;

		public Material StealCardMaterial => specialCardFrontMaterials.StealCardMaterial;

		public Material WildCardMaterial => specialCardFrontMaterials.WildCardMaterial;

		public List<Material> GetCardFrontMaterials(GameDifficulties selectedGameDifficulty)
		{
			return standardCardFrontMaterials.GetTokenMaterials(selectedGameDifficulty);
		}

		public int GetWildCardCount(GameDifficulties selectedGameDifficulty)
		{
			return specialCardFrontMaterials.GetWildCardCount(selectedGameDifficulty);
		}
	}
}
