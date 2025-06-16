using System;
using System.Collections.Generic;

namespace GameSession
{
	public class GameSessionTeamModel
	{
		public bool? Approved { get; set; }

		public DateTime DateCreated { get; set; }

		public List<GameSessionTeamOptionsModel> Options { get; set; } = new List<GameSessionTeamOptionsModel>();

		public bool SetupComplete { get; set; }

		public int TeamId { get; set; }

		public string TeamName { get; set; }
	}
}
