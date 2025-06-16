using System;

namespace GameSession.ApiResponses
{
	[Serializable]
	public class GetGameSessionApiResponse
	{
		public GameSessionModel Data { get; set; }

		public bool Success { get; set; }

		public string Reason { get; set; }
	}
}
