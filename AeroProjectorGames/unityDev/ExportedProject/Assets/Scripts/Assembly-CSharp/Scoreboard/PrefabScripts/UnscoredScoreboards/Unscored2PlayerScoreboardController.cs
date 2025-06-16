using Games;
using Games.GameState;
using Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scoreboard.PrefabScripts.UnscoredScoreboards
{
	public class Unscored2PlayerScoreboardController : MonoBehaviour
	{
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private TextMeshProUGUI Player1Text;

		[SerializeField]
		private TextMeshProUGUI Player2Text;

		[SerializeField]
		private Image Player1Image;

		[SerializeField]
		private Image Player2Image;

		private void OnEnable()
		{
			Player1Text.text = gameState.Player1Name;
			Player2Text.text = gameState.Player2Name;
			Player1Image.color = Color.yellow;
			Player1Image.sprite = gameState.Player1Icon;
			Player2Image.color = Color.red;
			Player2Image.sprite = gameState.Player2Icon;
			TogglePlayer1Active();
			gameEvents.OnUpdateScoreboard += HandleUpdateEvent;
		}

		private void OnDisable()
		{
			gameEvents.OnUpdateScoreboard -= HandleUpdateEvent;
		}

		private void HandleUpdateEvent()
		{
			if (gameState.CurrentPlayer == gameState.Player1Name)
			{
				TogglePlayer1Active();
			}
			else
			{
				TogglePlayer2Active();
			}
		}

		private void TogglePlayer1Active()
		{
			Player2Text.color = Color.white;
			Player2Image.gameObject.SetActive(value: false);
			Player1Text.color = Color.yellow;
			Player1Image.gameObject.SetActive(value: true);
		}

		private void TogglePlayer2Active()
		{
			Player1Text.color = Color.white;
			Player1Image.gameObject.SetActive(value: false);
			Player2Text.color = Color.red;
			Player2Image.gameObject.SetActive(value: true);
		}
	}
}
