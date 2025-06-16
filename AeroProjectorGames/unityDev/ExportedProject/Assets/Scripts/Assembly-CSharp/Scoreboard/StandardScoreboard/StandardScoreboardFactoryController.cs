using System;
using Games.GameState;
using Players;
using UnityEngine;

namespace Scoreboard.StandardScoreboard
{
	public class StandardScoreboardFactoryController : MonoBehaviour
	{
		[Space]
		[Header("Standard 10 Frame Target Templates")]
		[SerializeField]
		private GameObject standardTarget10FrameTemplate1Player;

		[SerializeField]
		private GameObject standardTarget10FrameTemplate2Player;

		[SerializeField]
		private GameObject standardTarget10FrameTemplate3Player;

		[SerializeField]
		private GameObject standardTarget10FrameTemplate4Player;

		[SerializeField]
		private GameObject standardTarget10FrameTemplate5Player;

		[SerializeField]
		private GameObject standardTarget10FrameTemplate6Player;

		[Space]
		[Header("Standard 5 Frame Target Templates")]
		[SerializeField]
		private GameObject standardTarget5FrameTemplate1Player;

		[SerializeField]
		private GameObject standardTarget5FrameTemplate2Player;

		[SerializeField]
		private GameObject standardTarget5FrameTemplate3Player;

		[SerializeField]
		private GameObject standardTarget5FrameTemplate4Player;

		[SerializeField]
		private GameObject standardTarget5FrameTemplate5Player;

		[SerializeField]
		private GameObject standardTarget5FrameTemplate6Player;

		public GameObject GetScoreboard(NumberOfRounds numberOfRounds, PlayerStateSO playerState)
		{
			int count = playerState.players.Count;
			switch (numberOfRounds)
			{
			case NumberOfRounds.TenFrames:
				switch (count)
				{
				case 1:
					return standardTarget10FrameTemplate1Player;
				case 2:
					return standardTarget10FrameTemplate2Player;
				case 3:
					return standardTarget10FrameTemplate3Player;
				case 4:
					return standardTarget10FrameTemplate4Player;
				case 5:
					return standardTarget10FrameTemplate5Player;
				case 6:
					return standardTarget10FrameTemplate6Player;
				}
				break;
			case NumberOfRounds.FiveFrames:
				switch (count)
				{
				case 1:
					return standardTarget5FrameTemplate1Player;
				case 2:
					return standardTarget5FrameTemplate2Player;
				case 3:
					return standardTarget5FrameTemplate3Player;
				case 4:
					return standardTarget5FrameTemplate4Player;
				case 5:
					return standardTarget5FrameTemplate5Player;
				case 6:
					return standardTarget5FrameTemplate6Player;
				}
				break;
			}
			throw new ArgumentOutOfRangeException();
		}
	}
}
