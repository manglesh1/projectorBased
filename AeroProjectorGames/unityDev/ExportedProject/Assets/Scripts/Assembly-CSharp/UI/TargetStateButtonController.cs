using Games;
using Games.GameState;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class TargetStateButtonController : MonoBehaviour
	{
		private Button _targetButton;

		[Header("Conditions")]
		[SerializeField]
		private bool onlyWhenGameIsFinished;

		[Header("External References")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		private void OnEnable()
		{
			_targetButton = GetComponent<Button>();
			if (gameState.IsTargetDisabled)
			{
				HandleOnTargetDisabled();
			}
			else
			{
				HandleOnTargetEnable();
			}
			gameEvents.OnTargetDisabled += HandleOnTargetDisabled;
			gameEvents.OnTargetEnabled += HandleOnTargetEnable;
		}

		private void OnDisable()
		{
			gameEvents.OnTargetDisabled -= HandleOnTargetDisabled;
			gameEvents.OnTargetEnabled -= HandleOnTargetEnable;
		}

		private void HandleOnTargetDisabled()
		{
			if (onlyWhenGameIsFinished)
			{
				_targetButton.interactable = gameState.GameStatus != GameStatus.Finished;
			}
			else
			{
				_targetButton.interactable = false;
			}
		}

		private void HandleOnTargetEnable()
		{
			_targetButton.interactable = true;
		}
	}
}
