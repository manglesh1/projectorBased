namespace Games.Cricket.Logic.Scoring
{
	public class ScoreBucket
	{
		private const int HITS_TO_OPEN = 3;

		private ScoreBucketKey _bucketKey;

		private int _playerIndex;

		private int _hits;

		private ScoreStatus _scoreStatus;

		public ScoreBucketKey BucketKey => _bucketKey;

		public int PlayerIndex => _playerIndex;

		public int TotalHits => _hits;

		public int Score
		{
			get
			{
				if (_hits <= 3)
				{
					return 0;
				}
				return (_hits - 3) * (int)_bucketKey;
			}
		}

		public ScoreStatus ScoreStatus => _scoreStatus;

		public ScoreBucket(ScoreBucketKey bucketKey, int playerIndex, int hits, ScoreStatus scoreStatus)
		{
			_bucketKey = bucketKey;
			_playerIndex = playerIndex;
			_hits = hits;
			_scoreStatus = scoreStatus;
		}

		public ScoreBucket GetNewInstance(int hits, ScoreStatus scoreStatus)
		{
			return new ScoreBucket(_bucketKey, _playerIndex, hits, scoreStatus);
		}
	}
}
