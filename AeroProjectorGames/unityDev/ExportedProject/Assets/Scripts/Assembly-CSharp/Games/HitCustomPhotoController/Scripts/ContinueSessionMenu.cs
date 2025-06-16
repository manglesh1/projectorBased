using System.Collections;
using Games.HitCustomPhotoController.ScriptableObjects;
using UnityEngine;

namespace Games.HitCustomPhotoController.Scripts
{
	public class ContinueSessionMenu : MonoBehaviour
	{
		private const float SHOW_PREPAIR_GAME_TEXT_TIME_IN_SECONDS = 1.5f;

		private IEnumerator _prepareGameSessionCoroutine;

		private bool _multiDisplay;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private HitCustomPhotoEventsSO hitCustomPhotoEvents;

		[SerializeField]
		private HitCustomPhotoGameSessionSO hitCustomPhotoGameSession;

		[SerializeField]
		private GameObject preparingGameText;

		[SerializeField]
		private GameObject useExistingImagesButtonGroup;

		private void OnEnable()
		{
			preparingGameText.SetActive(value: true);
			useExistingImagesButtonGroup.SetActive(value: false);
			_prepareGameSessionCoroutine = PrepareGameSession();
			StartCoroutine(_prepareGameSessionCoroutine);
		}

		private void OnDisable()
		{
			if (_prepareGameSessionCoroutine != null)
			{
				StopCoroutine(_prepareGameSessionCoroutine);
			}
		}

		public void ContinueCurrentGameSession()
		{
			hitCustomPhotoEvents.RaiseChangeGameState(HitCustomPhotoGameStates.SpeedSelection);
			RemoveFromGameFlexSpace();
		}

		public void StartNewGameSession()
		{
			hitCustomPhotoEvents.RaiseEndGameSession();
			hitCustomPhotoEvents.RaiseChangeGameState(HitCustomPhotoGameStates.WaitingOnUserImages);
			RemoveFromGameFlexSpace();
		}

		private IEnumerator PrepareGameSession()
		{
			yield return new WaitForSeconds(1.5f);
			if (!hitCustomPhotoGameSession.CheckCurrentGameSession())
			{
				StartNewGameSession();
				yield break;
			}
			preparingGameText.SetActive(value: false);
			useExistingImagesButtonGroup.SetActive(value: true);
		}

		private void RemoveFromGameFlexSpace()
		{
			if (!_multiDisplay)
			{
				gameEvents.RaiseRemoveObjectFromGameFlexSpace(base.gameObject);
			}
		}

		public void SetMultiDisplay()
		{
			_multiDisplay = true;
		}
	}
}
