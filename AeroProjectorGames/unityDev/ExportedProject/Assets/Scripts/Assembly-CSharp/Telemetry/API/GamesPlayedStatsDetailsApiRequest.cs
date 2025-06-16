namespace Telemetry.API
{
	public class GamesPlayedStatsDetailsApiRequest
	{
		public string LicenseKey { get; set; }

		public int GameId { get; set; }

		public int AddGamesCreatedCount { get; set; }

		public int AddTotalCompletedCount { get; set; }
	}
}
