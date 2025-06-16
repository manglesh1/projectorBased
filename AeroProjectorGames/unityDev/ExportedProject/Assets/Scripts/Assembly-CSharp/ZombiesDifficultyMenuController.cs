using Games.GameState;
using Games.Models;
using UnityEngine;

public class ZombiesDifficultyMenuController : MonoBehaviour
{
	private GameManager _gameManager;

	[SerializeField]
	private GameObject easyModeOutlineImage;

	[SerializeField]
	private GameSO game;

	[SerializeField]
	private GameStateSO gameState;

	[SerializeField]
	private GameObject hardModeOutlineImage;

	[SerializeField]
	private GameObject zombieHuntOutlineImage;

	private void OnEnable()
	{
		gameState.GameDifficulty = GameDifficulties.Easy;
		gameState.GameType = string.Empty;
		zombieHuntOutlineImage.SetActive(value: true);
		easyModeOutlineImage.SetActive(value: true);
		hardModeOutlineImage.SetActive(value: false);
		_gameManager = Object.FindObjectOfType<GameManager>();
	}

	public void LoadGame()
	{
		_gameManager.LoadGame(game.GamePrefab, game);
	}
}
