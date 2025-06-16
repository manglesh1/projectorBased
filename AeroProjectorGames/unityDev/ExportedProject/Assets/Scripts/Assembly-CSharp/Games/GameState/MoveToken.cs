using System;

namespace Games.GameState
{
	[Serializable]
	public class MoveToken
	{
		public string FromPlayer { get; set; }

		public int? FromPlayerScore { get; set; }

		public string TokenName { get; set; }

		public string ToPlayer { get; set; }

		public int? ToPlayerScore { get; set; }
	}
}
