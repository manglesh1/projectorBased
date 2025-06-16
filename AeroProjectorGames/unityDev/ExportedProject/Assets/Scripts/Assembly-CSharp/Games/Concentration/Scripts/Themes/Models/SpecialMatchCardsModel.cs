using System;
using UnityEngine;

namespace Games.Concentration.Scripts.Themes.Models
{
	[Serializable]
	public class SpecialMatchCardsModel
	{
		private const int STEAL_CARD_COUNT = 2;

		private const int WILD_CARD_COUNT_EASY_DIFFICULTY = 2;

		private const int WILD_CARD_COUNT_HARD_DIFFICULTY = 0;

		[SerializeField]
		[Tooltip("Card Face for steal card")]
		private Material stealCardMaterial;

		[SerializeField]
		[Tooltip("Card Face for wild card")]
		private Material wildCardMaterial;

		public int StealCardCount => 2;

		public Material StealCardMaterial => stealCardMaterial;

		public Material WildCardMaterial => wildCardMaterial;

		public int GetWildCardCount(GameDifficulties selectedGameDifficulty)
		{
			if (selectedGameDifficulty == GameDifficulties.Easy)
			{
				return 2;
			}
			return 0;
		}
	}
}
