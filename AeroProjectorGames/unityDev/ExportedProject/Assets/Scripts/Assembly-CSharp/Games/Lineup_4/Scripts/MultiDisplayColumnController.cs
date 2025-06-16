using System.Collections;
using System.Collections.Generic;
using Games.GameState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Lineup_4.Scripts
{
	public class MultiDisplayColumnController : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private List<GameObject> _spawnedTokens;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private GameEventsSO gameEvents;

		[SerializeField]
		private GameObject spawner;

		[SerializeField]
		private GameObject tokenPrefab;

		[Header("Main Game References")]
		[SerializeField]
		private GameObject mainColumn;

		[SerializeField]
		private Lineup4GameController mainGameController;

		private void OnEnable()
		{
			_spawnedTokens = new List<GameObject>();
			gameEvents.OnNewGame += Reset;
			gameEvents.OnUndo += UpdateCount;
		}

		private void OnDisable()
		{
			Reset();
			gameEvents.OnNewGame -= Reset;
			gameEvents.OnUndo -= UpdateCount;
		}

		public void OnPointerDown(PointerEventData pointerEventData)
		{
			ColumnHit(pointerEventData);
		}

		private void OnHitDetected(PointerEventData pointerEventData)
		{
			ColumnHit(pointerEventData);
		}

		private void ColumnHit(PointerEventData pointerEventData)
		{
			if (_spawnedTokens.Count < 5 && !gameState.IsTargetDisabled)
			{
				mainColumn.SendMessage("ColumnHit");
				SpawnToken();
			}
		}

		public void SpawnToken()
		{
			GameObject gameObject = Object.Instantiate(tokenPrefab, base.gameObject.transform);
			gameObject.transform.localPosition = Vector3.up;
			gameObject.GetComponent<SpriteRenderer>().color = ((gameState.CurrentPlayer == gameState.Player1Name) ? Color.yellow : Color.red);
			gameObject.name = base.gameObject.name;
			_spawnedTokens.Add(gameObject);
			mainGameController.RegisterMultiDisplayTokenInColumn(gameObject);
			StartCoroutine(WaitForTokenToDrop(gameObject));
		}

		private IEnumerator WaitForTokenToDrop(GameObject spawnedToken)
		{
			yield return new WaitForSecondsRealtime(1f);
		}

		private void UpdateCount()
		{
			for (int num = _spawnedTokens.Count - 1; num >= 0; num--)
			{
				if (_spawnedTokens[num] == null)
				{
					_spawnedTokens.RemoveAt(num);
				}
			}
		}

		private void Reset()
		{
			foreach (GameObject spawnedToken in _spawnedTokens)
			{
				Object.Destroy(spawnedToken);
			}
			_spawnedTokens.Clear();
		}
	}
}
