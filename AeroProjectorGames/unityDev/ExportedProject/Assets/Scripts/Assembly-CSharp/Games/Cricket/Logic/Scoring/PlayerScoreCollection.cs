using System.Collections.Generic;

namespace Games.Cricket.Logic.Scoring
{
	public class PlayerScoreCollection
	{
		private List<ScoreBucket> _playerScores;

		public IReadOnlyList<ScoreBucket> PlayerScores => _playerScores;

		public PlayerScoreCollection(List<PlayerData> players)
		{
			_playerScores = new List<ScoreBucket>();
			for (int i = 0; i < players.Count; i++)
			{
				_playerScores.Add(new ScoreBucket(ScoreBucketKey.Bull, i, 0, ScoreStatus.Available));
				_playerScores.Add(new ScoreBucket(ScoreBucketKey.Twenty, i, 0, ScoreStatus.Available));
				_playerScores.Add(new ScoreBucket(ScoreBucketKey.Nineteen, i, 0, ScoreStatus.Available));
				_playerScores.Add(new ScoreBucket(ScoreBucketKey.Eighteen, i, 0, ScoreStatus.Available));
				_playerScores.Add(new ScoreBucket(ScoreBucketKey.Seventeen, i, 0, ScoreStatus.Available));
				_playerScores.Add(new ScoreBucket(ScoreBucketKey.Sixteen, i, 0, ScoreStatus.Available));
				_playerScores.Add(new ScoreBucket(ScoreBucketKey.Fifteen, i, 0, ScoreStatus.Available));
			}
		}

		private PlayerScoreCollection(List<ScoreBucket> _scoreBuckets)
		{
			_playerScores = _scoreBuckets;
		}

		public void AddScore(int playerIndex, ScoreBucketKey bucketKey, ScoreStatus scoreStatus, int hits)
		{
			ScoreBucket scoreBucket = _playerScores.Find((ScoreBucket bucket) => bucket.PlayerIndex == playerIndex && bucket.BucketKey == bucketKey);
			ScoreBucket newInstance = scoreBucket.GetNewInstance(hits, scoreStatus);
			_playerScores.Remove(scoreBucket);
			_playerScores.Add(newInstance);
		}

		public PlayerScoreCollection Clone()
		{
			List<ScoreBucket> bucketClones = new List<ScoreBucket>();
			_playerScores.ForEach(delegate(ScoreBucket b)
			{
				bucketClones.Add(b.GetNewInstance(b.TotalHits, b.ScoreStatus));
			});
			return new PlayerScoreCollection(bucketClones);
		}

		public void CloseBuckets(ScoreBucketKey bucketKey)
		{
			List<ScoreBucket> list = _playerScores.FindAll((ScoreBucket b) => b.BucketKey == bucketKey && b.ScoreStatus != ScoreStatus.Closed);
			List<ScoreBucket> closedBuckets = new List<ScoreBucket>();
			list.ForEach(delegate(ScoreBucket b)
			{
				closedBuckets.Add(b.GetNewInstance(b.TotalHits, ScoreStatus.Closed));
			});
			_playerScores.RemoveAll((ScoreBucket sb) => sb.BucketKey == bucketKey && sb.ScoreStatus != ScoreStatus.Closed);
			_playerScores.AddRange(closedBuckets);
		}
	}
}
