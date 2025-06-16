using System.Collections.Generic;

namespace Telemetry.API
{
	public class GamesPlayedStatsApiRequest
	{
		public List<GamesPlayedStatsDetailsApiRequest> Data { get; set; } = new List<GamesPlayedStatsDetailsApiRequest>();
	}
}
