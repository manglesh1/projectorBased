using System.Collections.Generic;
using System.Linq;
using Players;
using UnityEngine;

namespace Games.Cricket.Logic.Scoring
{
	public class CricketScoringLogic : MonoBehaviour
	{
		private const int PLAYER_BUCKET_COUNT = 7;

		private const int PLAYER_COUNT_QUARANTEED_WIN = 2;

		[SerializeField]
		private CricketContextSO context;

		[SerializeField]
		private PlayerStateSO playerState;

		public bool AddScore(ScoreBucketKey bucketKey, ScoringModifier modifier)
		{
			context.SaveState();
			bool result = false;
			if (!BucketClosed(bucketKey))
			{
				bool flag = false;
				ScoreBucket scoreBucket = context.PlayerScoreCollection.PlayerScores.First((ScoreBucket b) => b.PlayerIndex == context.CurrentPlayerIndex && b.BucketKey == bucketKey);
				int num = (int)(scoreBucket.TotalHits + modifier);
				ScoreStatus scoreStatus = GetNextScoreStatus(num);
				if (BucketClosing(bucketKey, scoreStatus))
				{
					flag = true;
					num = scoreBucket.TotalHits;
					scoreStatus = ScoreStatus.Closed;
				}
				context.PlayerScoreCollection.AddScore(context.CurrentPlayerIndex, bucketKey, scoreStatus, num);
				if (flag)
				{
					context.PlayerScoreCollection.CloseBuckets(bucketKey);
				}
				result = true;
			}
			NextThrow();
			return result;
		}

		private bool CheckForWinnerInTwoPlayerGame()
		{
			bool flag = context.PlayerScoreCollection.PlayerScores.Count((ScoreBucket b) => b.PlayerIndex == 0 && b.ScoreStatus == ScoreStatus.Available) != 0;
			int num = context.PlayerScoreCollection.PlayerScores.Count((ScoreBucket b) => b.PlayerIndex == 1 && b.ScoreStatus == ScoreStatus.Available);
			if (!flag)
			{
				List<PlayerData> currentScoredWinners = GetCurrentScoredWinners();
				if (playerState.players[0].PlayerName == currentScoredWinners[0].PlayerName && currentScoredWinners.Count == 1)
				{
					return true;
				}
			}
			if (num == 0)
			{
				List<PlayerData> currentScoredWinners2 = GetCurrentScoredWinners();
				if (playerState.players[1].PlayerName == currentScoredWinners2[0].PlayerName && currentScoredWinners2.Count == 1)
				{
					return true;
				}
			}
			return false;
		}

		private int GetScoringStatusCountForSinglePlayer(int PlayerIndex, ScoreStatus ScoringStatus)
		{
			return context.PlayerScoreCollection.PlayerScores.Count((ScoreBucket b) => b.PlayerIndex == PlayerIndex && b.ScoreStatus == ScoringStatus);
		}

		public void Miss()
		{
			context.SaveState();
			NextThrow();
		}

		public bool IsGameOver()
		{
			if (playerState.players.Count == 2 && CheckForWinnerInTwoPlayerGame())
			{
				return true;
			}
			return context.PlayerScoreCollection.PlayerScores.All((ScoreBucket b) => b.ScoreStatus == ScoreStatus.Closed);
		}

		public List<PlayerData> GetCurrentScoredWinners()
		{
			List<PlayerData> list = new List<PlayerData>();
			int num = 0;
			int playerIndex;
			for (playerIndex = 0; playerIndex < playerState.players.Count; playerIndex++)
			{
				int num2 = context.PlayerScoreCollection.PlayerScores.Where((ScoreBucket b) => b.PlayerIndex == playerIndex).Sum((ScoreBucket b) => b.Score);
				if (num2 == num)
				{
					list.Add(playerState.players[playerIndex]);
				}
				if (num2 > num)
				{
					num = num2;
					list.Clear();
					list.Add(playerState.players[playerIndex]);
				}
			}
			return list;
		}

		public PlayerData GetFirstUnscoredWinner()
		{
			PlayerData result = null;
			int playerIndex;
			for (playerIndex = 0; playerIndex < playerState.players.Count; playerIndex++)
			{
				if (context.PlayerScoreCollection.PlayerScores.Where((ScoreBucket b) => b.PlayerIndex == playerIndex).All((ScoreBucket b) => b.ScoreStatus == ScoreStatus.Closed || b.ScoreStatus == ScoreStatus.Open))
				{
					result = playerState.players[playerIndex];
					break;
				}
			}
			return result;
		}

		private bool BucketClosed(ScoreBucketKey bucketKey)
		{
			if (context.CurrentGameState == CricketGameState.Unscored)
			{
				return context.PlayerScoreCollection.PlayerScores.Where((ScoreBucket b) => b.BucketKey == bucketKey && b.PlayerIndex == context.CurrentPlayerIndex).All((ScoreBucket b) => b.ScoreStatus == ScoreStatus.Open || b.ScoreStatus == ScoreStatus.Closed);
			}
			return context.PlayerScoreCollection.PlayerScores.Where((ScoreBucket b) => b.BucketKey == bucketKey).All((ScoreBucket b) => b.ScoreStatus == ScoreStatus.Open || b.ScoreStatus == ScoreStatus.Closed);
		}

		private bool BucketClosing(ScoreBucketKey bucketKey, ScoreStatus playersNextScoreStatus)
		{
			if (playersNextScoreStatus == ScoreStatus.Available)
			{
				return false;
			}
			return context.PlayerScoreCollection.PlayerScores.Where((ScoreBucket b) => b.PlayerIndex != context.CurrentPlayerIndex && b.BucketKey == bucketKey).All((ScoreBucket b) => b.ScoreStatus == ScoreStatus.Open);
		}

		private ScoreStatus GetNextScoreStatus(int totalHits)
		{
			return totalHits switch
			{
				0 => ScoreStatus.Available, 
				1 => ScoreStatus.Available, 
				2 => ScoreStatus.Available, 
				3 => ScoreStatus.Open, 
				_ => ScoreStatus.Open, 
			};
		}

		private void NextThrow()
		{
			if (!context.NextThrow())
			{
				context.NextPlayer();
			}
		}
	}
}
