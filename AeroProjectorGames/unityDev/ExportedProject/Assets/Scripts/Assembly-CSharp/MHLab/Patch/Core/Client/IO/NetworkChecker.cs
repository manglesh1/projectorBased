using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace MHLab.Patch.Core.Client.IO
{
	public class NetworkChecker : INetworkChecker
	{
		public NetworkCredential Credentials { get; set; }

		public IWebProxy Proxy { get; set; }

		public virtual bool IsNetworkAvailable()
		{
			return IsNetworkAvailable(1000000L);
		}

		public virtual bool IsNetworkAvailable(long minimumSpeed)
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
			{
				return false;
			}
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel && networkInterface.Speed >= minimumSpeed && networkInterface.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0 && networkInterface.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0 && !networkInterface.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool IsRemoteServiceAvailable(string url, out Exception exception)
		{
			try
			{
				using HttpClientHandler handler = new HttpClientHandler
				{
					Credentials = Credentials,
					Proxy = Proxy
				};
				using HttpClient httpClient = new HttpClient(handler);
				HttpResponseMessage result = httpClient.GetAsync(url).Result;
				exception = null;
				return result.StatusCode == HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				exception = ex;
				return false;
			}
		}
	}
}
