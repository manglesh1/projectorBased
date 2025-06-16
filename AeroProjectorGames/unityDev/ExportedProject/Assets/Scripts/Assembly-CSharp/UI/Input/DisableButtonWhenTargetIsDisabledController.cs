using Games;
using Games.GameState;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Input
{
	public class DisableButtonWhenTargetIsDisabledController : MonoBehaviour
	{
		[SerializeField]
		private Button targetButton;

		[SerializeField]
		private GameEventsSO events;

		[SerializeField]
		private GameStateSO gameState;

		private void OnDisable()
		{
			events.OnTargetEnabled -= Enable;
			events.OnTargetDisabled -= Disable;
		}

		private void OnEnable()
		{
			targetButton.enabled = !gameState.IsTargetDisabled;
			events.OnTargetEnabled += Enable;
			events.OnTargetDisabled += Disable;
		}

		private void Enable()
		{
			targetButton.enabled = true;
		}

		private void Disable()
		{
			targetButton.enabled = false;
		}
	}
}
