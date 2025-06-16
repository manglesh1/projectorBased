using Games.Big_Axe_Hunter.Scripts;
using UnityEngine;

namespace Games.Big_Axe_Hunter.Gameboard
{
	public class ForstHuntingGroundsController : MonoBehaviour
	{
		[SerializeField]
		private BigAxeHunterEventsSO bahEvents;

		[SerializeField]
		private Camera cam;

		private void OnDisable()
		{
		}

		private void OnEnable()
		{
			bahEvents.RaiseSendGameboardCameraToGame(cam);
		}
	}
}
