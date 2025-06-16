using UnityEngine;

namespace Settings
{
	public class RestoreSettingsCommand : MonoBehaviour
	{
		[SerializeField]
		private SettingsEventsSO settingEvents;

		public void Execute(string rawSettingsData)
		{
			SaveSettingsData(rawSettingsData);
			SettingsStore.Reload();
			settingEvents.RaiseSettingsReloaded();
		}

		private void SaveSettingsData(string settingsData)
		{
			new SettingsRepository().Overwrite(settingsData);
		}
	}
}
