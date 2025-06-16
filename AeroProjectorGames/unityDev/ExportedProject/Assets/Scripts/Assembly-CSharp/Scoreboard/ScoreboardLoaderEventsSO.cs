using Scoreboard.Messaging;
using UnityEngine;
using UnityEngine.Events;

namespace Scoreboard
{
	[CreateAssetMenu(menuName = "Scoreboards/Scoreboard Loader Events")]
	public class ScoreboardLoaderEventsSO : ScriptableObject
	{
		public event UnityAction<ScoreboardType> OnLoadScoreboardRequest;

		public event UnityAction OnScoreboardMessageFinished;

		public event UnityAction<ScoreboardMessageRequest> OnScoreboardMessageRequest;

		public event UnityAction OnUnloadScoreboardRequest;

		public void RaiseLoadScoreboardRequest(ScoreboardType scoreboardType)
		{
			this.OnLoadScoreboardRequest?.Invoke(scoreboardType);
		}

		public void RaiseScoreboardMessageFinished()
		{
			this.OnScoreboardMessageFinished?.Invoke();
		}

		public void RaiseScoreboardMessageRequest(ScoreboardMessageRequest request)
		{
			this.OnScoreboardMessageRequest?.Invoke(request);
		}

		public void RaiseUnloadScoreboardRequest()
		{
			this.OnUnloadScoreboardRequest?.Invoke();
		}
	}
}
