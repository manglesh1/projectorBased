using UnityEngine;
using UnityEngine.Events;

namespace Games.Word_Whack.Scripts
{
	[CreateAssetMenu(menuName = "Games/WordWhack/Word Whack Events")]
	public class WordWhackEventsSO : ScriptableObject
	{
		public event UnityAction<char> OnCharacterHit;

		public event UnityAction<char> OnCharacterFound;

		public event UnityAction<char> OnCharacterNotFound;

		public event UnityAction OnFoundAnimation;

		public event UnityAction OnNotFoundAnimation;

		public event UnityAction<string> OnPhraseChanged;

		public event UnityAction OnPhraseSolved;

		public event UnityAction<char> OnResetCharacter;

		public event UnityAction OnSolveAttemptCancelled;

		public event UnityAction<int> OnSolveAttemptCharacterIndexChanged;

		public event UnityAction<int, char> OnSolveAttemptCharacterInputChanged;

		public event UnityAction<char> OnSolveAttemptCharacterInputForCurrentIndex;

		public event UnityAction OnSolveAttemptCancellationRequest;

		public event UnityAction<string> OnSolveAttemptCompleted;

		public event UnityAction OnSolveAttemptCompleteRequest;

		public event UnityAction OnSolveAttemptFailed;

		public event UnityAction OnSolveAttemptRequest;

		public event UnityAction<string> OnSolveAttemptStart;

		public event UnityAction OnSolveAttemptStarted;

		public event UnityAction OnStopGameboardAnimationsRequest;

		public void RaiseCharacterHit(char character)
		{
			this.OnCharacterHit?.Invoke(character);
		}

		public void RaiseOnCharacterNotFound(char character)
		{
			this.OnCharacterNotFound?.Invoke(character);
		}

		public void RaiseOnCharacterFound(char character)
		{
			this.OnCharacterFound?.Invoke(character);
		}

		public void RaiseFoundAnimation()
		{
			this.OnFoundAnimation?.Invoke();
		}

		public void RaiseNotFoundAnimation()
		{
			this.OnNotFoundAnimation?.Invoke();
		}

		public void RaisePhraseChanged(string newPhrase)
		{
			this.OnPhraseChanged?.Invoke(newPhrase);
		}

		public void RaisePhraseSolved()
		{
			this.OnPhraseSolved?.Invoke();
		}

		public void RaiseResetCharacter(char character)
		{
			this.OnResetCharacter?.Invoke(character);
		}

		public void RaiseSolveAttemptCancelled()
		{
			this.OnSolveAttemptCancelled?.Invoke();
		}

		public void RaiseSolveAttemptCancellationRequest()
		{
			this.OnSolveAttemptCancellationRequest?.Invoke();
		}

		public void RaiseSolveAttemptCompleted(string answer)
		{
			this.OnSolveAttemptCompleted?.Invoke(answer);
		}

		public void RaiseSolveAttemptCompleteRequest()
		{
			this.OnSolveAttemptCompleteRequest?.Invoke();
		}

		public void RaiseSolveAttemptFailed()
		{
			this.OnSolveAttemptFailed?.Invoke();
		}

		public void RaiseSolveAttemptStart(string maskedPhrase)
		{
			this.OnSolveAttemptStart?.Invoke(maskedPhrase);
		}

		public void RaiseSolveAttemptStarted()
		{
			this.OnSolveAttemptStarted?.Invoke();
		}

		public void RaiseSolveAttemptStartRequest()
		{
			this.OnSolveAttemptRequest?.Invoke();
		}

		public void RaiseSolveCharacterIndexChanged(int index)
		{
			this.OnSolveAttemptCharacterIndexChanged?.Invoke(index);
		}

		public void RaiseSolveAttemptCharacterInputChanged(int index, char character)
		{
			this.OnSolveAttemptCharacterInputChanged?.Invoke(index, character);
		}

		public void RaiseSolveAttemptCharacterInputForCurrentIndex(char character)
		{
			this.OnSolveAttemptCharacterInputForCurrentIndex?.Invoke(character);
		}

		public void RaiseStopGameboardAnimationsRequest()
		{
			this.OnStopGameboardAnimationsRequest?.Invoke();
		}
	}
}
