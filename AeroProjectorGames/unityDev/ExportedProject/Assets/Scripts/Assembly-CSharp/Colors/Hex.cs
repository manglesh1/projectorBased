using System;
using System.Globalization;
using UnityEngine;

namespace Colors
{
	public static class Hex
	{
		public static Color ToColor(string hex)
		{
			string text = hex.TrimStart(new char[1] { '#' });
			if (text.Length != 8)
			{
				throw new ArgumentException($"Expected string length of 8, instead got {text.Length}");
			}
			float num = (int)byte.Parse(text.Substring(0, 2), NumberStyles.HexNumber);
			float num2 = (int)byte.Parse(text.Substring(2, 2), NumberStyles.HexNumber);
			float num3 = (int)byte.Parse(text.Substring(4, 2), NumberStyles.HexNumber);
			float num4 = (int)byte.Parse(text.Substring(6, 2), NumberStyles.HexNumber);
			return new Color(num / 255f, num2 / 255f, num3 / 255f, num4 / 255f);
		}
	}
}
