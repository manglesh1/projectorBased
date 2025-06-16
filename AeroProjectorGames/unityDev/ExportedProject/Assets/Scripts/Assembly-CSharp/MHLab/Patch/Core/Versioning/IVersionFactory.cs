namespace MHLab.Patch.Core.Versioning
{
	public interface IVersionFactory
	{
		IVersion Create(int major, int minor, int patch);

		IVersion Create(string version);

		IVersion Create(IVersion version);

		IVersion Create();

		IVersion Parse(string text);
	}
}
