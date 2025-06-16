namespace GameSession.ApiResponses
{
	public class CreateGameSessionApiResponse
	{
		public GameSessionAndUrlModel Data { get; set; }

		public bool Success { get; set; }

		public string Reason { get; set; }
	}
}
