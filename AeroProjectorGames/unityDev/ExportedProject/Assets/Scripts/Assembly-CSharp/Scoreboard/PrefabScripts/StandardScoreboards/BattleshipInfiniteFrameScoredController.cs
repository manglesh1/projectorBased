using Games;
using Games.Battleship.Scoreboard;
using Games.GameState;
using Players;
using TMPro;
using UnityEngine;

namespace Scoreboard.PrefabScripts.StandardScoreboards
{
	public class BattleshipInfiniteFrameScoredController : MonoBehaviour
	{
		private PlayerData _player;

		[SerializeField]
		private BattleshipScoreboardEventsSO battleshipScoreboardEvents;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private GameObject playerHightlight;

		[SerializeField]
		private int playerIndex;

		[SerializeField]
		private TextMeshProUGUI playerName;

		[Header("Default Colors")]
		[SerializeField]
		private Color invisibleColor;

		[SerializeField]
		private Color visibleColor;

		[Header("UI Elements")]
		[SerializeField]
		private GameObject twoHitShipSunkFlame;

		[SerializeField]
		private GameObject threeHitShipSunkFlame;

		[SerializeField]
		private GameObject fourHitShipSunkFlame;

		[Header("Eliminated Elements")]
		[SerializeField]
		private GameObject eliminatedColumn;

		[SerializeField]
		private GameObject twoHitColumn;

		[SerializeField]
		private GameObject threeHitColumn;

		[SerializeField]
		private GameObject fourHitColumn;

		private void OnEnable()
		{
			_player = playerState.players[playerIndex];
			playerName.text = _player.PlayerName;
			bool active = _player.PlayerName == playerState.players[0].PlayerName;
			playerHightlight.SetActive(active);
			InitializeFlameImages();
			battleshipScoreboardEvents.OnUpdateEliminatedPlayers += HandleUpdateEliminatedPlayers;
			battleshipScoreboardEvents.OnSetSunkenShipFlameImage += SetSunkenShipFlameImage;
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnGameOver += HandleUpdatePlayerHighlight;
			gameEvents.OnUpdatePlayerTurn += HandleUpdatePlayerHighlight;
			gameEvents.OnUpdateScoreboard += HandleUpdatePlayerHighlight;
		}

		private void OnDisable()
		{
			battleshipScoreboardEvents.OnUpdateEliminatedPlayers -= HandleUpdateEliminatedPlayers;
			battleshipScoreboardEvents.OnSetSunkenShipFlameImage -= SetSunkenShipFlameImage;
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnGameOver -= HandleUpdatePlayerHighlight;
			gameEvents.OnUpdatePlayerTurn -= HandleUpdatePlayerHighlight;
			gameEvents.OnUpdateScoreboard -= HandleUpdatePlayerHighlight;
		}

		private void HandleUpdateEliminatedPlayers(string playerToEliminate, bool removePlayerFromElimination = false, bool isUndo = false)
		{
			if (!(_player.PlayerName != playerToEliminate || isUndo) && !removePlayerFromElimination)
			{
				eliminatedColumn.SetActive(value: true);
				twoHitColumn.SetActive(value: false);
				threeHitColumn.SetActive(value: false);
				fourHitColumn.SetActive(value: false);
			}
		}

		private void HandleNewGame()
		{
			if (!gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				InitializeFlameImages();
			}
		}

		private void HandleUpdatePlayerHighlight()
		{
			bool active = gameState.CurrentPlayer == _player.PlayerName;
			playerHightlight.SetActive(active);
		}

		private void InitializeFlameImages()
		{
			twoHitShipSunkFlame.SetActive(value: false);
			threeHitShipSunkFlame.SetActive(value: false);
			fourHitShipSunkFlame.SetActive(value: false);
			eliminatedColumn.SetActive(value: false);
			twoHitColumn.SetActive(value: true);
			threeHitColumn.SetActive(value: true);
			fourHitColumn.SetActive(value: true);
		}

		private void SetSunkenShipFlameImage(string playerName, int shipScore, bool showFlameImage)
		{
			if (!(_player.PlayerName != playerName))
			{
				switch (shipScore)
				{
				case 2:
					twoHitShipSunkFlame.SetActive(showFlameImage);
					break;
				case 3:
					threeHitShipSunkFlame.SetActive(showFlameImage);
					break;
				case 4:
					fourHitShipSunkFlame.SetActive(showFlameImage);
					break;
				}
			}
		}
	}
}
