using System.Collections.Generic;
using System.Linq;
using Games.Models;
using Settings;
using UnityEngine;

namespace Games
{
	[CreateAssetMenu(menuName = "Games/Available Games")]
	public class ViewableGamesSO : ScriptableObject
	{
		private List<GameSO> _userAvailableGames = new List<GameSO>();

		private GameAccessSettings _userGameAccessSettings;

		private List<GameSO> _viewableGames = new List<GameSO>();

		public List<GameSO> UserAvailableGames
		{
			set
			{
				_userAvailableGames = value;
			}
		}

		private void OnEnable()
		{
			LoadSettings();
		}

		public void AddViewableGamesToSettings(Dictionary<int, bool> newGames)
		{
			foreach (KeyValuePair<int, bool> newGame in newGames)
			{
				if (_userGameAccessSettings.UserVisibleGameDictionary.ContainsKey(newGame.Key))
				{
					_userGameAccessSettings.UserVisibleGameDictionary[newGame.Key] = newGame.Value;
				}
				else
				{
					_userGameAccessSettings.UserVisibleGameDictionary.Add(newGame.Key, newGame.Value);
				}
			}
			_userGameAccessSettings.Save();
		}

		public List<GameSO> GetLicensedGames()
		{
			return _userAvailableGames.OrderBy((GameSO ag) => ag.GameId).ToList();
		}

		public Dictionary<int, bool> GetViewableGamesDictionary()
		{
			return _userGameAccessSettings.UserVisibleGameDictionary;
		}

		public List<GameSO> GetViewableGames()
		{
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			_viewableGames.Clear();
			foreach (GameSO userAvailableGame in _userAvailableGames)
			{
				if (!_userGameAccessSettings.UserVisibleGameDictionary.ContainsKey(userAvailableGame.GameId))
				{
					_viewableGames.Add(userAvailableGame);
					dictionary.Add(userAvailableGame.GameId, value: true);
				}
				else if (_userGameAccessSettings.UserVisibleGameDictionary[userAvailableGame.GameId])
				{
					_viewableGames.Add(userAvailableGame);
				}
			}
			_viewableGames = _viewableGames.OrderBy((GameSO ag) => ag.GameId).ToList();
			if (dictionary.Count > 0)
			{
				AddViewableGamesToSettings(dictionary);
			}
			return _viewableGames;
		}

		public void LoadSettings()
		{
			_userGameAccessSettings = SettingsStore.GameAccessVisibility;
		}
	}
}
