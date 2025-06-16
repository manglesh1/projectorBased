using System;
using UnityEngine;

namespace Games.SharedScoringLogic
{
	[Obsolete("Scoring Logic should be implemented without using the legacy events system")]
	public abstract class ScoringStrategyBase<T> : MonoBehaviour where T : IScore
	{
		public abstract void RecordScore(T score);
	}
}
