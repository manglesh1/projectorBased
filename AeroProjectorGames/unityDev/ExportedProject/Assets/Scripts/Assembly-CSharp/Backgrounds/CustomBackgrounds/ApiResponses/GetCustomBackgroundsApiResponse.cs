using System.Collections.Generic;

namespace Backgrounds.CustomBackgrounds.ApiResponses
{
	public class GetCustomBackgroundsApiResponse
	{
		public List<BackgroundImageResponseData> Data { get; set; }

		public bool Success { get; set; }

		public string Reason { get; set; }
	}
}
