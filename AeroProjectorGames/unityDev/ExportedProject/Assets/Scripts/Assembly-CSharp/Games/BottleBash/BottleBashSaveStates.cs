using System.Collections.Generic;
using Games.GameState;
using UnityEngine;

namespace Games.BottleBash
{
	public class BottleBashSaveStates : MonoBehaviour
	{
		private Stack<List<List<string>>> _tokenPlacementHistory = new Stack<List<List<string>>>();

		[Header("Scriptable Objects")]
		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameStateSO gameState;

		[Header("External Reference")]
		[SerializeField]
		private TokenSpawnerController tokenSpawnerController;

		private void OnEnable()
		{
			gameEvents.OnMiss += SaveCurrentGameboardState;
			gameEvents.OnNewGame += ResetValues;
			gameEvents.OnUndo += HandleUndo;
		}

		protected void OnDisable()
		{
			gameEvents.OnMiss -= SaveCurrentGameboardState;
			gameEvents.OnNewGame -= ResetValues;
			gameEvents.OnUndo -= HandleUndo;
		}

		public void SaveCurrentGameboardState()
		{
			List<TokenSpawners> spawners = tokenSpawnerController.Spawners;
			List<List<string>> list = new List<List<string>>();
			int num = 0;
			foreach (TokenSpawners item in spawners)
			{
				list.Add(new List<string>());
				for (int i = 0; i < item.GameSpawner.transform.childCount; i++)
				{
					string[] array = item.GameSpawner.transform.GetChild(i).name.Split(new char[1] { '(' });
					list[num].Add(array[0]);
				}
				num++;
			}
			_tokenPlacementHistory.Push(list);
		}

		private void HandleUndo()
		{
			if (!gameState.IsTargetDisabled && gameState.GameStatus != GameStatus.Finished && _tokenPlacementHistory.Count != 0)
			{
				List<List<string>> toPreviousState = _tokenPlacementHistory.Pop();
				tokenSpawnerController.SetToPreviousState(toPreviousState);
			}
		}

		private void ResetValues()
		{
			_tokenPlacementHistory = new Stack<List<List<string>>>();
		}
	}
}
