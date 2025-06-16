using UnityEngine;
using UnityEngine.Events;

namespace Games.Concentration.Scoreboard
{
	[CreateAssetMenu(menuName = "Games/Concentration/Scoreboard Events")]
	public class ConcentrationScoreboardEventsSO : ScriptableObject
	{
		public event UnityAction OnRecordMissedScore;

		public event UnityAction<GameObject, int> OnRecordStandardScore;

		public event UnityAction<GameObject, int> OnRecordWildScore;

		public event UnityAction<GameObject, int> OnRecordScoreWithStolenToken;

		public event UnityAction<string, int> OnRecordRemoveStolenTokenScore;

		public event UnityAction<string, int> OnUpdateScoreboardRemovingStolenTokenFromPlayer;

		public event UnityAction<string, GameObject> OnUpdateScoreboardWithStandardScore;

		public event UnityAction<string, GameObject> OnUpdateScoreboardWithStealScore;

		public void RaiseRecordMissedScore()
		{
			this.OnRecordMissedScore?.Invoke();
		}

		public void RaiseRecordStandardScore(GameObject gameToken, int tokenScoreValue)
		{
			this.OnRecordStandardScore?.Invoke(gameToken, tokenScoreValue);
		}

		public void RaiseRecordWildScore(GameObject gameToken, int tokenScoreValue)
		{
			this.OnRecordWildScore?.Invoke(gameToken, tokenScoreValue);
		}

		public void RaiseRecordScoreWithStolenToken(GameObject gameToken, int tokenScoreValue)
		{
			this.OnRecordScoreWithStolenToken?.Invoke(gameToken, tokenScoreValue);
		}

		public void RaiseRecordRemoveStolenTokenScore(string playerToRemoveScoreFrom, int tokenScoreValue)
		{
			this.OnRecordRemoveStolenTokenScore?.Invoke(playerToRemoveScoreFrom, tokenScoreValue);
		}

		public void RaiseUpdateScoreboardRemovingStolenTokenFromPlayer(string playerToStealFrom, int tokenScoreValue)
		{
			this.OnUpdateScoreboardRemovingStolenTokenFromPlayer?.Invoke(playerToStealFrom, tokenScoreValue);
		}

		public void RaiseUpdateScoreboardWithStandardScore(string playerName, GameObject gameToken)
		{
			this.OnUpdateScoreboardWithStandardScore?.Invoke(playerName, gameToken);
		}

		public void RaiseUpdateScoreboardWithStealScore(string playerName, GameObject stolenGameToken)
		{
			this.OnUpdateScoreboardWithStealScore?.Invoke(playerName, stolenGameToken);
		}
	}
}
