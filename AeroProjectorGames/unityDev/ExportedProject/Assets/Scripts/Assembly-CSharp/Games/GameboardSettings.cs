using System.Globalization;
using Settings;
using UnityEngine;

namespace Games
{
	public class GameboardSettings : BaseResizableAndMovableSettings
	{
		private Color _backgroundColor;

		private string _backgroundColorAsHex;

		public string BackgroundColor
		{
			get
			{
				return _backgroundColorAsHex;
			}
			set
			{
				if (value.Length == 8)
				{
					_backgroundColor = HexToColor(value);
					_backgroundColorAsHex = value;
				}
			}
		}

		public override SettingsKey StorageKey => SettingsKey.Gameboard;

		public GameboardSettings()
		{
			SetDefaults();
		}

		public override void SetDefaults()
		{
			base.Width = 1f;
			base.Height = 1f;
			base.PositionX = 0f;
			base.PositionY = -790f;
			BackgroundColor = "000000FF";
		}

		private static Color HexToColor(string hex)
		{
			float num = (int)byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			float num2 = (int)byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			float num3 = (int)byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			return new Color(num / 255f, num2 / 255f, num3 / 255f, 1f);
		}

		public Color GetBackgroundColor()
		{
			return _backgroundColor;
		}
	}
}
