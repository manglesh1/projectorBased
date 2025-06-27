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
			return modifier switch
			{
				GameboardRingModifier.SingleThin => ScoringModifier.Single, 
				GameboardRingModifier.SingleWide => ScoringModifier.Single, 
				GameboardRingModifier.Double => ScoringModifier.Double, 
				GameboardRingModifier.Triple => ScoringModifier.Triple, 
				GameboardRingModifier.OuterBull => ScoringModifier.Single, 
				GameboardRingModifier.InnerBull => ScoringModifier.Double, 
				_ => throw new InvalidOperationException($"Unexpected GetMessageStylePanel value: {modifier}"), 
			};
		}
	}
}
