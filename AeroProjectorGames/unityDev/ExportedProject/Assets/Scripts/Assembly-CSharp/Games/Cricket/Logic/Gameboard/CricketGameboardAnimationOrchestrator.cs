using System.Collections.Generic;
using UnityEngine;

namespace Games.Cricket.Logic.Gameboard
{
	public class CricketGameboardAnimationOrchestrator : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> animationPrefabsList;

		[SerializeField]
		private GameObject gameplayGameboard;

		[SerializeField]
		private List<GameObject> innerBullseyeAnimationPrefabsList;

		[SerializeField]
		private List<GameObject> outerBullseyeAnimationPrefabsList;

		[SerializeField]
		private GameObject noScoreAnimation;

		public CricketGameboardHitModel StartsectionModel { get; set; }

		private void OnEnable()
		{
			animationPrefabsList.ForEach(delegate(GameObject a)
			{
				a.SetActive(value: false);
			});
			innerBullseyeAnimationPrefabsList.ForEach(delegate(GameObject a)
			{
				a.SetActive(value: false);
			});
			outerBullseyeAnimationPrefabsList.ForEach(delegate(GameObject a)
			{
				a.SetActive(value: false);
			});
			noScoreAnimation.SetActive(value: false);
		}

		public void HandleEndPlayCricketGameboardAnimation()
		{
			gameplayGameboard.SetActive(value: true);
			animationPrefabsList[0].SetActive(value: false);
			innerBullseyeAnimationPrefabsList[0].SetActive(value: false);
			outerBullseyeAnimationPrefabsList[0].SetActive(value: false);
			noScoreAnimation.SetActive(value: false);
		}

		public void HandleStartPlayCricketGameboardAnimation(CricketGameboardHitModel gameboardSectionHitModel)
		{
			StartsectionModel = gameboardSectionHitModel;
			if (!StartsectionModel.ScoreRegistered)
			{
				noScoreAnimation.SetActive(value: true);
			}
			else if (StartsectionModel.Modifier == GameboardRingModifier.OuterBull)
			{
				outerBullseyeAnimationPrefabsList[0].SetActive(value: true);
			}
			else if (StartsectionModel.Modifier == GameboardRingModifier.InnerBull)
			{
				innerBullseyeAnimationPrefabsList[0].SetActive(value: true);
			}
			else
			{
				animationPrefabsList[0].SetActive(value: true);
			}
			gameplayGameboard.SetActive(value: false);
		}
	}
}
