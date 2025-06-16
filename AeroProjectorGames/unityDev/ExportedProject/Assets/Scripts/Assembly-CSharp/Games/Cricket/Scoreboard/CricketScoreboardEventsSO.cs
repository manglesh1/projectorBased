using UnityEngine;
using UnityEngine.Events;

namespace Games.Cricket.Scoreboard
{
	[CreateAssetMenu(menuName = "Scoreboards/Cricket/Cricket Scoreboard Events")]
	public class CricketScoreboardEventsSO : ScriptableObject
	{
		public event UnityAction OnUpdateScoreboard;

		public void RaiseUpdateScoreboard()
		{
			this.OnUpdateScoreboard?.Invoke();
		}
	}
}
