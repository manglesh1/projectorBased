using System.Collections;
using System.Collections.Generic;
using Games.Cricket.Logic.Scoring;
using UnityEngine;

namespace Games.Cricket.Logic.Gameboard.Animations
{
	public class NoRecordedScoreAnimationController : MonoBehaviour
	{
		private const float TOTAL_DURATION = 3f;

		[Header("Segments")]
		[SerializeField]
		private GameObject animatedSegment;

		[SerializeField]
		private List<GameObject> animatedSegments = new List<GameObject>();

		[SerializeField]
		private List<GameObject> segmentList = new List<GameObject>();

		[Header("Bullseye Animated Section")]
		[SerializeField]
		private GameObject bullseyeAnimatedSegment;

		[Header("Orchestrator")]
		[SerializeField]
		private CricketGameboardAnimationOrchestrator orchestrator;

		private void OnEnable()
		{
			bullseyeAnimatedSegment.SetActive(value: false);
			animatedSegments.ForEach(delegate(GameObject g)
			{
				g.SetActive(value: true);
			});
			animatedSegment.SetActive(value: true);
			StartCoroutine(ShowHitHighlight());
		}

		private IEnumerator ShowHitHighlight()
		{
			if (orchestrator.StartsectionModel.BucketKey == ScoreBucketKey.Bull)
			{
				animatedSegments.ForEach(delegate(GameObject g)
				{
					g.SetActive(value: false);
				});
				bullseyeAnimatedSegment.SetActive(value: true);
			}
			else
			{
				bullseyeAnimatedSegment.SetActive(value: false);
				segmentList.ForEach(delegate(GameObject segment)
				{
					if (segment.name == orchestrator.StartsectionModel.GameObjectName)
					{
						animatedSegment.transform.rotation = segment.transform.parent.transform.rotation;
					}
				});
			}
			yield return new WaitForSecondsRealtime(3f);
			orchestrator.HandleEndPlayCricketGameboardAnimation();
		}
	}
}
