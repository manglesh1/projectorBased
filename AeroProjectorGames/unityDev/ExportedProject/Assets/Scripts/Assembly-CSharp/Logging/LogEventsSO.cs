using MHLab.Patch.Core.Logging;
using UnityEngine;
using UnityEngine.Events;

namespace Logging
{
	[CreateAssetMenu(menuName = "Logging/Log Events SO")]
	public class LogEventsSO : ScriptableObject
	{
		public event UnityAction<LogSeverity, string, string> OnLogRequest;

		public void RaiseLogRequest(LogSeverity logSeverity, string processName, string message)
		{
			UnityAction<LogSeverity, string, string> unityAction = this.OnLogRequest;
			if (unityAction != null)
			{
				MHLab.Patch.Core.Logging.Logger.LogInfo($"{logSeverity} {processName} {message}");
			}
		}
	}
}
