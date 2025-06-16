using System.Collections.Generic;

namespace Telemetry.Models
{
	public class TelemetryCollection
	{
		public Dictionary<int, TelemetryGamesPlayedStats> GamesPlayed { get; set; } = new Dictionary<int, TelemetryGamesPlayedStats>();
	}
}
