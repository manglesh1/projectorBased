using System.Linq;
using System.Net.NetworkInformation;
using API;
using Authentication;
using UnityEngine;

namespace Logging
{
	public class LogManager : MonoBehaviour
	{
		[SerializeField]
		private AxcitementApiHandler apiHandler;

		[SerializeField]
		private AuthenticationStateSO authenticationState;

		[SerializeField]
		private LogEventsSO logEvents;

		private void Awake()
		{
			logEvents.OnLogRequest += HandleLogRequest;
		}

		private void OnDisable()
		{
			logEvents.OnLogRequest -= HandleLogRequest;
		}

		private void HandleLogRequest(LogSeverity logSeverity, string processName, string message)
		{
			LogRecord logRecord = new LogRecord
			{
				LicenseExpiration = authenticationState.TokenExpiration,
				LaneNumber = authenticationState.LaneNumber,
				LicenseKey = authenticationState.LicenseKey,
				MacAddress = GetMacAddress(),
				Message = message,
				ProcessName = processName,
				SeverityId = (int)logSeverity
			};
			StartCoroutine(apiHandler.SendLog(logRecord));
		}

		private string GetMacAddress()
		{
			return NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault((NetworkInterface i) => i.OperationalStatus == OperationalStatus.Up && i.NetworkInterfaceType != NetworkInterfaceType.Loopback)?.GetPhysicalAddress().ToString() ?? string.Empty;
		}
	}
}
