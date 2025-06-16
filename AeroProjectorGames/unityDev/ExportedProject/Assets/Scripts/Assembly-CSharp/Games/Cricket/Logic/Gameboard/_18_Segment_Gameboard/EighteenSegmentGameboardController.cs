using System.Collections;
using Games.Cricket.Logic.Scoring;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Games.Cricket.Logic.Gameboard._18_Segment_Gameboard
{
	public class EighteenSegmentGameboardController : MonoBehaviour
	{
		private const float ANIMATION_TIMING = 3f;

		private const string VARIABLE_BUCKET_KEY = "BucketKey";

		private const string VARIABLE_LOCATION = "Location";

		private const string VARIABLE_MODIFIER = "Modifier";

		[SerializeField]
		private CricketGameboardAnimationOrchestrator cricketGameboardAnimationOrchestrator;

		public event UnityAction<CricketGameboardHitModel> OnScore;

		public IEnumerator AnimateHit(CricketGameboardHitModel hitModel)
		{
			cricketGameboardAnimationOrchestrator.HandleStartPlayCricketGameboardAnimation(hitModel);
			yield return new WaitForSecondsRealtime(3f);
		}

		private void RaiseOnScore(CricketGameboardHitModel hitModel)
		{
			this.OnScore?.Invoke(hitModel);
		}

		public void PlayerClickedBullseye(bool innerRing = false)
		{
			CricketGameboardHitModel hitModel = new CricketGameboardHitModel(null, ScoreBucketKey.Bull, GameboardRingLocation.Middle, innerRing ? GameboardRingModifier.InnerBull : GameboardRingModifier.OuterBull);
			RaiseOnScore(hitModel);
		}

		public void PlayerClickedOnGameboard(SpriteRenderer clickedSpriteRenderer)
		{
			Transform parent = clickedSpriteRenderer.transform.parent;
			ScoreBucketKey bucketKey = parent.parent.GetComponent<Variables>().declarations.Get<ScoreBucketKey>("BucketKey");
			GameboardRingLocation location = parent.GetComponent<Variables>().declarations.Get<GameboardRingLocation>("Location");
			GameboardRingModifier modifier = clickedSpriteRenderer.gameObject.GetComponent<Variables>().declarations.Get<GameboardRingModifier>("Modifier");
			CricketGameboardHitModel hitModel = new CricketGameboardHitModel(parent.name, bucketKey, location, modifier);
			RaiseOnScore(hitModel);
		}
	}
}
