using System;
using System.Collections.Generic;

namespace GameSession
{
	public class GameSessionModel
	{
		public bool? Approved { get; set; }

		public bool? ApprovalRequired { get; set; }

		public string GameSessionId { get; set; }

		public bool SetupComplete { get; set; }

		public List<GameSessionTeamModel> Teams { get; set; } = new List<GameSessionTeamModel>();

		public DateTime CreatedAt { get; set; }

		public string UploadImagesSite { get; set; }
	}
}
