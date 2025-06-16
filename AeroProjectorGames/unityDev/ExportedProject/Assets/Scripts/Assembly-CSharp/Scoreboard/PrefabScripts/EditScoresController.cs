using Games;
using UnityEngine;

namespace Scoreboard.PrefabScripts
{
	public class EditScoresController : MonoBehaviour
	{
		[SerializeField]
		private GameEventsSO gameEvents;

		public string PlayerName { get; set; }

		public int FrameIndex { get; set; }

		private void OnDisable()
		{
			FrameIndex = 0;
			PlayerName = null;
		}

		private void HandleBeginScoreEdit((string playerName, int frameIndex) frameInfo)
		{
			FrameIndex = frameInfo.frameIndex;
			(PlayerName, _) = frameInfo;
		}

		public void Cancel()
		{
			gameEvents.RaiseCancelScoreEdit();
		}

		public void EditScore(int score)
		{
			gameEvents.RaiseConfirmScoreEdit(PlayerName, FrameIndex, score);
		}
	}
}
