using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackSolvingController : MonoBehaviour
	{
		private StringBuilder _maskedPhrase;

		private StringBuilder _currentPhrase;

		private List<int> _indexesToSolve;

		private int _selectedIndexToSolve;

		private bool _solving;

		[Header("Input Actions")]
		[SerializeField]
		private InputAction leftAction;

		[SerializeField]
		private InputAction rightAction;

		[Header("State")]
		[SerializeField]
		private WordWhackEventsSO events;

		[SerializeField]
		private GameEventsSO gameEvents;

		private int CurrentSolveIndex => _indexesToSolve[_selectedIndexToSolve];

		private char CurrentSolveCharacter
		{
			get
			{
				return _maskedPhrase[CurrentSolveIndex];
			}
			set
			{
				_maskedPhrase[CurrentSolveIndex] = value;
			}
		}

		private void OnDisable()
		{
			events.OnSolveAttemptStart -= StartSolveAttempt;
			events.OnSolveAttemptCancellationRequest -= HandleCancellationRequest;
			events.OnSolveAttemptCharacterInputForCurrentIndex -= HandleInput;
			events.OnSolveAttemptCompleteRequest -= CompleteRequest;
			Keyboard.current.onTextInput -= HandleInput;
			gameEvents.OnUndo -= HandleBackspace;
			rightAction.Disable();
			leftAction.Disable();
			rightAction.performed -= HandleRightArrow;
			leftAction.performed -= HandleLeftArrow;
		}

		private void OnEnable()
		{
			if (_indexesToSolve == null)
			{
				_indexesToSolve = new List<int>();
			}
			if (_indexesToSolve.Count > 0)
			{
				_indexesToSolve.Clear();
			}
			_maskedPhrase = _maskedPhrase?.Clear() ?? new StringBuilder();
			events.OnSolveAttemptStart += StartSolveAttempt;
			events.OnSolveAttemptCancellationRequest += HandleCancellationRequest;
			events.OnSolveAttemptCharacterInputForCurrentIndex += HandleInput;
			events.OnSolveAttemptCompleteRequest += CompleteRequest;
			rightAction.Enable();
			leftAction.Enable();
			rightAction.performed += HandleRightArrow;
			leftAction.performed += HandleLeftArrow;
		}

		private void CompleteRequest()
		{
			events.RaiseSolveAttemptCompleted(_maskedPhrase.ToString());
			Reset();
		}

		private void HandleBackspace()
		{
			CurrentSolveCharacter = '_';
			events.RaiseSolveAttemptCharacterInputChanged(CurrentSolveIndex, ' ');
			PreviousIndex();
			events.RaiseSolveCharacterIndexChanged(CurrentSolveIndex);
		}

		private void HandleCancellationRequest()
		{
			events.RaiseSolveAttemptCancelled();
			Reset();
		}

		private void HandleCharacterGuessForIndex(char character)
		{
			CurrentSolveCharacter = character;
			events.RaiseSolveAttemptCharacterInputChanged(CurrentSolveIndex, CurrentSolveCharacter);
			NextIndex();
		}

		private void HandleInput(char inputCharacter)
		{
			if (inputCharacter >= 'a')
			{
				if (inputCharacter <= 'z')
				{
					goto IL_0031;
				}
			}
			else if (inputCharacter >= 'A' && inputCharacter <= 'Z')
			{
				goto IL_0031;
			}
			bool flag = false;
			goto IL_0038;
			IL_0031:
			flag = true;
			goto IL_0038;
			IL_0038:
			if (flag)
			{
				HandleCharacterGuessForIndex(inputCharacter);
			}
			if (inputCharacter == '\b')
			{
				HandleBackspace();
			}
			if (inputCharacter == '\r')
			{
				CompleteRequest();
			}
			if (inputCharacter == '\u001b')
			{
				events.RaiseSolveAttemptCancellationRequest();
			}
		}

		private void HandleLeftArrow(InputAction.CallbackContext obj)
		{
			PreviousIndex();
		}

		private void HandleRightArrow(InputAction.CallbackContext obj)
		{
			NextIndex();
		}

		private void NextIndex()
		{
			if (_selectedIndexToSolve >= _indexesToSolve.Count - 1)
			{
				_selectedIndexToSolve = 0;
			}
			else
			{
				_selectedIndexToSolve++;
			}
			events.RaiseSolveCharacterIndexChanged(CurrentSolveIndex);
		}

		private void PreviousIndex()
		{
			if (_selectedIndexToSolve <= 0)
			{
				_selectedIndexToSolve = _indexesToSolve.Count - 1;
			}
			else
			{
				_selectedIndexToSolve--;
			}
			events.RaiseSolveCharacterIndexChanged(CurrentSolveIndex);
		}

		private void Reset()
		{
			_indexesToSolve.Clear();
			_maskedPhrase.Clear();
			_selectedIndexToSolve = 0;
			Keyboard.current.onTextInput -= HandleInput;
			gameEvents.OnUndo -= HandleBackspace;
			_solving = false;
		}

		private void StartSolveAttempt(string maskedPhrase)
		{
			Reset();
			_solving = true;
			_maskedPhrase.Append(maskedPhrase);
			for (int i = 0; i < maskedPhrase.Length; i++)
			{
				if (maskedPhrase[i] == '_')
				{
					_indexesToSolve.Add(i);
				}
			}
			events.RaiseSolveAttemptStarted();
			events.RaiseSolveCharacterIndexChanged(CurrentSolveIndex);
			Keyboard.current.onTextInput += HandleInput;
			gameEvents.OnUndo += HandleBackspace;
		}
	}
}
