using Settings;

namespace Target
{
	public class TargetColorSettings : ISettings
	{
		public TargetColorEnum ChosenTargetColor { get; set; }

		public bool LetPlayerChooseColor { get; set; } = true;

		public SettingsKey StorageKey => SettingsKey.TargetColor;

		public void Save()
		{
			SettingsStore.Set(this);
		}
	}
}
