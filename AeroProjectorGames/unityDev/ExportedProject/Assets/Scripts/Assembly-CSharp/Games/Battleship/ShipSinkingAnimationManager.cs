using System.Collections;
using Games.Battleship.Scoreboard;
using UnityEngine;

namespace Games.Battleship
{
	public class ShipSinkingAnimationManager : MonoBehaviour
	{
		private const float TWO_HIT_ANIMATION_LENGTH = 8.5f;

		private const float THREE_HIT_ANIMATION_LENGTH = 9f;

		private const float FOUR_HIT_ANIMATION_LENGTH = 15f;

		[SerializeField]
		private BattleshipScoreboardEventsSO battleshipScoreboardEvents;

		[Header("Ships Sinking Elements")]
		[SerializeField]
		private GameObject fourhitShipSinkinigGroup;

		[SerializeField]
		private GameObject sinkingAnimationWater;

		[SerializeField]
		private GameObject threehitShipSinkinigGroup;

		[SerializeField]
		private GameObject twohitShipSinkinigGroup;

		private void OnEnable()
		{
			InitializeShipSinkingObjects();
		}

		private void InitializeShipSinkingObjects()
		{
			fourhitShipSinkinigGroup.SetActive(value: false);
			sinkingAnimationWater.SetActive(value: false);
			threehitShipSinkinigGroup.SetActive(value: false);
			twohitShipSinkinigGroup.SetActive(value: false);
		}

		public IEnumerator PlaySinkingAnimation(int shipScore)
		{
			battleshipScoreboardEvents.RaiseIsGameboardVisible(isVisible: false);
			sinkingAnimationWater.SetActive(value: true);
			switch (shipScore)
			{
			case 2:
				twohitShipSinkinigGroup.SetActive(value: true);
				yield return new WaitForSeconds(8.5f);
				break;
			case 3:
				threehitShipSinkinigGroup.SetActive(value: true);
				yield return new WaitForSeconds(9f);
				break;
			case 4:
				fourhitShipSinkinigGroup.SetActive(value: true);
				yield return new WaitForSeconds(15f);
				break;
			}
			fourhitShipSinkinigGroup.SetActive(value: false);
			threehitShipSinkinigGroup.SetActive(value: false);
			twohitShipSinkinigGroup.SetActive(value: false);
			sinkingAnimationWater.SetActive(value: false);
			battleshipScoreboardEvents.RaiseIsGameboardVisible(isVisible: true);
		}
	}
}
