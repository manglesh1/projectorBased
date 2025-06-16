using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using Settings;
using UnityEngine;

namespace Games.BottleBash
{
	public class TokenSpawnerController : MonoBehaviour
	{
		private const int UNIQUE_TOKEN_COUNT = 5;

		private const int ROW_COUNT = 5;

		private const float SECONDS_BETWEEN_INSTATIATING = 0.5f;

		private List<int> _indexForSlectedTokensToUse = new List<int>();

		private bool _tokensAreSpawning;

		[SerializeField]
		private List<TokenSpawners> spawners;

		[SerializeField]
		private List<GameObject> tokens;

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[Header("External Reference")]
		[SerializeField]
		private BottleBashSaveStates bottleBashSaveStates;

		[SerializeField]
		private TokenMatchManager tokenMatchManager;

		public List<TokenSpawners> Spawners => spawners;

		private void OnEnable()
		{
			HandleNewGame();
			gameEvents.OnNewGame += HandleNewGame;
			gameEvents.OnNewRound += CheckIfTokensAreNeeded;
		}

		protected void OnDisable()
		{
			gameEvents.OnNewGame -= HandleNewGame;
			gameEvents.OnNewRound -= CheckIfTokensAreNeeded;
		}

		private void CheckIfTokensAreNeeded()
		{
			if (!_tokensAreSpawning)
			{
				_tokensAreSpawning = true;
				StartCoroutine(RunCheckIfTokensAreNeeded());
			}
		}

		private void HandleNewGame()
		{
			ResetTokens();
			CheckIfTokensAreNeeded();
		}

		private void ResetTokens(bool isNewGame = true)
		{
			if (isNewGame)
			{
				_indexForSlectedTokensToUse.Clear();
			}
			foreach (TokenSpawners spawner in spawners)
			{
				for (int num = spawner.GameSpawner.transform.childCount; num > 0; num--)
				{
					Object.DestroyImmediate(spawner.GameSpawner.transform.GetChild(num - 1).gameObject);
					if (SettingsStore.Interaction.MultiDisplayEnabled)
					{
						Object.DestroyImmediate(spawner.ScoringSpawner.transform.GetChild(num - 1).gameObject);
					}
				}
			}
		}

		private IEnumerator RunCheckIfTokensAreNeeded()
		{
			TokenSpawnerController spawnerController = this;
			spawnerController.gameState.DisableTarget();
			yield return null;
			foreach (TokenSpawners spawner in spawnerController.spawners)
			{
				if (spawner.GameSpawner.transform.childCount < 5)
				{
					spawnerController.StartCoroutine(spawnerController.SpawnTokens(spawner));
				}
			}
			yield return new WaitForSeconds(1.5f);
			spawnerController._tokensAreSpawning = false;
			if (!spawnerController.gameState.IsTargetDisabled || spawnerController.gameState.GameStatus != GameStatus.Finished)
			{
				spawnerController.gameState.EnableTarget();
			}
		}

		private IEnumerator RunSetToPreviousState(List<List<string>> previousState)
		{
			TokenSpawnerController spawnerController = this;
			yield return new WaitForSeconds(0.25f);
			spawnerController.gameState.DisableTarget();
			int spawnerIndex = 0;
			foreach (List<string> previousState2 in previousState)
			{
				spawnerController.StartCoroutine(spawnerController.SpawnPreviousStateTokens(previousState2, spawnerIndex));
				int num = spawnerIndex + 1;
				spawnerIndex = num;
			}
			yield return new WaitForSeconds(1.5f);
			spawnerController.gameState.EnableTarget();
		}

		private void SelectTokensToUse()
		{
			for (int i = 0; i < tokens.Count; i++)
			{
				_indexForSlectedTokensToUse.Add(i);
			}
			for (int num = _indexForSlectedTokensToUse.Count; num > 5; num--)
			{
				_indexForSlectedTokensToUse.RemoveAt(Random.Range(0, _indexForSlectedTokensToUse.Count));
			}
		}

		public void SetToPreviousState(List<List<string>> previousState)
		{
			StartCoroutine(RunSetToPreviousState(previousState));
		}

		private IEnumerator SpawnPreviousStateTokens(List<string> previousState, int spawnerIndex)
		{
			int i;
			for (i = 0; i < previousState.Count; i++)
			{
				GameObject gameObject1 = spawners[spawnerIndex].GameSpawner.transform.GetChild(i).gameObject;
				int index = this.tokens.FindIndex((GameObject t) => t.name == previousState[i]);
				BottleBashTokens tokens = new BottleBashTokens
				{
					GameToken = Object.Instantiate(this.tokens[index], gameObject1.transform.position, gameObject1.transform.rotation, spawners[spawnerIndex].GameSpawner.transform)
				};
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					GameObject gameObject2 = spawners[spawnerIndex].ScoringSpawner.transform.GetChild(i).gameObject;
					tokens.ScoringToken = Object.Instantiate(this.tokens[index], gameObject2.transform.position, gameObject2.transform.rotation, spawners[spawnerIndex].ScoringSpawner.transform);
					tokens.ScoringToken.GetComponent<TokenController>().Init(tokens, tokenMatchManager);
					Object.Destroy(gameObject2);
				}
				tokens.GameToken.GetComponent<TokenController>().Init(tokens, tokenMatchManager);
				Object.Destroy(gameObject1);
			}
			yield return null;
		}

		private IEnumerator SpawnTokens(TokenSpawners spawners)
		{
			if (_indexForSlectedTokensToUse.Count == 0)
			{
				SelectTokensToUse();
			}
			do
			{
				int index = Random.Range(0, _indexForSlectedTokensToUse.Count);
				BottleBashTokens tokens = new BottleBashTokens
				{
					GameToken = Object.Instantiate(this.tokens[index], spawners.GameSpawner.transform)
				};
				if (SettingsStore.Interaction.MultiDisplayEnabled)
				{
					tokens.ScoringToken = Object.Instantiate(this.tokens[index], spawners.ScoringSpawner.transform);
					tokens.ScoringToken.GetComponent<TokenController>().Init(tokens, tokenMatchManager);
				}
				tokens.GameToken.GetComponent<TokenController>().Init(tokens, tokenMatchManager);
				yield return new WaitForSeconds(0.5f);
			}
			while (spawners.GameSpawner.transform.childCount < 5);
		}
	}
}
