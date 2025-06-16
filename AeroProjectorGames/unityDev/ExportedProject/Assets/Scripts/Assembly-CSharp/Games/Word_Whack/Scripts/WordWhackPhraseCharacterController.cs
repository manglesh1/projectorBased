using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackPhraseCharacterController : MonoBehaviour
	{
		private const int FACE_TEXTURE_INDEX = 1;

		private int _characterIndex;

		private Coroutine _routine;

		[Header("Solve Attempt")]
		[SerializeField]
		private Texture2D highlightTexture;

		[SerializeField]
		private Texture2D selectedTexture;

		[SerializeField]
		private Texture2D normalTexture;

		[SerializeField]
		private TMP_Text solveInput;

		[Header("Token Renderer")]
		[SerializeField]
		private MeshRenderer tokenRenderer;

		[Header("Phrase Character Components")]
		[SerializeField]
		private Transform characterTile;

		[SerializeField]
		private TMP_Text characterInput;

		[Header("Effects")]
		[SerializeField]
		private ParticleSystem hitEffect;

		[Header("State")]
		[SerializeField]
		private WordWhackEventsSO events;

		private bool Revealed => characterTile.rotation.y != 0f;

		private void OnDisable()
		{
			events.OnCharacterFound -= HandleCharacterFound;
			events.OnResetCharacter -= HandleCharacterReset;
			events.OnPhraseSolved -= HandlePhraseSolved;
			events.OnSolveAttemptCancelled -= HandleSolveAttemptStopped;
			events.OnPhraseSolved -= HandleSolveAttemptStopped;
			events.OnSolveAttemptStarted -= HandleSolveAttemptStarted;
			events.OnSolveAttemptCharacterInputChanged -= HandleSolveAttemptCharacterInputChanged;
			events.OnSolveAttemptCharacterIndexChanged -= HandleSolveIndexChanged;
		}

		private void OnEnable()
		{
			characterInput.text = string.Empty;
			hitEffect.gameObject.SetActive(value: false);
			events.OnCharacterFound += HandleCharacterFound;
			events.OnResetCharacter += HandleCharacterReset;
			events.OnPhraseSolved += HandlePhraseSolved;
			events.OnSolveAttemptCancelled += HandleSolveAttemptStopped;
			events.OnPhraseSolved += HandleSolveAttemptStopped;
			events.OnSolveAttemptStarted += HandleSolveAttemptStarted;
			events.OnSolveAttemptCharacterInputChanged += HandleSolveAttemptCharacterInputChanged;
			events.OnSolveAttemptCharacterIndexChanged += HandleSolveIndexChanged;
		}

		private void HandleCharacterFound(char character)
		{
			if (!Revealed && characterInput.text.IndexOf(character.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				StopRoutine();
				_routine = StartCoroutine(RevealLetter());
			}
		}

		private void HandleCharacterReset(char character)
		{
			if (Revealed && characterInput.text.IndexOf(character.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				StopRoutine();
				_routine = StartCoroutine(HideLetter());
			}
		}

		private void HandlePhraseSolved()
		{
			if (!Revealed)
			{
				StopRoutine();
				_routine = StartCoroutine(RevealLetter());
			}
		}

		private void HandleSolveAttemptStarted()
		{
			if (!Revealed)
			{
				tokenRenderer.materials[1].mainTexture = highlightTexture;
			}
		}

		private void HandleSolveAttemptStopped()
		{
			tokenRenderer.materials[1].mainTexture = normalTexture;
			solveInput.text = string.Empty;
		}

		private void HandleSolveAttemptCharacterInputChanged(int characterIndex, char character)
		{
			if (_characterIndex == characterIndex)
			{
				solveInput.text = character.ToString().ToUpper();
			}
		}

		private void HandleSolveIndexChanged(int index)
		{
			if (_characterIndex != index)
			{
				tokenRenderer.materials[1].mainTexture = highlightTexture;
			}
			else
			{
				tokenRenderer.materials[1].mainTexture = selectedTexture;
			}
		}

		public void SetCharacter(char character, int index)
		{
			_characterIndex = index;
			characterInput.text = character.ToString();
			if (character == ' ')
			{
				characterInput.gameObject.SetActive(value: false);
				characterTile.gameObject.SetActive(value: false);
			}
			if (character == '\'')
			{
				characterTile.localRotation = new Quaternion(0f, 180f, 0f, 0f);
			}
		}

		private void StopRoutine()
		{
			if (_routine != null)
			{
				StopCoroutine(_routine);
			}
		}

		private IEnumerator HideLetter()
		{
			int duration = UnityEngine.Random.Range(1, 5);
			Quaternion startRotation = characterTile.rotation;
			Quaternion endRotation = Quaternion.Euler(0f, 0f, 0f);
			float modifier = 0f;
			for (float t = 0f; t < 1f; t += Time.deltaTime / (float)duration)
			{
				characterTile.rotation = Quaternion.Lerp(startRotation, endRotation, t * modifier);
				modifier += 0.03f;
				yield return null;
			}
			characterTile.rotation = endRotation;
		}

		private IEnumerator RevealLetter()
		{
			hitEffect.gameObject.SetActive(value: true);
			int duration = UnityEngine.Random.Range(1, 5);
			Quaternion startRotation = characterTile.rotation;
			int num = UnityEngine.Random.Range(-10, 10);
			Quaternion endRotation = startRotation * Quaternion.Euler(0f, (num > 0) ? (-180f) : 180f, 0f);
			float modifier = 0f;
			for (float t = 0f; t < 1f; t += Time.deltaTime / (float)duration)
			{
				characterTile.rotation = Quaternion.Lerp(startRotation, endRotation, t * modifier);
				modifier += 0.03f;
				yield return null;
			}
			characterTile.rotation = endRotation;
		}
	}
}
