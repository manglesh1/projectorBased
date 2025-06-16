using System.Collections;
using System.Collections.Generic;
using Games.Battleship.ScoringLogic;
using Games.GameState;
using Settings;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Battleship
{
	public class MultiDisplayCellManager : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		private const string SCORING_EVENT_MESSAGE = "OnPointerDown";

		private const float SECONDS_TO_WAIT_BEFORE_SCORING = 3f;

		private const float SECONDS_TO_WAIT_BEFORE_SCORING_SHORT = 2f;

		private List<Coroutine> _coroutines;

		[SerializeField]
		private BattleshipGameboardManager battleshipGameboardManager;

		[SerializeField]
		private GameStateSO gameState;

		[Range(1f, 25f)]
		[SerializeField]
		private int cellPosition = 1;

		[Header("Cascade PointerDown Reference Object")]
		[SerializeField]
		private GameObject cascadePointerDownRef;

		[Header("Hit Elements")]
		[SerializeField]
		private GameObject hitFireAnimation;

		[Header("Miss Elements")]
		[SerializeField]
		private GameObject missImage;

		[Header("Target Lock Image")]
		[SerializeField]
		private GameObject targetLock;

		private void OnDestroy()
		{
			_coroutines?.ForEach(base.StopCoroutine);
		}

		private void OnEnable()
		{
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!gameState.IsTargetDisabled && gameState.GameStatus != GameStatus.Finished)
			{
				BattleshipScoreModel scoreWithoutRecording = battleshipGameboardManager.GetScoreWithoutRecording(cellPosition);
				SetCellAsTargeted();
				cascadePointerDownRef.SendMessage("OnPointerDown", eventData);
				if (_coroutines == null)
				{
					_coroutines = new List<Coroutine>();
				}
				_coroutines.Add(StartCoroutine(HandleScoring(scoreWithoutRecording.Score)));
			}
		}

		private IEnumerator HandleScoring(int score)
		{
			if (SettingsStore.Target.UseShorterBattleshipAnimations)
			{
				yield return new WaitForSecondsRealtime(2f);
			}
			else
			{
				yield return new WaitForSecondsRealtime(3f);
			}
			if (score != 0)
			{
				if (score - 2 <= 2)
				{
					SetCellAsHit();
				}
				else
				{
					ResetCell();
				}
			}
			else
			{
				SetCellAsMiss();
			}
		}

		private void SetCellAsTargeted()
		{
			ResetCell();
			targetLock.SetActive(value: true);
			targetLock.transform.SetParent(base.transform, worldPositionStays: false);
		}

		private void ResetCell()
		{
			hitFireAnimation.SetActive(value: false);
			missImage.SetActive(value: false);
			targetLock.SetActive(value: false);
		}

		public void SetCellAsEmpty()
		{
			ResetCell();
		}

		public void SetCellAsHit()
		{
			missImage.SetActive(value: false);
			hitFireAnimation.SetActive(value: true);
			targetLock.SetActive(value: false);
		}

		public void SetCellAsMiss()
		{
			hitFireAnimation.SetActive(value: false);
			missImage.SetActive(value: true);
			targetLock.SetActive(value: false);
		}

		public void SetCellState(GameboardCellStates state)
		{
			switch (state)
			{
			case GameboardCellStates.Empty:
				SetCellAsEmpty();
				break;
			case GameboardCellStates.Hit:
				SetCellAsHit();
				break;
			case GameboardCellStates.Miss:
				SetCellAsMiss();
				break;
			default:
				SetCellAsEmpty();
				break;
			}
		}
	}
}
