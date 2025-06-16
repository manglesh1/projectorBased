using System.Collections.Generic;
using Games.Concentration.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Games.Concentration.SO
{
	[CreateAssetMenu(menuName = "Games/Concentration/Game Events")]
	public class ConcentrationGameEventsSO : ScriptableObject
	{
		public event UnityAction OnAutomaticallySelectFirstCard;

		public event UnityAction<ConcentrationGameStates> OnChangeGameState;

		public event UnityAction OnEndPlayerTurn;

		public event UnityAction OnFinishedFlippngAutoSelecedCardFaceUp;

		public event UnityAction OnFinishedFlippngPlayerSelecedCardFaceUp;

		public event UnityAction OnFinishedGameboardSetup;

		public event UnityAction OnFinishedFailedMatchAnimation;

		public event UnityAction OnFinishedGameboardUndo;

		public event UnityAction OnFinishedHitAutoSelctedtokenAnimation;

		public event UnityAction OnFinishedMatchTwoTokensAnimation;

		public event UnityAction OnFinishedMatchWithWildAnimation;

		public event UnityAction OnFinishedSavingGameboardState;

		public event UnityAction<GameObject> OnFlipAutoSelectedGameToken;

		public event UnityAction<GameObject> OnFlipPlayerSelectedGameToken;

		public event UnityAction OnNoStandardTargetsToFlip;

		public event UnityAction<GameObject, GameObject> OnPlayFailedMatchAnimation;

		public event UnityAction<GameObject> OnPlayHitAutoSelctedtokenAnimation;

		public event UnityAction<GameObject, GameObject> OnPlayMatchTwoTokensAnimation;

		public event UnityAction<GameObject, GameObject> OnPlayMatchWithWildAnimation;

		public event UnityAction<GameObject> OnPlayMissAnimation;

		public event UnityAction<GameObject, GameObject, string> OnPlayStealTokenAnimation;

		public event UnityAction<GameObject, GameObject, string> OnPlayStealTokenWithWildAnimation;

		public event UnityAction<GameObject> OnPlayUndoFaceDownToFaceUpAnimation;

		public event UnityAction<GameObject> OnPlayUndoFaceUpToFaceDownAnimation;

		public event UnityAction<GameObject, GameObject> OnRemoveMatchingTargets;

		public event UnityAction<GameObject, GameObject, List<GameObject>> OnRemoveMatchingTargetsWithWild;

		public event UnityAction OnSaveGameboardState;

		public event UnityAction<GameObject> OnStoreAutoSelectedTokenValue;

		public event UnityAction OnSetupGameboard;

		public event UnityAction<GameObject> OnTargetClicked;

		public void RaiseAutomaticallySelectFirstCard()
		{
			this.OnAutomaticallySelectFirstCard?.Invoke();
		}

		public void RaiseChangeGameState(ConcentrationGameStates newGameState)
		{
			this.OnChangeGameState?.Invoke(newGameState);
		}

		public void RaiseEndPlayerTurn()
		{
			this.OnEndPlayerTurn?.Invoke();
		}

		public void RaiseFinishedFlippngAutoSelecedCardFaceUp()
		{
			this.OnFinishedFlippngAutoSelecedCardFaceUp?.Invoke();
		}

		public void RaiseFinishedFlippngPlayerSelecedCardFaceUp()
		{
			this.OnFinishedFlippngPlayerSelecedCardFaceUp?.Invoke();
		}

		public void RaiseFinishedGameboardSetup()
		{
			this.OnFinishedGameboardSetup?.Invoke();
		}

		public void RaiseFinishedFailedMatchAnimation()
		{
			this.OnFinishedFailedMatchAnimation?.Invoke();
		}

		public void RaiseFinishedGameboardUndo()
		{
			this.OnFinishedGameboardUndo?.Invoke();
		}

		public void RaiseFinishedHitAutoSelctedtokenAnimation()
		{
			this.OnFinishedHitAutoSelctedtokenAnimation?.Invoke();
		}

		public void RaiseFinishedMatchTwoTokensAnimation()
		{
			this.OnFinishedMatchTwoTokensAnimation?.Invoke();
		}

		public void RaiseFinishedMatchWithWildAnimation()
		{
			this.OnFinishedMatchWithWildAnimation?.Invoke();
		}

		public void RaiseFinishedSavingGameboardState()
		{
			this.OnFinishedSavingGameboardState?.Invoke();
		}

		public void RaiseFlipAutoSelectedGameToken(GameObject autoSelectedTokenToFlip)
		{
			this.OnFlipAutoSelectedGameToken?.Invoke(autoSelectedTokenToFlip);
		}

		public void RaiseFlipPlayerSelectedGameToken(GameObject playerSelectedTokenToFlip)
		{
			this.OnFlipPlayerSelectedGameToken?.Invoke(playerSelectedTokenToFlip);
		}

		public void RaiseNoStandardTargetsToFlip()
		{
			this.OnNoStandardTargetsToFlip?.Invoke();
		}

		public void RaisePlayFailedMatchAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			this.OnPlayFailedMatchAnimation?.Invoke(autoSelectedToken, playerSelectedToken);
		}

		public void RaisePlayHitAutoSelctedtokenAnimation(GameObject autoSelectedToken)
		{
			this.OnPlayHitAutoSelctedtokenAnimation?.Invoke(autoSelectedToken);
		}

		public void RaisePlayMatchTwoTokensAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			this.OnPlayMatchTwoTokensAnimation?.Invoke(autoSelectedToken, playerSelectedToken);
		}

		public void RaisePlayMatchWithWildAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken)
		{
			this.OnPlayMatchWithWildAnimation?.Invoke(autoSelectedToken, playerSelectedToken);
		}

		public void RaisePlayMissAnimation(GameObject autoSelectedToken)
		{
			this.OnPlayMissAnimation?.Invoke(autoSelectedToken);
		}

		public void RaisePlayStealTokenAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken, string playerToStealFrom)
		{
			this.OnPlayStealTokenAnimation?.Invoke(autoSelectedToken, playerSelectedToken, playerToStealFrom);
		}

		public void RaisePlayStealTokenWithWildAnimation(GameObject autoSelectedToken, GameObject playerSelectedToken, string playerToStealFrom)
		{
			this.OnPlayStealTokenWithWildAnimation?.Invoke(autoSelectedToken, playerSelectedToken, playerToStealFrom);
		}

		public void RaisePlayUndoFaceDownToFaceUpAnimation(GameObject undoTokenToFlipFaceUp)
		{
			this.OnPlayUndoFaceDownToFaceUpAnimation?.Invoke(undoTokenToFlipFaceUp);
		}

		public void RaisePlayUndoFaceUpToFaceDownAnimation(GameObject undoTokenToFlipFaceDown)
		{
			this.OnPlayUndoFaceUpToFaceDownAnimation?.Invoke(undoTokenToFlipFaceDown);
		}

		public void RaiseRemoveMatchingTargets(GameObject autoFlippedTargetToRemove, GameObject playerFlippedTargetToRemove)
		{
			this.OnRemoveMatchingTargets?.Invoke(autoFlippedTargetToRemove, playerFlippedTargetToRemove);
		}

		public void RaiseRemoveMatchingTargetsWithWild(GameObject autoSelectedTokenToRemove, GameObject playerSelectedTokenToRemove, List<GameObject> matchingTokensToRemove)
		{
			this.OnRemoveMatchingTargetsWithWild?.Invoke(autoSelectedTokenToRemove, playerSelectedTokenToRemove, matchingTokensToRemove);
		}

		public void RaiseSaveGameboardState()
		{
			this.OnSaveGameboardState?.Invoke();
		}

		public void RaiseStoreAutoSelectedTokenValue(GameObject autoSelectedTokenThatWasFlip)
		{
			this.OnStoreAutoSelectedTokenValue?.Invoke(autoSelectedTokenThatWasFlip);
		}

		public void RaiseSetupGameboard()
		{
			this.OnSetupGameboard?.Invoke();
		}

		public void RaiseTargetClicked(GameObject clickedTarget)
		{
			this.OnTargetClicked?.Invoke(clickedTarget);
		}
	}
}
