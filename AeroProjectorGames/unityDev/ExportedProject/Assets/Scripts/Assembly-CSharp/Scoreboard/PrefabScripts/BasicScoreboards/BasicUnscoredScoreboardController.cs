using Games;
using Games.GameState;
using TMPro;
using UnityEngine;

namespace Scoreboard.PrefabScripts.BasicScoreboards
{
	public class BasicUnscoredScoreboardController : MonoBehaviour
	{
		[Header("Game State & Game Events")]
		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[Header("Current Player Label")]
		[SerializeField]
		private TMP_Text currentPlayerLabel;

		private void OnDisable()
		{
			gameEvents.OnUpdatePlayerTurn -= UpdateCurrentPlayer;
			gameEvents.OnUpdateScoreboard -= UpdateCurrentPlayer;
		}

		private void OnEnable()
		{
			gameEvents.OnUpdatePlayerTurn += UpdateCurrentPlayer;
			gameEvents.OnUpdateScoreboard += UpdateCurrentPlayer;
			UpdateCurrentPlayer();
		}

		private void UpdateCurrentPlayer()
		{
			currentPlayerLabel.text = gameState.CurrentPlayer;
		}
	}
}
