namespace MHLab.Patch.Core.Versioning
{
	public sealed class VersionFactory : IVersionFactory
	{
		public IVersion Create(int major, int minor, int patch)
		{
			return new Version(major, minor, patch);
		}

		public IVersion Create(string version)
		{
			return new Version(version);
		}

		public IVersion Create(IVersion version)
		{
			return new Version(version);
		}

		public IVersion Create()
		{
			return new Version();
		}

		public IVersion Parse(string text)
		{
			return Version.Parse(text);
		}
	}
}
