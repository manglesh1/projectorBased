using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace API.ResponseModels
{
	public class ApiResponseBuilder<T>
	{
		private const string GENERIC_ERROR_MESSAGE = "Oops, something went wrong";

		private const string NETWORK_ERROR_MESSAGE = "There was an error sending the request";

		private StringBuilder _errorMessage;

		private StringBuilder _response;

		private UnityWebRequest.Result? _result;

		public ApiResponseBuilder()
		{
			_errorMessage = new StringBuilder();
			_response = new StringBuilder();
		}

		private void AssertData()
		{
			if (!_result.HasValue)
			{
				throw new InvalidOperationException("Result cannot be null");
			}
		}

		public ApiResponseBuilder<T> AddResponse(string jsonResponse)
		{
			_response.Clear();
			_response.Append(jsonResponse);
			return this;
		}

		public ApiResponseBuilder<T> AddResult(UnityWebRequest.Result result)
		{
			_result = result;
			return this;
		}

		public ApiResponse<T> Build()
		{
			AssertData();
			ApiResponse<T> apiResponse = new ApiResponse<T>
			{
				Result = _result.Value
			};
			UnityWebRequest.Result value = _result.Value;
			if (value != UnityWebRequest.Result.Success)
			{
				if (value - 2 <= UnityWebRequest.Result.ConnectionError)
				{
					apiResponse.ErrorMessage = "There was an error sending the request";
					return apiResponse;
				}
				apiResponse.ErrorMessage = "Oops, something went wrong";
				return apiResponse;
			}
			try
			{
				T data = JsonConvert.DeserializeObject<T>(_response.ToString());
				apiResponse.Data = data;
				return apiResponse;
			}
			catch
			{
				apiResponse.Result = UnityWebRequest.Result.DataProcessingError;
				apiResponse.ErrorMessage = "Oops, something went wrong";
				return apiResponse;
			}
		}
	}
}
