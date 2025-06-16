using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackSolveButtonsController : MonoBehaviour
	{
		[Header("UI Elements")]
		[SerializeField]
		private Button cancelButton;

		[SerializeField]
		private Button okButton;

		[SerializeField]
		private Button solveButton;

		[SerializeField]
		[OptionalField]
		private Button backspaceButton;

		[Header("Events")]
		[SerializeField]
		private WordWhackEventsSO wordWhackEvents;

		[SerializeField]
		private GameEventsSO gameEvents;

		private void OnDisable()
		{
			wordWhackEvents.OnSolveAttemptStarted -= ShowSolveAttemptButtons;
			wordWhackEvents.OnSolveAttemptCompleted -= HandleSolveAttemptCompleted;
			wordWhackEvents.OnSolveAttemptCancelled -= HideSolveAttemptButtons;
			wordWhackEvents.OnPhraseSolved -= HideSolveAttemptButtons;
			solveButton.onClick.RemoveListener(StartSolveAttempt);
			okButton.onClick.RemoveListener(CompleteSolveAttempt);
			cancelButton.onClick.RemoveListener(CancelSolveAttempt);
			if (backspaceButton != null)
			{
				backspaceButton.onClick.RemoveListener(gameEvents.RaiseUndo);
			}
		}

		private void OnEnable()
		{
			HideSolveAttemptButtons();
			wordWhackEvents.OnSolveAttemptStarted += ShowSolveAttemptButtons;
			wordWhackEvents.OnSolveAttemptCompleted += HandleSolveAttemptCompleted;
			wordWhackEvents.OnSolveAttemptCancelled += HideSolveAttemptButtons;
			wordWhackEvents.OnPhraseSolved += HideSolveAttemptButtons;
			solveButton.onClick.AddListener(StartSolveAttempt);
			okButton.onClick.AddListener(CompleteSolveAttempt);
			cancelButton.onClick.AddListener(CancelSolveAttempt);
			if (backspaceButton != null)
			{
				backspaceButton.onClick.AddListener(gameEvents.RaiseUndo);
			}
		}

		private void CancelSolveAttempt()
		{
			wordWhackEvents.RaiseSolveAttemptCancellationRequest();
		}

		private void CompleteSolveAttempt()
		{
			wordWhackEvents.RaiseSolveAttemptCompleteRequest();
		}

		private void HandleSolveAttemptCompleted(string _)
		{
			HideSolveAttemptButtons();
		}

		private void HideSolveAttemptButtons()
		{
			HideBackspace();
			solveButton.gameObject.SetActive(value: true);
			okButton.gameObject.SetActive(value: false);
			cancelButton.gameObject.SetActive(value: false);
			EventSystem.current.SetSelectedGameObject(null);
		}

		private void HideBackspace()
		{
			if (backspaceButton != null)
			{
				backspaceButton.gameObject.SetActive(value: false);
			}
		}

		private void ShowBackspace()
		{
			if (backspaceButton != null)
			{
				backspaceButton.gameObject.SetActive(value: true);
			}
		}

		private void ShowSolveAttemptButtons()
		{
			ShowBackspace();
			solveButton.gameObject.SetActive(value: false);
			okButton.gameObject.SetActive(value: true);
			cancelButton.gameObject.SetActive(value: true);
			EventSystem.current.SetSelectedGameObject(null);
		}

		private void StartSolveAttempt()
		{
			wordWhackEvents.RaiseSolveAttemptStartRequest();
			ShowSolveAttemptButtons();
		}
	}
}
