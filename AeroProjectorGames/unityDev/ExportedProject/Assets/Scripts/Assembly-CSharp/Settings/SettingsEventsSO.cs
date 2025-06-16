using UnityEngine;
using UnityEngine.Events;

namespace Settings
{
	[CreateAssetMenu(menuName = "Settings/Settings Events", fileName = "SettingsEvents")]
	public class SettingsEventsSO : ScriptableObject
	{
		public event UnityAction OnSettingsReloaded;

		public void RaiseSettingsReloaded()
		{
			this.OnSettingsReloaded?.Invoke();
		}
	}
}
