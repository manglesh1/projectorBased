using System.Collections;
using System.Collections.Generic;
using Games.Big_Axe_Hunter.Scripts;
using Settings;
using UnityEngine;

namespace Games.Big_Axe_Hunter.Gameboard
{
	public class ForestAnimalSpawnerController : MonoBehaviour
	{
		[Header("Events & State")]
		[SerializeField]
		private BigAxeHunterEventsSO bahEvents;

		[Header("Position and Spawn Information")]
		[SerializeField]
		private ViewPosition viewPosition;

		[SerializeField]
		private SpawnPoint spawnPoint;

		[Header("Animal")]
		[SerializeField]
		private GameObject animalParent;

		[SerializeField]
		private List<AnimalSO> spawnableAnimals2;

		private int _animalIndex;

		private readonly Stack<int> _undoStack = new Stack<int>();

		private const int ANIMAL_NOT_SET = -1;

		private void OnDisable()
		{
			DestroySpawnedAnimal();
			bahEvents.OnCameraMovingAwayFrom -= MakeAnimalFlee;
			bahEvents.OnCameraMovedTo -= LoadAnimalAtViewPosition;
			bahEvents.OnHitOrMiss -= HandleHitOrMiss;
			bahEvents.OnLoadAnimalRequest -= LoadAnimalRequestHandler;
			bahEvents.OnLoadAnimalsAtPosition -= LoadAnimalAtViewPosition;
			bahEvents.OnUndo -= HandleUndo;
			bahEvents.OnUnloadAnimalsAtPosition -= DestroySpawnedAnimalsAtPosition;
		}

		private void OnEnable()
		{
			DestroySpawnedAnimal();
			bahEvents.OnCameraMovingAwayFrom += MakeAnimalFlee;
			bahEvents.OnCameraMovedTo += LoadAnimalAtViewPosition;
			bahEvents.OnHitOrMiss += HandleHitOrMiss;
			bahEvents.OnLoadAnimalRequest += LoadAnimalRequestHandler;
			bahEvents.OnLoadAnimalsAtPosition += LoadAnimalAtViewPosition;
			bahEvents.OnUndo += HandleUndo;
			bahEvents.OnUnloadAnimalsAtPosition += DestroySpawnedAnimalsAtPosition;
		}

		private void MakeAnimalFlee(ViewPosition viewPositionInput)
		{
			if (viewPosition == viewPositionInput)
			{
				BigAxeHunterAnimalController componentInChildren = animalParent.GetComponentInChildren<BigAxeHunterAnimalController>();
				if (!(componentInChildren == null))
				{
					componentInChildren.Flee();
				}
			}
		}

		private void DestroySpawnedAnimalsAtPosition(ViewPosition viewPositionInput)
		{
			Debug.Log($"View Position: {viewPositionInput}");
			if (viewPosition != viewPositionInput)
			{
				DestroySpawnedAnimal();
			}
		}

		private bool AnimalSpawned()
		{
			return animalParent.GetComponentInChildren<BigAxeHunterAnimalController>() != null;
		}

		private void DestroySpawnedAnimal()
		{
			_animalIndex = -1;
			foreach (object item in animalParent.transform)
			{
				Object.Destroy(((Transform)item).gameObject);
			}
		}

		private void HandleHitOrMiss()
		{
			PushUndoHistory();
		}

		private void HandleUndo()
		{
			if (_undoStack.Count != 0)
			{
				DestroySpawnedAnimal();
				_animalIndex = _undoStack.Pop();
				if (_animalIndex != -1)
				{
					SpawnAnimal(spawnableAnimals2[_animalIndex]);
				}
			}
		}

		private void LoadAnimalAtViewPosition(ViewPosition viewPositionInput)
		{
			if (viewPositionInput != viewPosition)
			{
				DestroySpawnedAnimal();
			}
			else
			{
				StartCoroutine(SpawnAnimalRoutine());
			}
		}

		private void LoadAnimalRequestHandler(ViewPosition viewPositionInput, SpawnPoint spawnPointInput)
		{
			if (viewPosition == viewPositionInput && spawnPoint == spawnPointInput)
			{
				StartCoroutine(SpawnAnimalRoutine());
			}
		}

		private void PushUndoHistory()
		{
			_undoStack.Push(_animalIndex);
		}

		private IEnumerator SpawnAnimalRoutine()
		{
			yield return null;
			if (!AnimalSpawned())
			{
				DestroySpawnedAnimal();
				_animalIndex = Random.Range(0, spawnableAnimals2.Count);
				SpawnAnimal(spawnableAnimals2[_animalIndex]);
			}
		}

		private void SpawnAnimal(AnimalSO animal)
		{
			Object.Instantiate(animal.AnimalPrefab, animalParent.transform, worldPositionStays: false).GetComponentInChildren<BigAxeHunterAnimalController>().Initialize(spawnPoint, animal.ScoreValue);
			if (SettingsStore.Interaction.MultiDisplayEnabled)
			{
				bahEvents.RaiseMultiDisplayLoadAnimalIconForSpawnPoint(spawnPoint, animal.AnimalThumbnail);
			}
		}
	}
}
