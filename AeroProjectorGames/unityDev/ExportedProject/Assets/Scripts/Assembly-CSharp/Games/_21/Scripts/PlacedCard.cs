using System;
using UnityEngine;

namespace Games._21.Scripts
{
	[Serializable]
	public class PlacedCard
	{
		public GameObject GameboardCard { get; private set; }

		public GameObject ScoringCard { get; private set; }

		public PlacedCard(GameObject gameboardCard, GameObject scoringCard)
		{
			GameboardCard = gameboardCard;
			ScoringCard = scoringCard;
		}
	}
}
