using System.Collections.Generic;
using System.Linq;
using Settings;
using UnityEngine;

namespace Players
{
	[CreateAssetMenu(menuName = "Players/Player State")]
	public class PlayerStateSO : ScriptableObject
	{
		private PlayerNameSettings _playerNameSettings;

		[SerializeField]
		private List<string> defaultPlayerNameOptions = new List<string>
		{
			"Player 1", "Player 2", "Axe to Grind", "Nice Axe", "Pain in the Axe", "Axeperts", "Kiss my Axe", "Axe-ciddent", "My Axe Wife", "My Axe Husband",
			"Axe Scent", "Re Laxe", "Total Hacks", "Un Axe Ceptable", "Axe Hole"
		};

		public readonly int maxPlayers = 6;

		public List<PlayerData> players = new List<PlayerData>();

		public List<string> AvailableTeams
		{
			get
			{
				_playerNameSettings = SettingsStore.PlayerNames;
				if (_playerNameSettings.CustomPlayerNames.Count > 0)
				{
					return _playerNameSettings.CustomPlayerNames.Where((string p) => !players.Exists((PlayerData player) => player.PlayerName == p)).ToList();
				}
				return defaultPlayerNameOptions.Where((string p) => !players.Exists((PlayerData player) => player.PlayerName == p)).ToList();
			}
		}

		public List<string> CurrentPlayerNames => players.Select((PlayerData p) => p.PlayerName).ToList();

		private void Awake()
		{
			Reset();
		}

		public void AddPlayer(PlayerData player)
		{
			players.Add(player);
		}

		public void RemovePlayer(string playerName)
		{
			players.Remove(players.Find((PlayerData p) => p.PlayerName == playerName));
		}

		public void Reset()
		{
			players.Clear();
			players.AddRange(new List<PlayerData>
			{
				new PlayerData("Player 1"),
				new PlayerData("Player 2")
			});
		}

		public void UpdatePlayer(string oldName, string newName)
		{
			players.First((PlayerData p) => p.PlayerName == oldName).PlayerName = newName;
		}
	}
}
