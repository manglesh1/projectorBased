using System.Text;

namespace MHLab.Patch.Core.Octodiff
{
	public sealed class BinaryFormat
	{
		public static readonly byte[] SignatureHeader = Encoding.ASCII.GetBytes("OCTOSIG");

		public static readonly byte[] DeltaHeader = Encoding.ASCII.GetBytes("OCTODELTA");

		public static readonly byte[] EndOfMetadata = Encoding.ASCII.GetBytes(">>>");

		public const byte CopyCommand = 96;

		public const byte DataCommand = 128;

		public const byte Version = 1;
	}
}
