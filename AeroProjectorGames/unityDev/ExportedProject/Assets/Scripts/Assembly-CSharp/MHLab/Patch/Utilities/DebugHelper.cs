using System;
using UnityEngine;

namespace MHLab.Patch.Utilities
{
	public static class DebugHelper
	{
		public static string GetSystemInfo()
		{
			string text = "OS: " + SystemInfo.operatingSystem + " - Family: " + SystemInfo.operatingSystemFamily.ToString() + "\n";
			text = text + "Processor: " + SystemInfo.processorType + " " + SystemInfo.processorFrequency + "MHz (" + SystemInfo.processorCount + " vcore)\n";
			text = text + "Device: " + SystemInfo.deviceModel + " - Type: " + SystemInfo.deviceType.ToString() + "\n";
			text = text + "System Memory: " + SystemInfo.systemMemorySize + "MB\n";
			text = text + "GPU: " + SystemInfo.graphicsDeviceName + " - Vendor: " + SystemInfo.graphicsDeviceVendor + " - Type: " + SystemInfo.graphicsDeviceType.ToString() + " - Version: " + SystemInfo.graphicsDeviceVersion + " - Memory: " + SystemInfo.graphicsMemorySize + "MB - Shader: " + SystemInfo.graphicsShaderLevel + "\n";
			string[] array = new string[6]
			{
				text,
				"Process: ",
				(Environment.Is64BitProcess ? 64 : 32).ToString(),
				" bits - CLR Version: ",
				null,
				null
			};
			int num = 4;
			Version version = Environment.Version;
			array[num] = ((version != null) ? version.ToString() : null);
			array[5] = "\n";
			text = string.Concat(array);
			return text + "Commandline arguments: " + Environment.CommandLine;
		}
	}
}
