using System.Collections.Generic;
using Settings;

namespace Games.IATF.Ring_Target.Scripts
{
	public class IATFTargetSettings : ISettings
	{
		public Dictionary<IATFComponent, RingSettings> RingSettingsMap { get; set; } = new Dictionary<IATFComponent, RingSettings>();

		public SettingsKey StorageKey => SettingsKey.IATFTarget;

		public IATFTargetSettings()
		{
			Dictionary<IATFComponent, RingSettings> ringSettingsMap = RingSettingsMap;
			if (ringSettingsMap != null && !ringSettingsMap.ContainsKey(IATFComponent.Clutch1))
			{
				RingSettingsMap.Add(IATFComponent.Clutch1, new RingSettings
				{
					Height = 30f,
					Width = 30f,
					PositionX = -161.5f,
					PositionY = 201.5f
				});
			}
			Dictionary<IATFComponent, RingSettings> ringSettingsMap2 = RingSettingsMap;
			if (ringSettingsMap2 != null && !ringSettingsMap2.ContainsKey(IATFComponent.Clutch2))
			{
				RingSettingsMap.Add(IATFComponent.Clutch2, new RingSettings
				{
					Height = 30f,
					Width = 30f,
					PositionX = 161.5f,
					PositionY = 201.5f
				});
			}
			Dictionary<IATFComponent, RingSettings> ringSettingsMap3 = RingSettingsMap;
			if (ringSettingsMap3 != null && !ringSettingsMap3.ContainsKey(IATFComponent.Ring1))
			{
				RingSettingsMap.Add(IATFComponent.Ring1, new RingSettings
				{
					Height = 400f,
					Width = 400f,
					PositionX = 0f,
					PositionY = -13.5f
				});
			}
			Dictionary<IATFComponent, RingSettings> ringSettingsMap4 = RingSettingsMap;
			if (ringSettingsMap4 != null && !ringSettingsMap4.ContainsKey(IATFComponent.Ring2))
			{
				RingSettingsMap.Add(IATFComponent.Ring2, new RingSettings
				{
					Height = 250f,
					Width = 250f,
					PositionX = 0f,
					PositionY = -13.5f
				});
			}
			Dictionary<IATFComponent, RingSettings> ringSettingsMap5 = RingSettingsMap;
			if (ringSettingsMap5 != null && !ringSettingsMap5.ContainsKey(IATFComponent.Ring3))
			{
				RingSettingsMap.Add(IATFComponent.Ring3, new RingSettings
				{
					Height = 100f,
					Width = 100f,
					PositionX = 0f,
					PositionY = -13.5f
				});
			}
		}

		public void Save()
		{
			SettingsStore.Set(this);
		}
	}
}
