using System.Collections.Generic;
using Games.Cricket.Logic.Scoring;
using UnityEngine;

namespace Games.Cricket
{
	[CreateAssetMenu(menuName = "Context/Cricket/Cricket Context - ReadOnly")]
	public class CricketContextReadOnlySO : ScriptableObject
	{
		[SerializeField]
		private CricketContextSO context;

		public PlayerData CurrentPlayer => context.CurrentPlayer;

		public int CurrentPlayerIndex => context.CurrentPlayerIndex;

		public int CurrentThrow => context.CurrentThrow;

		public bool IsSinglePlayer => context.IsSinglePlayer;

		public IReadOnlyList<ScoreBucket> PlayerScores => context.PlayerScoreCollection.PlayerScores;

		public int ThrowsPerTurn => context.ThrowsPerTurn;

		public int ThrowsRemaining => context.ThrowsRemaining;
	}
}
