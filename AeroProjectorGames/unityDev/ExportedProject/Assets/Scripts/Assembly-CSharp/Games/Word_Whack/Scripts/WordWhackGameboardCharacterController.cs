using System;
using Games.GameState;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackGameboardCharacterController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		private WordWhackCharacterStateEnum _state;

		private bool _solving;

		[SerializeField]
		private Image backgroundImage;

		[SerializeField]
		private Sprite foundBackgroundSprite;

		[SerializeField]
		private Sprite hiddenBackgroundSprite;

		[SerializeField]
		private Sprite notFoundBackgroundSprite;

		[SerializeField]
		private TMP_Text characterText;

		[FormerlySerializedAs("events")]
		[Header("State")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private WordWhackEventsSO wordWhackEvents;

		private void OnEnable()
		{
			backgroundImage.sprite = hiddenBackgroundSprite;
			_state = WordWhackCharacterStateEnum.Hidden;
			wordWhackEvents.OnCharacterFound += MarkCharacterFound;
			wordWhackEvents.OnCharacterNotFound += MarkCharacterNotFound;
			wordWhackEvents.OnResetCharacter += ResetCharacter;
			wordWhackEvents.OnPhraseSolved += TurnOffSolveState;
			wordWhackEvents.OnSolveAttemptStarted += TurnOnSolveState;
			wordWhackEvents.OnSolveAttemptCancelled += TurnOffSolveState;
			gameEvents.OnNewGame += Reset;
		}

		private void OnDisable()
		{
			wordWhackEvents.OnCharacterFound -= MarkCharacterFound;
			wordWhackEvents.OnCharacterNotFound -= MarkCharacterNotFound;
			wordWhackEvents.OnResetCharacter -= ResetCharacter;
			wordWhackEvents.OnPhraseSolved -= TurnOffSolveState;
			wordWhackEvents.OnSolveAttemptStarted -= TurnOnSolveState;
			wordWhackEvents.OnSolveAttemptCancelled -= TurnOffSolveState;
			gameEvents.OnNewGame -= Reset;
		}

		private void TurnOffSolveState()
		{
			_solving = false;
			if (_state == WordWhackCharacterStateEnum.Found)
			{
				backgroundImage.sprite = foundBackgroundSprite;
				backgroundImage.color = Color.white;
				characterText.color = Color.white;
			}
			else if (_state == WordWhackCharacterStateEnum.NotFound)
			{
				backgroundImage.sprite = notFoundBackgroundSprite;
				backgroundImage.color = Color.red;
				characterText.color = Color.white;
			}
		}

		private void TurnOnSolveState()
		{
			_solving = true;
			if (_state == WordWhackCharacterStateEnum.Found || _state == WordWhackCharacterStateEnum.NotFound)
			{
				backgroundImage.color = Color.clear;
				characterText.color = Color.clear;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				if (_solving && _state == WordWhackCharacterStateEnum.Hidden)
				{
					wordWhackEvents.RaiseSolveAttemptCharacterInputForCurrentIndex(characterText.text[0]);
				}
			}
			else if (_state == WordWhackCharacterStateEnum.Found)
			{
				wordWhackEvents.RaiseFoundAnimation();
			}
			else
			{
				backgroundImage.sprite = foundBackgroundSprite;
				wordWhackEvents.RaiseCharacterHit(characterText.text[0]);
			}
		}

		private void MarkCharacterFound(char character)
		{
			if (characterText.text.IndexOf(character.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				_state = WordWhackCharacterStateEnum.Found;
				backgroundImage.sprite = foundBackgroundSprite;
				backgroundImage.color = Color.white;
			}
		}

		private void MarkCharacterNotFound(char character)
		{
			if (characterText.text.IndexOf(character.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				_state = WordWhackCharacterStateEnum.NotFound;
				backgroundImage.sprite = notFoundBackgroundSprite;
				backgroundImage.color = Color.red;
			}
		}

		private void ResetCharacter(char character)
		{
			if (characterText.text.IndexOf(character.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				Reset();
			}
		}

		private void Reset()
		{
			_state = WordWhackCharacterStateEnum.Hidden;
			_solving = false;
			backgroundImage.sprite = hiddenBackgroundSprite;
			backgroundImage.color = Color.white;
			characterText.color = Color.white;
		}
	}
}
