using System;

namespace GameSession
{
	public class GameSessionTeamOptionsModel
	{
		public DateTime CreatedAt { get; set; }

		public string DataTypeKey { get; set; }

		public string DataResponseId { get; set; }

		public string GameSessionId { get; set; }

		public int TeamId { get; set; }

		public string Value { get; set; }
	}
}
