using System;

namespace Target
{
	[Serializable]
	public class TargetColorTheme
	{
		public TargetColorEnum TargetColorThemeName { get; set; }

		public bool AlternateColors { get; set; }

		public RingThemeModel RingValue1 { get; set; }

		public RingThemeModel RingValue2 { get; set; }

		public RingThemeModel RingValue3 { get; set; }

		public RingThemeModel RingValue4 { get; set; }

		public RingThemeModel RingValue5 { get; set; }

		public RingThemeModel RingValue6 { get; set; }

		public RingThemeModel RingValue8 { get; set; }
	}
}
