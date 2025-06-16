using Games.GameState;
using Games.Models;
using UnityEngine;

namespace Games.Ducks.Scripts
{
	public class DuckSelectionMenuController : MonoBehaviour
	{
		private GameManager _gameManager;

		[SerializeField]
		private GameSO game;

		[SerializeField]
		private GameStateSO gameState;

		private void OnEnable()
		{
			gameState.GameType = Hit10GameTypes.DuckHunt.ToString();
			gameState.GameDifficulty = GameDifficulties.Easy;
			_gameManager = Object.FindObjectOfType<GameManager>();
		}

		public void LoadGame()
		{
			_gameManager.LoadGame(game.GamePrefab, game);
		}

		public void SelectedEasy()
		{
			gameState.GameDifficulty = GameDifficulties.Easy;
			LoadGame();
		}

		public void SelectedHard()
		{
			gameState.GameDifficulty = GameDifficulties.Hard;
			LoadGame();
		}
	}
}
