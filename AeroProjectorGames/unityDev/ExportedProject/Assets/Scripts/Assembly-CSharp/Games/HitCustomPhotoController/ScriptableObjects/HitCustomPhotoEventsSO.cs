using Games.CustomComponents;
using Games.HitCustomPhotoController.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Games.HitCustomPhotoController.ScriptableObjects
{
	[CreateAssetMenu(menuName = "Games/Hit Custom Photo/Game Events")]
	public class HitCustomPhotoEventsSO : ScriptableObject
	{
		public event UnityAction<HitCustomPhotoGameStates> OnChangeGameState;

		public event UnityAction OnEndGameSession;

		public event UnityAction OnEndPlayerTurn;

		public event UnityAction<ScoredButton> OnHitAnimationFinished;

		public event UnityAction<ScoredButton> OnTargetHit;

		public void RaiseChangeGameState(HitCustomPhotoGameStates newGameState)
		{
			this.OnChangeGameState?.Invoke(newGameState);
		}

		public void RaiseEndGameSession()
		{
			this.OnEndGameSession?.Invoke();
		}

		public void RaiseEndPlayerTurn()
		{
			this.OnEndPlayerTurn?.Invoke();
		}

		public void RaiseHitAnimationFinished(ScoredButton targetButton)
		{
			this.OnHitAnimationFinished?.Invoke(targetButton);
		}

		public void RaiseTargetHit(ScoredButton targetButton)
		{
			this.OnTargetHit?.Invoke(targetButton);
		}
	}
}
