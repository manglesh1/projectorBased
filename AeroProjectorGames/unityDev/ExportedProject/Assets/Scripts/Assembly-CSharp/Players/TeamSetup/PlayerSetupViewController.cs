using System.Collections.Generic;
using Games;
using Settings;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using VirtualKeyboard;

namespace Players.TeamSetup
{
	public class PlayerSetupViewController : MonoBehaviour
	{
		private const string PLUS_SIGN = "+";

		private Button _currentTeamNameButtonSelected;

		private TextMeshProUGUI _currentTeamNameButtonTextSelected;

		private readonly List<GameObject> _currentTeamsBeingDisplayedLIST = new List<GameObject>();

		private readonly List<string> _usedNamesLIST = new List<string>();

		private bool _isAdding;

		private bool _isEditing;

		[Header("Throws Per Turn View")]
		[SerializeField]
		private GameObject throwsPerTurnView;

		[Header("Player List View")]
		[SerializeField]
		private GameObject currentTeamNameButtonPrefab;

		[SerializeField]
		private GameObject currentTeamsParent;

		[SerializeField]
		private Transform currentTeamsGroupTextField;

		[SerializeField]
		private GameObject playerSetupView;

		[SerializeField]
		private GameObject resetTeamsButton;

		[Header("Edit Player View")]
		[SerializeField]
		private GameObject addEditTeamParent;

		[SerializeField]
		private GameObject pickTeamFromListParent;

		[SerializeField]
		private GameObject removeTeamButtonObject;

		[SerializeField]
		private TMP_Text teamNameLabel;

		[SerializeField]
		private List<TextMeshProUGUI> teamNameSelectionList;

		[Header("External References")]
		[SerializeField]
		private TeamNameController teamNameController;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private UIManager uiManager;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private VirtualKeyboardEventsSO virtualKeyboardEvents;

		[Space]
		[Header("Team Button Default Color")]
		[SerializeField]
		private ColorBlock teamNameButtonDefaultColorBlock;

		[Space]
		[Header("Team Button Selected Color")]
		[SerializeField]
		private ColorBlock teamNameButtonSelectedColorBlock;

		private void OnEnable()
		{
			DisplayCurrentTeams();
			teamNameController.CurrentPage = 0;
			ToggleEditing(PlayerEditViewEnum.Off);
			if (SettingsStore.Target.ThrowsPerTurnEnabled)
			{
				ShowThrowsPerTurnMenu();
			}
			else
			{
				ShowTeamSetup();
			}
			gameEvents.OnThrowsPerTurnUpdated += ShowTeamSetup;
			virtualKeyboardEvents.OnCompletedEntry += HandleVirtualKeyboardCompleted;
		}

		private void OnDisable()
		{
			gameEvents.OnThrowsPerTurnUpdated -= ShowTeamSetup;
			virtualKeyboardEvents.OnCompletedEntry -= HandleVirtualKeyboardCompleted;
		}

		private void AddTeam(string newTeam = "+")
		{
			if (_isEditing)
			{
				EditPlayer();
				ToggleEditing(PlayerEditViewEnum.Off);
			}
			else if (NameIsInUse(newTeam))
			{
				ToggleEditing(PlayerEditViewEnum.Off);
			}
			else if (_usedNamesLIST.Count < playerState.maxPlayers)
			{
				teamNameController.RemoveNameFromDisplayedNamesList(newTeam);
				GameObject button = Object.Instantiate(currentTeamNameButtonPrefab, currentTeamsGroupTextField);
				button.GetComponentInChildren<TextMeshProUGUI>().text = newTeam;
				button.GetComponent<Button>().onClick.AddListener(delegate
				{
					ShowTeamOptions(button.GetComponentInChildren<TextMeshProUGUI>());
				});
				_currentTeamsBeingDisplayedLIST.Add(button);
				if (newTeam != "+")
				{
					_usedNamesLIST.Add(newTeam);
				}
				ToggleEditing(PlayerEditViewEnum.Off);
			}
		}

		private void ClearAllTeams()
		{
			foreach (GameObject item in _currentTeamsBeingDisplayedLIST)
			{
				Object.Destroy(item);
			}
			_currentTeamsBeingDisplayedLIST.Clear();
			_usedNamesLIST.Clear();
		}

		private void DisplayCurrentTeams()
		{
			try
			{
				_isEditing = false;
				teamNameController.SetUnusedTeamNames();
				ClearAllTeams();
				foreach (PlayerData player in playerState.players)
				{
					AddTeam(player.PlayerName);
				}
				AddTeam();
			}
			catch
			{
			}
		}

		private void EditPlayer()
		{
			string text = teamNameLabel.text.Trim();
			string text2 = _currentTeamNameButtonTextSelected.text.Trim();
			if (!(text2 == text) && !NameIsInUse(text))
			{
				if (text2 != string.Empty)
				{
					int index = _usedNamesLIST.IndexOf(text2);
					playerState.UpdatePlayer(text2, text);
					_usedNamesLIST[index] = text;
				}
				teamNameController.RemoveNameFromDisplayedNamesList(text);
				teamNameController.AddNameBackToDisplayNamesList(text2);
				if (text != "+" && text2 == string.Empty)
				{
					_usedNamesLIST.Add(text);
				}
				_currentTeamNameButtonSelected.GetComponentInChildren<TextMeshProUGUI>().text = text;
			}
		}

		public void EnterCustomName()
		{
			virtualKeyboardEvents.RaiseVirtualKeyboardEntryRequest();
		}

		private void HandleVirtualKeyboardCompleted(string text)
		{
			if (text.Trim().Length != 0)
			{
				teamNameLabel.text = text;
			}
			ToggleEditing(PlayerEditViewEnum.DisplayTeam);
		}

		private string GetNextDefaultPlayerName()
		{
			return $"Player {playerState.players.Count + 1}";
		}

		private bool NameIsInUse(string chkName)
		{
			return _usedNamesLIST.Contains(chkName);
		}

		private void ShowTeamSetup()
		{
			resetTeamsButton.SetActive(value: true);
			throwsPerTurnView.SetActive(value: false);
			playerSetupView.SetActive(value: true);
		}

		private void ShowThrowsPerTurnMenu()
		{
			resetTeamsButton.SetActive(value: false);
			throwsPerTurnView.SetActive(value: true);
			playerSetupView.SetActive(value: false);
		}

		public void DoneButton()
		{
			uiManager.LoadDefaultView();
		}

		public void RemoveTeam()
		{
			ToggleEditing(PlayerEditViewEnum.Off);
			if (_usedNamesLIST.Count == 1)
			{
				return;
			}
			string text = _currentTeamNameButtonTextSelected.text.Trim();
			if (text != string.Empty)
			{
				_usedNamesLIST.Remove(text);
				playerState.RemovePlayer(text);
			}
			teamNameController.SetUnusedTeamNames();
			for (int i = 0; i < _currentTeamsBeingDisplayedLIST.Count; i++)
			{
				if (_currentTeamsBeingDisplayedLIST[i].GetComponentInChildren<TextMeshProUGUI>().text == text)
				{
					Object.Destroy(_currentTeamsBeingDisplayedLIST[i]);
					_currentTeamsBeingDisplayedLIST.RemoveAt(i);
				}
			}
			int index = _currentTeamsBeingDisplayedLIST.Count - 1;
			if (_currentTeamsBeingDisplayedLIST[index].GetComponentInChildren<TextMeshProUGUI>().text != "+")
			{
				_isEditing = false;
				AddTeam();
			}
		}

		public void ResetTeams()
		{
			playerState.Reset();
			DisplayCurrentTeams();
			ToggleEditing(PlayerEditViewEnum.Off);
		}

		public void SaveTeam()
		{
			AddTeam(teamNameLabel.text.Trim());
			_currentTeamNameButtonSelected.colors = teamNameButtonDefaultColorBlock;
			if (_isAdding)
			{
				_isEditing = false;
				playerState.AddPlayer(new PlayerData(teamNameLabel.text.Trim()));
				AddTeam();
				ToggleEditing(PlayerEditViewEnum.Off);
			}
		}

		public void CancelEditing()
		{
			if (_isAdding)
			{
				_currentTeamNameButtonTextSelected.text = "+";
				_isAdding = false;
			}
			_currentTeamNameButtonSelected.colors = teamNameButtonDefaultColorBlock;
			ToggleEditing(PlayerEditViewEnum.Off);
		}

		public void ScrollTeamSelectionPage(int pageAdvance)
		{
			teamNameController.PopulateTeamSelectionPage(teamNameSelectionList, pageAdvance);
		}

		public void SetNameToInputField(TextMeshProUGUI selectedName)
		{
			teamNameLabel.text = selectedName.text;
			teamNameLabel.text = selectedName.text;
			ToggleEditing(PlayerEditViewEnum.DisplayTeam);
		}

		public void ShowTeamOptions(TextMeshProUGUI playerName)
		{
			if (_currentTeamNameButtonTextSelected != null && _currentTeamNameButtonTextSelected.text.Trim() == string.Empty)
			{
				_currentTeamNameButtonTextSelected.text = "+";
			}
			if (_currentTeamNameButtonSelected != null)
			{
				_currentTeamNameButtonSelected.colors = teamNameButtonDefaultColorBlock;
			}
			_currentTeamNameButtonTextSelected = playerName;
			_currentTeamNameButtonSelected = playerName.transform.parent.GetComponent<Button>();
			_currentTeamNameButtonSelected.colors = teamNameButtonSelectedColorBlock;
			teamNameController.PopulateTeamSelectionPage(teamNameSelectionList);
			if (playerName.text != "+")
			{
				teamNameLabel.text = playerName.text;
				_isAdding = false;
				removeTeamButtonObject.SetActive(value: true);
			}
			else
			{
				playerName.text = string.Empty;
				teamNameLabel.text = GetNextDefaultPlayerName();
				_isAdding = true;
				removeTeamButtonObject.SetActive(value: false);
			}
			_isEditing = true;
			ToggleEditing(PlayerEditViewEnum.DisplayTeam);
		}

		private void ToggleEditing(PlayerEditViewEnum view)
		{
			switch (view)
			{
			case PlayerEditViewEnum.DisplayTeam:
				currentTeamsParent.SetActive(value: false);
				pickTeamFromListParent.SetActive(value: false);
				addEditTeamParent.SetActive(value: true);
				break;
			case PlayerEditViewEnum.Off:
				addEditTeamParent.SetActive(value: false);
				currentTeamsParent.SetActive(value: true);
				break;
			case PlayerEditViewEnum.PickFromList:
				currentTeamsParent.SetActive(value: false);
				addEditTeamParent.SetActive(value: true);
				pickTeamFromListParent.SetActive(value: true);
				break;
			default:
				currentTeamsParent.SetActive(value: false);
				addEditTeamParent.SetActive(value: true);
				break;
			}
		}

		public void TogglePickTeamFromList()
		{
			ToggleEditing(PlayerEditViewEnum.PickFromList);
		}
	}
}
