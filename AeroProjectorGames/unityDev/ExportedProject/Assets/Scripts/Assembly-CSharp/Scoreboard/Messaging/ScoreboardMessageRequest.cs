using System;
using JetBrains.Annotations;

namespace Scoreboard.Messaging
{
	public class ScoreboardMessageRequest
	{
		[CanBeNull]
		public Action MessageCompleteAction { get; }

		public string MessageText { get; }

		public ScoreboardMessageStyle MessageStyle { get; }

		public ScoreboardMessageRequest([CanBeNull] Action messageCompleteAction, string messageText, ScoreboardMessageStyle messageStyle)
		{
			MessageCompleteAction = messageCompleteAction;
			MessageText = messageText;
			MessageStyle = messageStyle;
		}
	}
}
