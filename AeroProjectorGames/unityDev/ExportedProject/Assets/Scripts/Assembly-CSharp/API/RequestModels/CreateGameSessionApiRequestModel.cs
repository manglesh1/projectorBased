using System.Collections.Generic;

namespace API.RequestModels
{
	public class CreateGameSessionApiRequestModel
	{
		public string LicenseKey { get; set; }

		public int GameId { get; set; }

		public List<string> Players { get; set; }
	}
}
