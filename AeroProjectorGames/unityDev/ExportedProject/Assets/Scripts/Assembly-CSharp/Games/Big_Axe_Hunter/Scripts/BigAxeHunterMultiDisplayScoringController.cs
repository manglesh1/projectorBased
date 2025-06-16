using System.Collections;
using Games.GameState;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Big_Axe_Hunter.Scripts
{
	public class BigAxeHunterMultiDisplayScoringController : MonoBehaviour
	{
		private Coroutine _hitRoutine;

		private GameObject _spawnedAnimalPosition1;

		private GameObject _spawnedAnimalPosition2;

		[Header("Buttons")]
		[SerializeField]
		private Button position1Button;

		[SerializeField]
		private Button position2Button;

		[Header("Images")]
		[SerializeField]
		private Image position1Image;

		[SerializeField]
		private Image position2Image;

		[Header("BAH Specific")]
		[SerializeField]
		private BigAxeHunterEventsSO bahEvents;

		[Header("Game State")]
		[SerializeField]
		private GameStateSO gameState;

		private void OnDisable()
		{
			position1Button.onClick.RemoveAllListeners();
			position2Button.onClick.RemoveAllListeners();
			bahEvents.OnCameraMovingAwayFrom -= ClearAnimals;
			bahEvents.OnMultiDisplayLoadAnimalIconForSpawnPoint -= HandleAnimalIconChanged;
		}

		private void OnEnable()
		{
			position1Button.onClick.AddListener(delegate
			{
				HandleHit(SpawnPoint.SpawnPoint1);
			});
			position2Button.onClick.AddListener(delegate
			{
				HandleHit(SpawnPoint.SpawnPoint2);
			});
			bahEvents.OnCameraMovingAwayFrom += ClearAnimals;
			bahEvents.OnMultiDisplayLoadAnimalIconForSpawnPoint += HandleAnimalIconChanged;
		}

		private void ClearAnimals(ViewPosition _)
		{
			StopHitRoutine();
			position1Image.GameObject().SetActive(value: false);
			position2Image.GameObject().SetActive(value: false);
		}

		private void HandleHit(SpawnPoint spawnPoint)
		{
			if (!gameState.IsTargetDisabled)
			{
				StopHitRoutine();
				switch (spawnPoint)
				{
				case SpawnPoint.SpawnPoint1:
					_hitRoutine = StartCoroutine(HitRoutine(SpawnPoint.SpawnPoint1));
					break;
				case SpawnPoint.SpawnPoint2:
					_hitRoutine = StartCoroutine(HitRoutine(SpawnPoint.SpawnPoint2));
					break;
				}
			}
		}

		private void HandleAnimalIconChanged(SpawnPoint spawnPoint, Sprite animalIcon)
		{
			switch (spawnPoint)
			{
			case SpawnPoint.SpawnPoint1:
				position1Image.GameObject().SetActive(value: true);
				position1Image.sprite = animalIcon;
				break;
			case SpawnPoint.SpawnPoint2:
				position2Image.GameObject().SetActive(value: true);
				position2Image.sprite = animalIcon;
				break;
			}
		}

		private IEnumerator HitRoutine(SpawnPoint spawnPoint)
		{
			bahEvents.RaiseMultiDisplaySpawnPointHit(spawnPoint);
			if (spawnPoint == SpawnPoint.SpawnPoint1)
			{
				position2Image.GameObject().SetActive(value: false);
			}
			else
			{
				position1Image.GameObject().SetActive(value: false);
			}
			yield return new WaitUntil(() => !gameState.IsTargetDisabled);
			if (spawnPoint == SpawnPoint.SpawnPoint1)
			{
				position2Image.GameObject().SetActive(value: true);
			}
			else
			{
				position1Image.GameObject().SetActive(value: true);
			}
		}

		private void StopHitRoutine()
		{
			if (_hitRoutine != null)
			{
				StopCoroutine(_hitRoutine);
			}
		}
	}
}
