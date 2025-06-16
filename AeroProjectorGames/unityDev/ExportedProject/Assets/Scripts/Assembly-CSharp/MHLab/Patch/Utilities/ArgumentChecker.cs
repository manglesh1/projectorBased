using System;

namespace MHLab.Patch.Utilities
{
	public static class ArgumentChecker
	{
		public static bool IsLaunchedWithCorrectParameter(string parameter, string expectedValue)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			foreach (string text in commandLineArgs)
			{
				if (text.StartsWith(parameter))
				{
					return text.Replace(parameter + "=", "") == expectedValue;
				}
			}
			return false;
		}
	}
}
