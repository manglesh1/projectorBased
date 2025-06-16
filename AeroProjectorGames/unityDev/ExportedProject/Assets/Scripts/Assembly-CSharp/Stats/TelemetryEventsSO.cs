using UnityEngine;
using UnityEngine.Events;

namespace Stats
{
	[CreateAssetMenu(menuName = "Telemetry/Telemetry Events")]
	public class TelemetryEventsSO : ScriptableObject
	{
		public event UnityAction<int> OnGameStartedTelemetry;

		public event UnityAction OnGameCompletedTelemetry;

		public void RaiseGameStartedTelemetry(int gameId)
		{
			this.OnGameStartedTelemetry?.Invoke(gameId);
		}

		public void RaiseGameCompletedTelemetry()
		{
			this.OnGameCompletedTelemetry?.Invoke();
		}
	}
}
