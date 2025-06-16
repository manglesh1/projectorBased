using System;
using System.Collections.Generic;
using System.Linq;
using Games.Cricket.Logic.Scoring;
using Players;
using TMPro;
using UnityEngine;

namespace Games.Cricket.Scoreboard
{
	public class CricketPlayerGridController : MonoBehaviour
	{
		[Header("Context")]
		[SerializeField]
		private CricketContextReadOnlySO cricketContext;

		[SerializeField]
		private CricketScoreboardEventsSO scoreboardEvents;

		[SerializeField]
		private PlayerStateSO playerState;

		[SerializeField]
		private int playerIndex;

		[Header("UI Fields")]
		[SerializeField]
		private GameObject playerHighlight;

		[SerializeField]
		private TMP_Text playerNameTextField;

		[SerializeField]
		private TMP_Text totalScoreTextField;

		[SerializeField]
		private CricketFrameController frame20Controller;

		[SerializeField]
		private CricketFrameController frame19Controller;

		[SerializeField]
		private CricketFrameController frame18Controller;

		[SerializeField]
		private CricketFrameController frame17Controller;

		[SerializeField]
		private CricketFrameController frame16Controller;

		[SerializeField]
		private CricketFrameController frame15Controller;

		[SerializeField]
		private CricketFrameController frameBullController;

		private void OnEnable()
		{
			playerNameTextField.text = playerState.players[playerIndex].PlayerName;
			scoreboardEvents.OnUpdateScoreboard += HandleUpdate;
			HighlightPlayer();
		}

		private void OnDisable()
		{
			scoreboardEvents.OnUpdateScoreboard -= HandleUpdate;
			HighlightPlayer();
		}

		private void HandleTotalUpdate(int sum)
		{
			if (totalScoreTextField != null)
			{
				totalScoreTextField.text = sum.ToString();
			}
		}

		private void HandleUpdate()
		{
			IReadOnlyList<ScoreBucket> readOnlyList = cricketContext.PlayerScores.Where((ScoreBucket b) => b.PlayerIndex == playerIndex).ToList();
			foreach (ScoreBucket item in readOnlyList)
			{
				switch (item.BucketKey)
				{
				case ScoreBucketKey.Fifteen:
					frame15Controller.UpdateFrame(item);
					break;
				case ScoreBucketKey.Sixteen:
					frame16Controller.UpdateFrame(item);
					break;
				case ScoreBucketKey.Seventeen:
					frame17Controller.UpdateFrame(item);
					break;
				case ScoreBucketKey.Eighteen:
					frame18Controller.UpdateFrame(item);
					break;
				case ScoreBucketKey.Nineteen:
					frame19Controller.UpdateFrame(item);
					break;
				case ScoreBucketKey.Twenty:
					frame20Controller.UpdateFrame(item);
					break;
				case ScoreBucketKey.Bull:
					frameBullController.UpdateFrame(item);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			HandleTotalUpdate(readOnlyList.Sum((ScoreBucket b) => b.Score));
			HighlightPlayer();
		}

		private void HighlightPlayer()
		{
			playerHighlight.SetActive(cricketContext.CurrentPlayerIndex == playerIndex);
		}
	}
}
