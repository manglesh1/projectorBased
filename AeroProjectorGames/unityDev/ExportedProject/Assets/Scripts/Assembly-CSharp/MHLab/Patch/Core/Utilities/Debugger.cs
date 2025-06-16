using MHLab.Patch.Core.Serializing;

namespace MHLab.Patch.Core.Utilities
{
	public static class Debugger
	{
		public static string GenerateDebugReport<TSettings>(TSettings settings, string additionalInfo, ISerializer serializer) where TSettings : ISettings
		{
			return "===================== START DEBUG REPORT =====================\n==============================================================\n\n" + additionalInfo + "\n\n" + settings.ToDebugString() + "\n\n====================== END DEBUG REPORT ======================\n==============================================================\n";
		}
	}
}
