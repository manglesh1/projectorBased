using System.Collections;
using Games.Battleship.ScoringLogic;
using Games.GameState;
using Settings;
using Target;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Battleship
{
	public class CellManager : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private const float WAIT_TIME_AFTER_HIT_ANIMATION = 2.5f;

		private const float WAIT_TIME_AFTER_MISS_ANIMATION = 1.5f;

		private const float WAIT_TIME_WITH_SHORTER_ANIMATION = 0.5f;

		[SerializeField]
		private BattleshipGameboardManager battleshipGameboardManager;

		[SerializeField]
		private GameStateSO gameState;

		[SerializeField]
		private MissileAnimationManager missileAnimationManager;

		[Range(1f, 25f)]
		[SerializeField]
		private int cellPosition = 1;

		[Header("Hit Elements")]
		[SerializeField]
		private GameObject hitExplosionAnimation;

		[SerializeField]
		private GameObject hitFireAnimation;

		[Header("Miss Elements")]
		[SerializeField]
		private GameObject missAnimationObject;

		[SerializeField]
		private GameObject missImage;

		[Header("Settings Elements")]
		private TargetSettings _targetSettings;

		public void OnPointerDown(PointerEventData eventData)
		{
			HandleCellInteraction();
		}

		private void OnHitDetected(PointerEventData pointerData)
		{
			HandleCellInteraction();
		}

		private void LoadTargetSettings()
		{
			_targetSettings = SettingsStore.Target;
		}

		private IEnumerator PlayAlreadyHitAnimation(BattleshipScoreModel cellScore)
		{
			yield return StartCoroutine(PlayMissileAnimation());
			yield return new WaitForSeconds(1.5f);
			battleshipGameboardManager.HandleScoreChange(0, cellScore.AttackedPlayerName);
		}

		private void HandleCellInteraction()
		{
			if (gameState.IsTargetDisabled || gameState.GameStatus == GameStatus.Finished)
			{
				return;
			}
			gameState.DisableTarget();
			BattleshipScoreModel battleshipScoreModel = battleshipGameboardManager.HandleActiveCell(cellPosition);
			if (battleshipScoreModel.Score == 0)
			{
				StartCoroutine(PlayMissAnimation(battleshipScoreModel));
				return;
			}
			if (battleshipScoreModel.Score == -1)
			{
				StartCoroutine(PlayAlreadyHitAnimation(battleshipScoreModel));
				return;
			}
			int score = battleshipScoreModel.Score;
			if (score - 2 <= 2)
			{
				StartCoroutine(PlayHitAnimation(battleshipScoreModel));
			}
		}

		private IEnumerator PlayHitAnimation(BattleshipScoreModel cellScore)
		{
			yield return StartCoroutine(PlayMissileAnimation());
			float duration = hitExplosionAnimation.GetComponent<ParticleSystem>().main.duration;
			hitExplosionAnimation.SetActive(value: true);
			yield return new WaitForSeconds(duration / 3f);
			hitFireAnimation.SetActive(value: true);
			LoadTargetSettings();
			if (_targetSettings.UseShorterBattleshipAnimations)
			{
				yield return new WaitForSeconds(1f);
			}
			else
			{
				yield return new WaitForSeconds(2.5f);
			}
			battleshipGameboardManager.HandleScoreChange(cellScore.Score, cellScore.AttackedPlayerName);
		}

		private IEnumerator PlayMissAnimation(BattleshipScoreModel cellScore)
		{
			yield return StartCoroutine(PlayMissileAnimation());
			float duration = missAnimationObject.GetComponent<ParticleSystem>().main.duration;
			missAnimationObject.SetActive(value: true);
			yield return new WaitForSeconds(duration);
			missImage.SetActive(value: true);
			LoadTargetSettings();
			if (_targetSettings.UseShorterBattleshipAnimations)
			{
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				yield return new WaitForSeconds(1.5f);
			}
			battleshipGameboardManager.HandleScoreChange(cellScore.Score, cellScore.AttackedPlayerName);
		}

		private IEnumerator PlayMissileAnimation()
		{
			yield return StartCoroutine(missileAnimationManager.PlayMissileAnimation(cellPosition));
		}

		private void ResetCell()
		{
			hitExplosionAnimation.SetActive(value: false);
			hitFireAnimation.SetActive(value: false);
			missAnimationObject.SetActive(value: false);
			missImage.SetActive(value: false);
		}

		public void SetCellAsEmpty()
		{
			ResetCell();
		}

		public void SetCellAsHit()
		{
			hitExplosionAnimation.SetActive(value: false);
			missAnimationObject.SetActive(value: false);
			missImage.SetActive(value: false);
			hitFireAnimation.SetActive(value: true);
		}

		public void SetCellAsMiss()
		{
			hitExplosionAnimation.SetActive(value: false);
			hitFireAnimation.SetActive(value: false);
			missAnimationObject.SetActive(value: false);
			missImage.SetActive(value: true);
		}
	}
}
