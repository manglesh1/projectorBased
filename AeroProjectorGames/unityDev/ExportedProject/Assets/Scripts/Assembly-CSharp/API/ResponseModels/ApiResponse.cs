using UnityEngine.Networking;

namespace API.ResponseModels
{
	public class ApiResponse<T>
	{
		public T Data { get; set; }

		public UnityWebRequest.Result Result { get; set; }

		public string ErrorMessage { get; set; }
	}
}
