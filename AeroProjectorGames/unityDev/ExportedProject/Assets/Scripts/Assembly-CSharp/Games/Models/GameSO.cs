using System.Collections.Generic;
using UnityEngine;

namespace Games.Models
{
	[CreateAssetMenu(menuName = "Games/Game")]
	public class GameSO : ScriptableObject
	{
		[SerializeField]
		private bool detectionEnabled;

		[SerializeField]
		private int gameId;

		[SerializeField]
		private string gameName;

		[SerializeField]
		private Sprite gameIcon;

		[SerializeField]
		private GameObject gamePrefab;

		[SerializeField]
		private bool weeklyFreeTrial;

		[Header("Sub-Game Menu Elements")]
		[SerializeField]
		private GameObject subGameSelectionPrefab;

		[SerializeField]
		private Dictionary<string, bool> gameDifficulty;

		public bool DetectionEnabled
		{
			get
			{
				return detectionEnabled;
			}
			set
			{
				detectionEnabled = value;
			}
		}

		public int GameId => gameId;

		public Sprite GameIcon => gameIcon;

		public string GameName => gameName;

		public GameObject GamePrefab => gamePrefab;

		public GameObject SubGameSelectionPrefab => subGameSelectionPrefab;

		public Dictionary<string, bool> GameDifficulty
		{
			get
			{
				return gameDifficulty;
			}
			set
			{
				gameDifficulty = value;
			}
		}
	}
}
