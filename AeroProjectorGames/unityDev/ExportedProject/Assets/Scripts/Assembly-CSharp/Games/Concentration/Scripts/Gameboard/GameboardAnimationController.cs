using System.Collections;
using System.Collections.Generic;
using Games.Concentration.SO;
using Games.Concentration.Scripts.Tokens;
using UnityEngine;

namespace Games.Concentration.Scripts.Gameboard
{
	public class GameboardAnimationController : MonoBehaviour
	{
		private const string FLIP_CARD_FACE_DOWN_TRIGGER = "FlipFaceDown";

		private const string FLIP_CARD_FACE_UP_TRIGGER = "FlipFaceUp";

		private const float WAIT_BEFORE_CHECKING_CLIP_LENGTH = 0.5f;

		private IEnumerator _flipAutoSelectedCardEnumerator;

		private IEnumerator _flipAutoSelectedCardFromFaceDownToFaceUpEnumerator;

		private IEnumerator _flipCardFromFaceDownToFaceUpEnumerator;

		private IEnumerator _flipFailedMatchingGameTokensEnumerator;

		private IEnumerator _flipPlayerSelectedCardEnumerator;

		private IEnumerator _flipPlayerSelectedCardFromFaceDownToFaceUpEnumerator;

		private IEnumerator _playFailedMatchAnimationEnumerator;

		private IEnumerator _playHitAutoSelctedtokenAnimationEnumerator;

		private IEnumerator _playMatchTwoTokensAnimationEnumerator;

		private IEnumerator _playMatchWithWildAnimationEnumerator;

		private IEnumerator _playUndoFlipFaceDownToFaceUpAnimationEnumerator;

		[Header("Scriptable Objects")]
		[SerializeField]
		private ConcentrationGameEventsSO concentrationGameEvents;

		[Header("External References")]
		[SerializeField]
		private GameboardController gameboardController;

		private void OnEnable()
		{
			AtEnableAddListeners();
		}

		private void OnDisable()
		{
			AtDisableRemoveListeners();
			AtDisableStopAllCoroutines();
		}

		private void AtEnableAddListeners()
		{
			concentrationGameEvents.OnFlipAutoSelectedGameToken += HandleFlippingAutoSelectedToken;
			concentrationGameEvents.OnFlipPlayerSelectedGameToken += HandleFlippingPlayerSelectedToken;
			concentrationGameEvents.OnPlayFailedMatchAnimation += HandlePlayFailedMatchAnimation;
			concentrationGameEvents.OnPlayHitAutoSelctedtokenAnimation += HandlePlayHitAutoSelctedtokenAnimation;
			concentrationGameEvents.OnPlayMatchTwoTokensAnimation += HandlePlayMatchTwoTokensAnimation;
			concentrationGameEvents.OnPlayMatchWithWildAnimation += HandlePlayMatchWithWildAnimation;
			concentrationGameEvents.OnPlayMissAnimation += HandlePlayMissAnimation;
			concentrationGameEvents.OnPlayStealTokenAnimation += HandlePlayStealTokenAnimation;
			concentrationGameEvents.OnPlayStealTokenWithWildAnimation += HandlePlayStealTokenWithWildAnimation;
			concentrationGameEvents.OnPlayUndoFaceDownToFaceUpAnimation += HandlePlayUndoFlipFaceDownToFaceUpAnimation;
			concentrationGameEvents.OnPlayUndoFaceUpToFaceDownAnimation += HandlePlayUndoFlipFaceUpToFaceDownAnimation;
		}

		private void AtDisableRemoveListeners()
		{
			concentrationGameEvents.OnFlipAutoSelectedGameToken -= HandleFlippingAutoSelectedToken;
			concentrationGameEvents.OnFlipPlayerSelectedGameToken -= HandleFlippingPlayerSelectedToken;
			concentrationGameEvents.OnPlayFailedMatchAnimation -= HandlePlayFailedMatchAnimation;
			concentrationGameEvents.OnPlayHitAutoSelctedtokenAnimation -= HandlePlayHitAutoSelctedtokenAnimation;
			concentrationGameEvents.OnPlayMatchTwoTokensAnimation -= HandlePlayMatchTwoTokensAnimation;
			concentrationGameEvents.OnPlayMatchWithWildAnimation -= HandlePlayMatchWithWildAnimation;
			concentrationGameEvents.OnPlayMissAnimation -= HandlePlayMissAnimation;
			concentrationGameEvents.OnPlayStealTokenAnimation -= HandlePlayStealTokenAnimation;
			concentrationGameEvents.OnPlayStealTokenWithWildAnimation -= HandlePlayStealTokenWithWildAnimation;
			concentrationGameEvents.OnPlayUndoFaceDownToFaceUpAnimation -= HandlePlayUndoFlipFaceDownToFaceUpAnimation;
			concentrationGameEvents.OnPlayUndoFaceUpToFaceDownAnimation -= HandlePlayUndoFlipFaceUpToFaceDownAnimation;
		}

		private void AtDisableStopAllCoroutines()
		{
			if (_flipAutoSelectedCardEnumerator != null)
			{
				StopCoroutine(_flipAutoSelectedCardEnumerator);
			}
			if (_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator != null)
			{
				StopCoroutine(_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator);
			}
			if (_flipCardFromFaceDownToFaceUpEnumerator != null)
			{
				StopCoroutine(_flipCardFromFaceDownToFaceUpEnumerator);
			}
			if (_flipFailedMatchingGameTokensEnumerator != null)
			{
				StopCoroutine(_flipFailedMatchingGameTokensEnumerator);
			}
			if (_flipPlayerSelectedCardEnumerator != null)
			{
				StopCoroutine(_flipPlayerSelectedCardEnumerator);
			}
			if (_flipPlayerSelectedCardFromFaceDownToFaceUpEnumerator != null)
			{
				StopCoroutine(_flipPlayerSelectedCardFromFaceDownToFaceUpEnumerator);
			}
			if (_playFailedMatchAnimationEnumerator != null)
			{
				StopCoroutine(_playFailedMatchAnimationEnumerator);
			}
			if (_playHitAutoSelctedtokenAnimationEnumerator != null)
			{
				StopCoroutine(_playHitAutoSelctedtokenAnimationEnumerator);
			}
			if (_playMatchTwoTokensAnimationEnumerator != null)
			{
				StopCoroutine(_playMatchTwoTokensAnimationEnumerator);
			}
			if (_playMatchWithWildAnimationEnumerator != null)
			{
				StopCoroutine(_playMatchWithWildAnimationEnumerator);
			}
			if (_playUndoFlipFaceDownToFaceUpAnimationEnumerator != null)
			{
				StopCoroutine(_playUndoFlipFaceDownToFaceUpAnimationEnumerator);
			}
		}

		private void HandleFlippingAutoSelectedToken(GameObject autoSelectedTokenToFlip)
		{
			_flipAutoSelectedCardEnumerator = FlipAutoSelectedCard(autoSelectedTokenToFlip);
			StartCoroutine(_flipAutoSelectedCardEnumerator);
		}

		private void HandleFlippingPlayerSelectedToken(GameObject playerSelectedTokenToFlip)
		{
			_flipPlayerSelectedCardEnumerator = FlipPlayerSelectedCard(playerSelectedTokenToFlip);
			StartCoroutine(_flipPlayerSelectedCardEnumerator);
		}

		private void HandlePlayFailedMatchAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			_playFailedMatchAnimationEnumerator = PlayFailedMatchAnimation(autoSelectedToken, playerSelectedToken);
			StartCoroutine(_playFailedMatchAnimationEnumerator);
		}

		private void HandlePlayHitAutoSelctedtokenAnimation(GameObject autoSelectedToken)
		{
			_playHitAutoSelctedtokenAnimationEnumerator = PlayHitAutoSelctedtokenAnimation(autoSelectedToken);
			StartCoroutine(_playHitAutoSelctedtokenAnimationEnumerator);
		}

		private void HandlePlayMatchTwoTokensAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			_playMatchTwoTokensAnimationEnumerator = PlayMatchTwoTokensAnimation(autoSelectedToken, playerSelectedToken);
			StartCoroutine(_playMatchTwoTokensAnimationEnumerator);
		}

		private void HandlePlayMatchWithWildAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			_playMatchWithWildAnimationEnumerator = PlayMatchWithWildAnimation(autoSelectedToken, playerSelectedToken);
			StartCoroutine(_playMatchWithWildAnimationEnumerator);
		}

		private void HandlePlayMissAnimation(GameObject autoSelectedToken)
		{
			HandlePlayHitAutoSelctedtokenAnimation(autoSelectedToken);
		}

		private void HandlePlayStealTokenAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken, string playerToStealTokenFrom)
		{
			_playMatchTwoTokensAnimationEnumerator = PlayMatchTwoTokensAnimation(autoSelectedToken, playerSelectedToken);
			StartCoroutine(_playMatchTwoTokensAnimationEnumerator);
		}

		private void HandlePlayStealTokenWithWildAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken, string playerToStealTokenFrom)
		{
			_playMatchWithWildAnimationEnumerator = PlayMatchWithWildAnimation(autoSelectedToken, playerSelectedToken);
			StartCoroutine(_playMatchWithWildAnimationEnumerator);
		}

		private void HandlePlayUndoFlipFaceDownToFaceUpAnimation(GameObject undoTokenToFlip)
		{
			_playUndoFlipFaceDownToFaceUpAnimationEnumerator = PlayUndoFlipFaceDownToFaceUpAnimation(undoTokenToFlip);
			StartCoroutine(_playUndoFlipFaceDownToFaceUpAnimationEnumerator);
		}

		private void HandlePlayUndoFlipFaceUpToFaceDownAnimation(GameObject undoTokenToFlip)
		{
			_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceUpToFaceDown(undoTokenToFlip);
			StartCoroutine(_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator);
		}

		private IEnumerator FlipAutoSelectedCard(GameObject autoSelectedTokenToFlip)
		{
			_flipCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceDownToFaceUp(autoSelectedTokenToFlip);
			yield return StartCoroutine(_flipCardFromFaceDownToFaceUpEnumerator);
			concentrationGameEvents.RaiseFinishedFlippngAutoSelecedCardFaceUp();
		}

		private IEnumerator FlipCardFromFaceDownToFaceUp(GameObject tokenToFlip)
		{
			if (!(tokenToFlip.transform.rotation.eulerAngles.y > 100f))
			{
				Animator objectAnimator = tokenToFlip.GetComponent<Animator>();
				objectAnimator.SetTrigger("FlipFaceUp");
				yield return new WaitForSeconds(0.5f);
				float length = objectAnimator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length - 0.5f);
			}
		}

		private IEnumerator FlipCardFromFaceUpToFaceDown(GameObject tokenToFlip)
		{
			if (!(tokenToFlip.transform.rotation.eulerAngles.y < 100f))
			{
				Animator objectAnimator = tokenToFlip.GetComponent<Animator>();
				objectAnimator.SetTrigger("FlipFaceDown");
				yield return new WaitForSeconds(0.5f);
				float length = objectAnimator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length - 0.5f);
			}
		}

		private IEnumerator FlipFailedMatchingGameTokens(GameObject autoSelectedTokenToFlip, GameObject playerSelectedTokenToFlip)
		{
			_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceUpToFaceDown(autoSelectedTokenToFlip);
			StartCoroutine(_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator);
			_flipPlayerSelectedCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceUpToFaceDown(playerSelectedTokenToFlip);
			yield return StartCoroutine(_flipPlayerSelectedCardFromFaceDownToFaceUpEnumerator);
		}

		private IEnumerator FlipPlayerSelectedCard(GameObject playerSelectedTokenToFlip)
		{
			_flipCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceDownToFaceUp(playerSelectedTokenToFlip);
			yield return StartCoroutine(_flipCardFromFaceDownToFaceUpEnumerator);
			concentrationGameEvents.RaiseFinishedFlippngPlayerSelecedCardFaceUp();
		}

		private IEnumerator PlayFailedMatchAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			GameObject autoSelectedFailedPlane = autoSelectedToken.GetComponent<GameTokenController>().FailedMatchObject;
			GameObject playerSelectedFailedPlane = playerSelectedToken.GetComponent<GameTokenController>().FailedMatchObject;
			yield return new WaitForSeconds(1f);
			autoSelectedFailedPlane.SetActive(value: true);
			playerSelectedFailedPlane.SetActive(value: true);
			yield return new WaitForSeconds(2f);
			_flipFailedMatchingGameTokensEnumerator = FlipFailedMatchingGameTokens(autoSelectedToken, playerSelectedToken);
			yield return StartCoroutine(_flipFailedMatchingGameTokensEnumerator);
			autoSelectedFailedPlane.SetActive(value: false);
			playerSelectedFailedPlane.SetActive(value: false);
			concentrationGameEvents.RaiseFinishedFailedMatchAnimation();
		}

		private IEnumerator PlayHitAutoSelctedtokenAnimation(GameObject autoSelectedToken)
		{
			GameObject autoSelectedFailedPlane = autoSelectedToken.GetComponent<GameTokenController>().FailedMatchObject;
			autoSelectedFailedPlane.SetActive(value: true);
			yield return new WaitForSeconds(2f);
			autoSelectedFailedPlane.SetActive(value: false);
			_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceUpToFaceDown(autoSelectedToken);
			yield return StartCoroutine(_flipAutoSelectedCardFromFaceDownToFaceUpEnumerator);
			concentrationGameEvents.RaiseFinishedHitAutoSelctedtokenAnimation();
		}

		private IEnumerator PlayMatchTwoTokensAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			GameObject autoSelectedParticleObject = autoSelectedToken.GetComponent<GameTokenController>().ParticleSystemObject;
			GameObject playerSelectedParticleObject = playerSelectedToken.GetComponent<GameTokenController>().ParticleSystemObject;
			autoSelectedParticleObject.SetActive(value: true);
			playerSelectedParticleObject.SetActive(value: true);
			yield return new WaitForSeconds(1f);
			autoSelectedParticleObject.SetActive(value: false);
			playerSelectedParticleObject.SetActive(value: false);
			concentrationGameEvents.RaiseRemoveMatchingTargets(autoSelectedToken, playerSelectedToken);
			yield return new WaitForSeconds(1f);
			concentrationGameEvents.RaiseFinishedMatchTwoTokensAnimation();
		}

		private IEnumerator PlayMatchWithWildAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			GameObject autoSelectedParticleObject = autoSelectedToken.GetComponent<GameTokenController>().ParticleSystemObject;
			GameObject playerSelectedParticleObject = playerSelectedToken.GetComponent<GameTokenController>().ParticleSystemObject;
			List<GameObject> matchingTokenForWildMatch;
			List<GameObject> matchingGameTokens = (matchingTokenForWildMatch = gameboardController.GetMatchingTokenForWildMatch(autoSelectedToken));
			yield return matchingTokenForWildMatch;
			autoSelectedParticleObject.SetActive(value: true);
			playerSelectedParticleObject.SetActive(value: true);
			foreach (GameObject gameObject in matchingGameTokens)
			{
				gameObject.GetComponent<GameTokenController>().ParticleSystemObject.SetActive(value: true);
				_flipCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceDownToFaceUp(gameObject);
				StartCoroutine(_flipCardFromFaceDownToFaceUpEnumerator);
			}
			yield return new WaitForSeconds(1f);
			concentrationGameEvents.RaiseRemoveMatchingTargetsWithWild(autoSelectedToken, playerSelectedToken, matchingGameTokens);
			yield return new WaitForSeconds(1f);
			concentrationGameEvents.RaiseFinishedMatchWithWildAnimation();
		}

		private IEnumerator PlayUndoFlipFaceDownToFaceUpAnimation(GameObject undoTokenToFlip)
		{
			_flipCardFromFaceDownToFaceUpEnumerator = FlipCardFromFaceDownToFaceUp(undoTokenToFlip);
			yield return StartCoroutine(_flipCardFromFaceDownToFaceUpEnumerator);
			concentrationGameEvents.RaiseFinishedGameboardUndo();
		}
	}
}
