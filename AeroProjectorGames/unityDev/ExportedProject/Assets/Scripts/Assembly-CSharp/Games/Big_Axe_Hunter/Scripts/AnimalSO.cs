using UnityEngine;

namespace Games.Big_Axe_Hunter.Scripts
{
	[CreateAssetMenu(menuName = "Games/BigAxeHunter/Animal")]
	public class AnimalSO : ScriptableObject
	{
		public GameObject AnimalPrefab { get; set; }

		public Sprite AnimalThumbnail { get; set; }

		public int ScoreValue { get; set; }
	}
}
