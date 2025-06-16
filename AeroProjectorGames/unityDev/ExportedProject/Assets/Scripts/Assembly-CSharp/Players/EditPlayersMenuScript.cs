using System.Collections.Generic;
using Games;
using Settings;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Players
{
	public class EditPlayersMenuScript : MonoBehaviour
	{
		private const string PLUS_SIGN = "+";

		private Button _currentTeamNameButtonSelected;

		private TextMeshProUGUI _currentTeamNameButtonTextSelected;

		private readonly List<GameObject> _currentTeamsBeingDisplayedLIST = new List<GameObject>();

		private readonly List<string> _usedNamesLIST = new List<string>();

		private bool _isAdding;

		private bool _isEditing;

		[SerializeField]
		private GameObject throwsPerTurnView;

		[SerializeField]
		private GameObject playerSetupView;

		[SerializeField]
		private GameObject resetTeamsButton;

		[SerializeField]
		private GameObject currentTeamNameButtonPrefab;

		[SerializeField]
		private Transform currentTeamsGroupTextField;

		[SerializeField]
		private GameObject currentTeamsParent;

		[SerializeField]
		private GameObject addEditTeamParent;

		[SerializeField]
		private TMP_InputField customTeamNameInputField;

		[SerializeField]
		private List<TextMeshProUGUI> teamNameSelectionList;

		[SerializeField]
		private GameObject removeTeamButtonObject;

		[SerializeField]
		private TeamNameController teamNameController;

		[Space]
		[Header("Team Button Default Color")]
		[SerializeField]
		private ColorBlock teamNameButtonDefaultColorBlock;

		[Space]
		[Header("Team Button Selected Color")]
		[SerializeField]
		private ColorBlock teamNameButtonSelectedColorBlock;

		[Space]
		[Header("Player data")]
		[SerializeField]
		private PlayerStateSO playerState;

		[Space]
		[Header("UI Manager")]
		[SerializeField]
		private UIManager uiManager;

		[Space]
		[Header("Game Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnEnable()
		{
			DisplayCurrentTeams();
			teamNameController.CurrentPage = 0;
			ToggleEditing(value: false);
			if (SettingsStore.Target.ThrowsPerTurnEnabled)
			{
				ShowThrowsPerTurnMenu();
			}
			else
			{
				ShowTeamSetup();
			}
			gameEvents.OnThrowsPerTurnUpdated += ShowTeamSetup;
		}

		private void OnDisable()
		{
			gameEvents.OnThrowsPerTurnUpdated -= ShowTeamSetup;
		}

		private void AddTeam(string newTeam = "+")
		{
			if (_isEditing)
			{
				EditPlayer();
				ToggleEditing(value: false);
			}
			else if (NameIsInUse(newTeam))
			{
				ToggleEditing(value: false);
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
				ToggleEditing(value: false);
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
			string text = customTeamNameInputField.text.Trim();
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
			ToggleEditing(value: false);
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
			ToggleEditing(value: false);
		}

		public void SaveTeam()
		{
			if (!(customTeamNameInputField.text.Trim() == string.Empty))
			{
				AddTeam(customTeamNameInputField.text.Trim());
				if (_isAdding)
				{
					_isEditing = false;
					playerState.AddPlayer(new PlayerData(customTeamNameInputField.text.Trim()));
					AddTeam();
				}
				_currentTeamNameButtonSelected.colors = teamNameButtonDefaultColorBlock;
			}
			else if (_currentTeamNameButtonTextSelected.text.Trim() == string.Empty)
			{
				_currentTeamNameButtonTextSelected.text = "+";
				_currentTeamNameButtonSelected.colors = teamNameButtonDefaultColorBlock;
				ToggleEditing(value: false);
			}
		}

		public void CancelEditing()
		{
			if (customTeamNameInputField.text.Trim() == string.Empty)
			{
				_currentTeamNameButtonTextSelected.text = "+";
			}
			_currentTeamNameButtonSelected.colors = teamNameButtonDefaultColorBlock;
			ToggleEditing(value: false);
		}

		public void ScrollTeamSelectionPage(int pageAdvance)
		{
			teamNameController.PopulateTeamSelectionPage(teamNameSelectionList, pageAdvance);
		}

		public void SetNameToInputField(TextMeshProUGUI selectedName)
		{
			customTeamNameInputField.text = selectedName.text;
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
				customTeamNameInputField.text = playerName.text;
				_isAdding = false;
				removeTeamButtonObject.SetActive(value: true);
			}
			else
			{
				playerName.text = string.Empty;
				customTeamNameInputField.text = string.Empty;
				_isAdding = true;
				removeTeamButtonObject.SetActive(value: false);
			}
			_isEditing = true;
			ToggleEditing(value: true);
			customTeamNameInputField.Select();
		}

		private void ToggleEditing(bool value)
		{
			if (value)
			{
				currentTeamsParent.SetActive(value: false);
				addEditTeamParent.SetActive(value: true);
			}
			else
			{
				currentTeamsParent.SetActive(value: true);
				addEditTeamParent.SetActive(value: false);
			}
		}
	}
}
