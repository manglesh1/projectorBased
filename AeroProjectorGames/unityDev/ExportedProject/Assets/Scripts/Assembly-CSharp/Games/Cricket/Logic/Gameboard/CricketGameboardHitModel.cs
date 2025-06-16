using System;
using Games.Cricket.Logic.Scoring;

namespace Games.Cricket.Logic.Gameboard
{
	public class CricketGameboardHitModel
	{
		public string GameObjectName { get; }

		public ScoreBucketKey BucketKey { get; }

		public GameboardRingLocation Location { get; }

		public GameboardRingModifier Modifier { get; }

		public bool ScoreRegistered { get; set; }

		public CricketGameboardHitModel(string gameObjectName, ScoreBucketKey bucketKey, GameboardRingLocation location, GameboardRingModifier modifier)
		{
			GameObjectName = gameObjectName;
			BucketKey = bucketKey;
			Location = location;
			Modifier = modifier;
		}

		public ScoringModifier GetScoringModifier()
		{
			GameboardRingModifier modifier = Modifier;
			switch (modifier)
			{
			case GameboardRingModifier.SingleThin:
				return ScoringModifier.Single;
			case GameboardRingModifier.SingleWide:
				return ScoringModifier.Single;
			case GameboardRingModifier.Double:
				return ScoringModifier.Double;
			case GameboardRingModifier.Triple:
				return ScoringModifier.Triple;
			case GameboardRingModifier.OuterBull:
				return ScoringModifier.Single;
			case GameboardRingModifier.InnerBull:
				return ScoringModifier.Double;
			default:
				throw new InvalidOperationException($"Unexpected GetMessageStylePanel value: {modifier}");
			}
		}
	}
}
