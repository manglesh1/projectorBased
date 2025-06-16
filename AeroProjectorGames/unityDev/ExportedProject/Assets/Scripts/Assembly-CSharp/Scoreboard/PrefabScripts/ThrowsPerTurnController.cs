using Games;
using Games.GameState;
using UnityEngine;
using UnityEngine.UI;

namespace Scoreboard.PrefabScripts
{
	public class ThrowsPerTurnController : MonoBehaviour
	{
		private const int ONE_THROW = 1;

		private const int TWO_THROW = 2;

		private const int THREE_THROW = 3;

		private const int FOUR_THROW = 4;

		private const int FIVE_THROW = 5;

		private ColorBlock _defaultButtonColors;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private Button oneThrowPerTurnButton;

		[SerializeField]
		private Button twoThrowPerTurnButton;

		[SerializeField]
		private Button threeThrowPerTurnButton;

		[SerializeField]
		private Button fourThrowPerTurnButton;

		[SerializeField]
		private Button fiveThrowPerTurnButton;

		private void Awake()
		{
			_defaultButtonColors = oneThrowPerTurnButton.colors;
		}

		private void OnEnable()
		{
			UpdateHighlights();
		}

		private void ClearHighlights()
		{
			oneThrowPerTurnButton.colors = _defaultButtonColors;
			twoThrowPerTurnButton.colors = _defaultButtonColors;
			threeThrowPerTurnButton.colors = _defaultButtonColors;
			fourThrowPerTurnButton.colors = _defaultButtonColors;
			fiveThrowPerTurnButton.colors = _defaultButtonColors;
		}

		private void HighlightButton(Button button)
		{
			ColorBlock colors = button.colors;
			colors.normalColor = Color.blue;
			button.colors = colors;
		}

		private void UpdateHighlights()
		{
			ClearHighlights();
			switch (gameState.ThrowsPerTurn)
			{
			case 1:
				HighlightButton(oneThrowPerTurnButton);
				break;
			case 2:
				HighlightButton(twoThrowPerTurnButton);
				break;
			case 3:
				HighlightButton(threeThrowPerTurnButton);
				break;
			case 4:
				HighlightButton(fourThrowPerTurnButton);
				break;
			case 5:
				HighlightButton(fiveThrowPerTurnButton);
				break;
			}
		}

		public void SetThrowsPerTurn(int throwsPerTurn)
		{
			gameState.ThrowsPerTurn = throwsPerTurn;
			gameState.ThrowsRemaining = throwsPerTurn;
			gameEvents.RaiseThrowsPerTurnUpdated();
		}
	}
}
