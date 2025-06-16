using System;
using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using TMPro;
using UnityEngine;

namespace Games.Word_Whack.Scripts
{
	public class WordWhackScoreboardController : MonoBehaviour
	{
		private Coroutine _routine;

		private List<WordWhackPhraseController> _phraseControllers;

		[Header("UI Elements")]
		[SerializeField]
		private TMP_Text currentPlayer;

		[SerializeField]
		private GameObject phraseContainer;

		[Header("Animations")]
		[SerializeField]
		private List<ParticleSystem> failedSolveAnimations;

		[Header("Prefabs")]
		[SerializeField]
		private WordWhackPhraseController phrasePrefab;

		[Header("Events")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private WordWhackEventsSO wordWhackEvents;

		[Header("State")]
		[SerializeField]
		private GameStateSO gameState;

		private void OnDisable()
		{
			gameEvents.OnUpdateScoreboard -= UpdateScoreboard;
			gameEvents.OnUpdatePlayerTurn -= UpdateScoreboard;
			wordWhackEvents.OnPhraseChanged -= PhraseChanged;
			_phraseControllers.ForEach(delegate(WordWhackPhraseController w)
			{
				UnityEngine.Object.DestroyImmediate(w.gameObject);
			});
			_phraseControllers.Clear();
		}

		private void OnEnable()
		{
			if (_phraseControllers == null)
			{
				_phraseControllers = new List<WordWhackPhraseController>();
			}
			gameEvents.OnUpdateScoreboard += UpdateScoreboard;
			gameEvents.OnUpdatePlayerTurn += UpdateScoreboard;
			wordWhackEvents.OnPhraseChanged += PhraseChanged;
			wordWhackEvents.OnSolveAttemptFailed += PlayFailedSolveAnimation;
		}

		private void PlayFailedSolveAnimation()
		{
			failedSolveAnimations.ForEach(delegate(ParticleSystem a)
			{
				a.Play();
			});
		}

		private void PhraseChanged(string phrase)
		{
			_phraseControllers.ForEach(delegate(WordWhackPhraseController w)
			{
				UnityEngine.Object.DestroyImmediate(w.gameObject);
			});
			_phraseControllers.Clear();
			string[] array = phrase.Split(new char[1] { '/' }, StringSplitOptions.None);
			_phraseControllers.Add(UnityEngine.Object.Instantiate(phrasePrefab, phraseContainer.transform));
			int index = 0;
			int num = 0;
			bool flag = false;
			for (int num2 = 0; num2 < phrase.Length; num2++)
			{
				char c = phrase[num2];
				bool flag2 = c == ' ';
				if (flag2)
				{
					num++;
					flag = true;
				}
				string text = array[num];
				bool flag3 = false;
				if (flag && _phraseControllers[index].SpaceAvailable < text.Length)
				{
					_phraseControllers.Add(UnityEngine.Object.Instantiate(phrasePrefab, phraseContainer.transform));
					index = _phraseControllers.Count - 1;
					flag3 = true;
				}
				flag = false;
				if (!flag2 || !flag3)
				{
					_phraseControllers[index].AddCharacter(c, num2);
				}
			}
		}

		private void UpdateScoreboard()
		{
			if (!(currentPlayer.text == gameState.CurrentPlayer))
			{
				if (_routine != null)
				{
					StopCoroutine(_routine);
				}
				currentPlayer.transform.localPosition = Vector3.zero;
				_routine = StartCoroutine(UpdateScoreboardRoutine());
			}
		}

		private IEnumerator UpdateScoreboardRoutine()
		{
			StartCoroutine(LerpPlayerLabelPositionX(900f, -692f));
			yield return new WaitForSecondsRealtime(0.3f);
			currentPlayer.text = gameState.CurrentPlayer;
			StartCoroutine(LerpPlayerLabelPositionX(0f, 0f));
			yield return new WaitForSecondsRealtime(0.3f);
		}

		private IEnumerator LerpPlayerLabelPositionX(float toX, float endingX)
		{
			float elapsedTime = 0f;
			float duration = 0.2f;
			Vector3 startPosition = currentPlayer.transform.localPosition;
			Vector3 targetPosition = new Vector3(toX, startPosition.y, startPosition.z);
			while (elapsedTime < duration)
			{
				currentPlayer.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			currentPlayer.transform.localPosition = new Vector3(endingX, startPosition.y, startPosition.z);
		}
	}
}
