using System.Collections.Generic;
using Extensions;
using Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Players
{
	public class TeamNameController : MonoBehaviour
	{
		private const int TEAMS_PER_PAGE = 3;

		private int _currentPage;

		private PlayerNameSettings _playerNameSettings;

		private List<string> _unusedTeamNames = new List<string>();

		private List<string> _displayedTeamNames = new List<string>();

		[Space]
		[Header("Player data")]
		[SerializeField]
		private PlayerStateSO playerState;

		public int CurrentPage
		{
			get
			{
				return _currentPage;
			}
			set
			{
				_currentPage = value;
			}
		}

		public List<string> UnusedTeamNames
		{
			get
			{
				return _unusedTeamNames;
			}
			set
			{
				_unusedTeamNames = value;
			}
		}

		private void AddNameToDisplayedTeamNamesList(string chkName)
		{
			_displayedTeamNames.Add(chkName);
			_displayedTeamNames.Sort();
		}

		private void AddNewNameToList(string chkName)
		{
			_unusedTeamNames.Add(chkName);
			_unusedTeamNames.Sort();
		}

		public void AddNameBackToDisplayNamesList(string chkName)
		{
			if (_unusedTeamNames.Contains(chkName) && !_displayedTeamNames.Contains(chkName))
			{
				AddNameToDisplayedTeamNamesList(chkName);
			}
		}

		public void AddCustomNameToUnusedNamesList(string chkName)
		{
			GetPlayerNameSettings();
			if (!_unusedTeamNames.Contains(chkName))
			{
				AddNewNameToList(chkName);
				SavePlayerNameSettings();
			}
		}

		public void AddNameToUnusedNamesList(string chkName)
		{
			GetPlayerNameSettings();
			if (playerState.AvailableTeams.Contains(chkName) && !_unusedTeamNames.Contains(chkName))
			{
				AddNewNameToList(chkName);
				SavePlayerNameSettings();
			}
		}

		private void GetPlayerNameSettings()
		{
			_playerNameSettings = SettingsStore.PlayerNames;
		}

		public void PopulateTeamSelectionPage(List<TextMeshProUGUI> teamNameSelectionList, int pageAdvance = 0)
		{
			int num = _currentPage + pageAdvance;
			int num2 = _displayedTeamNames.Count / teamNameSelectionList.Count;
			float num3 = (float)_displayedTeamNames.Count / 3f;
			if (num < 0 || num > num2)
			{
				return;
			}
			_currentPage += pageAdvance;
			if ((float)_currentPage >= num3)
			{
				_currentPage--;
			}
			for (int i = 0; i < 3; i++)
			{
				int num4 = 3 * _currentPage;
				if (i + num4 < _displayedTeamNames.Count)
				{
					teamNameSelectionList[i].transform.parent.GetComponent<Image>().enabled = true;
					teamNameSelectionList[i].transform.parent.GetComponent<Button>().enabled = true;
					teamNameSelectionList[i].gameObject.SetActive(value: true);
					teamNameSelectionList[i].text = _displayedTeamNames[i + num4];
				}
				else
				{
					teamNameSelectionList[i].transform.parent.GetComponent<Image>().enabled = false;
					teamNameSelectionList[i].transform.parent.GetComponent<Button>().enabled = false;
					teamNameSelectionList[i].gameObject.SetActive(value: false);
				}
			}
		}

		public void RemoveNameFromDisplayedNamesList(string chkName)
		{
			if (_displayedTeamNames.Contains(chkName))
			{
				_displayedTeamNames.Remove(chkName);
			}
		}

		public void RemoveNameFromUnusedNamesList(string chkName)
		{
			GetPlayerNameSettings();
			if (_unusedTeamNames.Contains(chkName))
			{
				_unusedTeamNames.Remove(chkName);
				SavePlayerNameSettings();
			}
		}

		private void SavePlayerNameSettings()
		{
			_playerNameSettings.CustomPlayerNames = _unusedTeamNames.SimpleJsonClone();
			_playerNameSettings.Save();
		}

		public void SetUnusedTeamNames()
		{
			_currentPage = 0;
			_unusedTeamNames.Clear();
			_unusedTeamNames = new List<string>(playerState.AvailableTeams);
			_unusedTeamNames.Sort();
			_displayedTeamNames = _unusedTeamNames.SimpleJsonClone();
		}
	}
}
