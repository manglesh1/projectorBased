using System.Collections.Generic;
using Games.Battleship.Scoreboard;
using UnityEngine;

namespace Games.Battleship
{
	public class BattleshipObjectsManager : MonoBehaviour
	{
		[Header("Scriptable Objects")]
		[SerializeField]
		private BattleshipScoreboardEventsSO battleshipScoreboardEvents;

		[Header("Root Parent Objects")]
		[SerializeField]
		private GameObject _gameboardLayoutParent;

		[SerializeField]
		private GameObject _gameboardWaterParent;

		[SerializeField]
		private GameObject _missileAnimationParent;

		[SerializeField]
		private GameObject _scoringLogicParent;

		[SerializeField]
		private GameObject _shipSinkingAnimationsParent;

		[Header("Gameboard Elements")]
		[SerializeField]
		private List<GameObject> _gameboardRowElements = new List<GameObject>();

		[SerializeField]
		private List<GameObject> _gameboardWaterElements = new List<GameObject>();

		private void OnEnable()
		{
			battleshipScoreboardEvents.OnIsGameboardVisible += ShowGameboardElements;
		}

		protected void OnDisable()
		{
			battleshipScoreboardEvents.OnIsGameboardVisible -= ShowGameboardElements;
		}

		public bool InitializeGame()
		{
			InitializeNeededObjectsActive();
			ShowGameboardElements(isVisible: false);
			return true;
		}

		private void InitializeNeededObjectsActive()
		{
			_gameboardLayoutParent.SetActive(value: true);
			_missileAnimationParent.SetActive(value: true);
			_scoringLogicParent.SetActive(value: true);
			_shipSinkingAnimationsParent.SetActive(value: true);
			foreach (GameObject gameboardWaterElement in _gameboardWaterElements)
			{
				gameboardWaterElement.SetActive(value: true);
			}
		}

		private void ShowGameboardElements(bool isVisible)
		{
			_gameboardWaterParent.SetActive(isVisible);
			foreach (GameObject gameboardRowElement in _gameboardRowElements)
			{
				gameboardRowElement.SetActive(isVisible);
			}
		}
	}
}
