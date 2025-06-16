using Games.Cricket.Logic.Scoring;
using UnityEngine;

namespace Games.Cricket.Scoreboard
{
	public class CricketFrameController : MonoBehaviour
	{
		[Header("Frame Closed")]
		[SerializeField]
		private GameObject frameClosedHighlight;

		[Header("Score Tokens")]
		[SerializeField]
		private GameObject singleHit;

		[SerializeField]
		private GameObject doubleHit;

		[SerializeField]
		private GameObject tripleHit;

		private void OnEnable()
		{
			frameClosedHighlight.SetActive(value: false);
		}

		public void UpdateFrame(ScoreBucket bucket)
		{
			ScoreStatus scoreStatus = bucket.ScoreStatus;
			int totalHits = bucket.TotalHits;
			switch (scoreStatus)
			{
			case ScoreStatus.Available:
				switch (totalHits)
				{
				case 0:
					SetFrameAvailable();
					break;
				case 1:
					SetFrameSingle();
					break;
				case 2:
					SetFrameDouble();
					break;
				}
				break;
			case ScoreStatus.Open:
				SetFrameOpen();
				break;
			case ScoreStatus.Closed:
				SetFrameClosed();
				break;
			}
		}

		private void SetFrameAvailable()
		{
			singleHit.SetActive(value: false);
			doubleHit.SetActive(value: false);
			tripleHit.SetActive(value: false);
			frameClosedHighlight.SetActive(value: false);
		}

		private void SetFrameSingle()
		{
			singleHit.SetActive(value: true);
			doubleHit.SetActive(value: false);
			tripleHit.SetActive(value: false);
			frameClosedHighlight.SetActive(value: false);
		}

		private void SetFrameDouble()
		{
			doubleHit.SetActive(value: true);
			singleHit.SetActive(value: false);
			tripleHit.SetActive(value: false);
			frameClosedHighlight.SetActive(value: false);
		}

		private void SetFrameOpen()
		{
			doubleHit.SetActive(value: false);
			singleHit.SetActive(value: false);
			tripleHit.SetActive(value: true);
			frameClosedHighlight.SetActive(value: false);
		}

		private void SetFrameClosed()
		{
			doubleHit.SetActive(value: false);
			singleHit.SetActive(value: false);
			tripleHit.SetActive(value: true);
			frameClosedHighlight.SetActive(value: true);
		}
	}
}
