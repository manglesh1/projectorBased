using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Concentration.Scripts.Themes.Models
{
	[Serializable]
	public class StandardMatchCardsModel
	{
		private const int STANDARD_TOKEN_CARD_COUNT = 2;

		[SerializeField]
		private Material standardTokenCard1;

		[SerializeField]
		private Material standardTokenCard2;

		[SerializeField]
		private Material standardTokenCard3;

		[SerializeField]
		private Material standardTokenCard4;

		[SerializeField]
		private Material standardTokenCard5;

		[SerializeField]
		private Material standardTokenCard6;

		public int StandardTokenCardCount => 2;

		public List<Material> GetTokenMaterials(GameDifficulties selectedGameDifficulty)
		{
			List<Material> list = new List<Material>();
			list.Add(standardTokenCard1);
			list.Add(standardTokenCard2);
			list.Add(standardTokenCard3);
			list.Add(standardTokenCard4);
			list.Add(standardTokenCard5);
			list.Add(standardTokenCard6);
			if (selectedGameDifficulty == GameDifficulties.Easy)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				list.RemoveAt(index);
			}
			return list;
		}
	}
}
