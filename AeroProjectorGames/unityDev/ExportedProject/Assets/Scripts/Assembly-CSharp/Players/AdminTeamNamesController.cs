using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Players
{
	public class AdminTeamNamesController : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField customTeamNameInputField;

		[SerializeField]
		private TeamNameController teamNameController;

		[SerializeField]
		private List<TextMeshProUGUI> teamNameSelectionList;

		private void OnEnable()
		{
			teamNameController.SetUnusedTeamNames();
			PopulateTeamNames();
			StartCoroutine(WaitForObjectRender());
		}

		public IEnumerator WaitForObjectRender()
		{
			yield return null;
			customTeamNameInputField.Select();
		}

		private void PopulateTeamNames()
		{
			customTeamNameInputField.text = string.Empty;
			teamNameController.SetUnusedTeamNames();
			teamNameController.PopulateTeamSelectionPage(teamNameSelectionList);
		}

		public void AddPlayerName()
		{
			if (!string.IsNullOrEmpty(customTeamNameInputField.text.Trim()))
			{
				teamNameController.AddCustomNameToUnusedNamesList(customTeamNameInputField.text.Trim());
				PopulateTeamNames();
			}
		}

		public void RemovePlayerName()
		{
			if (!string.IsNullOrEmpty(customTeamNameInputField.text.Trim()))
			{
				teamNameController.RemoveNameFromUnusedNamesList(customTeamNameInputField.text.Trim());
				PopulateTeamNames();
			}
		}

		public void SetNameToInputField(TextMeshProUGUI selectedName)
		{
			customTeamNameInputField.text = selectedName.text;
		}

		public void ScrollTeamSelectionPage(int pageAdvance)
		{
			teamNameController.PopulateTeamSelectionPage(teamNameSelectionList, pageAdvance);
		}
	}
}
