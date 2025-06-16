using UnityEngine;
using UnityEngine.Events;

namespace Assets.Games.Norse.Scripts
{
	[CreateAssetMenu(menuName = "Games/ThrowAxe/ThrowAxeEvents")]
	public class NorseEventsSO : ScriptableObject
	{
		public event UnityAction OnBubbleAnimationStarting;

		public event UnityAction OnBubbleBurst;

		public event UnityAction OnBubbleGrown;

		public event UnityAction<Vector2> OnGameBoardHit;

		public event UnityAction OnGameStarted;

		public event UnityAction OnHelpClosed;

		public event UnityAction OnHelpOpened;

		public event UnityAction OnHideGamePieces;

		public event UnityAction OnIntroCompleted;

		public event UnityAction OnIntroStarted;

		public event UnityAction OnPowerUpHit;

		public event UnityAction OnPowerUpSelected;

		public event UnityAction OnTargetGrown;

		public event UnityAction OnTargetHit;

		public event UnityAction OnWordSelection;

		public event UnityAction<string> OnWordSelected;

		public void RaiseOnBubbleAnimationStarting()
		{
			this.OnBubbleAnimationStarting?.Invoke();
		}

		public void RaiseOnBubbleBurst()
		{
			this.OnBubbleBurst?.Invoke();
		}

		public void RaiseOnBubbleGrown()
		{
			this.OnBubbleGrown?.Invoke();
		}

		public void RaiseOnGameBoardHit(Vector2 point)
		{
			this.OnGameBoardHit?.Invoke(point);
		}

		public void RaiseOnGameStarted()
		{
			this.OnGameStarted?.Invoke();
		}

		public void RaiseOnHelpClosed()
		{
			this.OnHelpClosed?.Invoke();
		}

		public void RaiseOnHelpOpened()
		{
			this.OnHelpOpened?.Invoke();
		}

		public void RaiseOnHideGamePieces()
		{
			this.OnHideGamePieces?.Invoke();
		}

		public void RaiseOnIntroCompleted()
		{
			this.OnIntroCompleted?.Invoke();
		}

		public void RaiseOnIntroStarted()
		{
			this.OnIntroStarted?.Invoke();
		}

		public void RaiseOnPowerUpHit()
		{
			this.OnPowerUpHit?.Invoke();
		}

		public void RaiseOnPowerUpSelected()
		{
			this.OnPowerUpSelected?.Invoke();
		}

		public void RaiseOnTargetGrown()
		{
			this.OnTargetGrown?.Invoke();
		}

		public void RaiseOnTargetHit()
		{
			this.OnTargetHit?.Invoke();
		}

		public void RaiseOnWordSelection()
		{
			this.OnWordSelection?.Invoke();
		}

		public void RaiseOnWordSelected(string word)
		{
			this.OnWordSelected?.Invoke(word);
		}
	}
}
