using Games.GameState;
using Games.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Ducks.Scripts
{
	public class DuckHunterDifficultySelectionController : MonoBehaviour
	{
		private GameManager _gameManager;

		[SerializeField]
		private GameObject duckHuntOutlineImage;

		[SerializeField]
		private GameObject easyModeOutlineImage;

		[SerializeField]
		private GameSO game;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private Button goButton;

		[SerializeField]
		private GameObject hardModeOutlineImage;

		private void OnEnable()
		{
			gameState.GameDifficulty = GameDifficulties.Easy;
			gameState.GameType = string.Empty;
			duckHuntOutlineImage.SetActive(value: false);
			easyModeOutlineImage.SetActive(value: true);
			hardModeOutlineImage.SetActive(value: false);
			_gameManager = Object.FindObjectOfType<GameManager>();
		}

		public void LoadGame()
		{
			gameState.GameDifficulty = ((!easyModeOutlineImage.activeSelf) ? GameDifficulties.Hard : GameDifficulties.Easy);
			_gameManager.LoadGame(game.GamePrefab, game);
		}
	}
}
