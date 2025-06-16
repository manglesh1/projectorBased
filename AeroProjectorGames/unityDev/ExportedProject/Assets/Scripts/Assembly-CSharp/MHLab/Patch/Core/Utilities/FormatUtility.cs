using System;

namespace MHLab.Patch.Core.Utilities
{
	public static class FormatUtility
	{
		private static readonly string[] SizesBinary = new string[9] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

		private static readonly string[] SizesDecimal = new string[9] { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

		public static string FormatSizeBinary(long size, int decimals)
		{
			double num = size;
			int num2 = 0;
			while (num >= 1024.0 && num2 < SizesBinary.Length)
			{
				num /= 1024.0;
				num2++;
			}
			return Math.Round(num, decimals) + SizesBinary[num2];
		}

		public static string FormatSizeBinary(ulong size, int decimals)
		{
			double num = size;
			int num2 = 0;
			while (num >= 1024.0 && num2 < SizesBinary.Length)
			{
				num /= 1024.0;
				num2++;
			}
			return Math.Round(num, decimals) + SizesBinary[num2];
		}

		public static string FormatSizeDecimal(long size, int decimals)
		{
			double num = size;
			int num2 = 0;
			while (num >= 1000.0 && num2 < SizesDecimal.Length)
			{
				num /= 1000.0;
				num2++;
			}
			return Math.Round(num, decimals) + SizesDecimal[num2];
		}

		public static string FormatSizeDecimal(ulong size, int decimals)
		{
			double num = size;
			int num2 = 0;
			while (num >= 1000.0 && num2 < SizesDecimal.Length)
			{
				num /= 1000.0;
				num2++;
			}
			return Math.Round(num, decimals) + SizesDecimal[num2];
		}
	}
}
