using Settings;

namespace Interaction
{
	public class InteractionSettings : ISettings
	{
		public bool MultiDisplayEnabled { get; set; }

		public bool VirtualKeyboardEnabled { get; set; }

		public SettingsKey StorageKey => SettingsKey.InteractionSettings;

		public void Save()
		{
			SettingsStore.Set(this);
		}
	}
}
