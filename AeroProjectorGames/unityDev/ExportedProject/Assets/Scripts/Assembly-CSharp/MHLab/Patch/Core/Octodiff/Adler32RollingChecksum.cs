namespace MHLab.Patch.Core.Octodiff
{
	internal sealed class Adler32RollingChecksum : IRollingChecksum
	{
		public string Name => "Adler32";

		public uint Calculate(byte[] block, int offset, int count)
		{
			int num = 1;
			int num2 = 0;
			for (int i = offset; i < offset + count; i++)
			{
				num = (ushort)(block[i] + num);
				num2 = (ushort)(num2 + num);
			}
			return (uint)((num2 << 16) | num);
		}

		public uint Rotate(uint checksum, byte remove, byte add, int chunkSize)
		{
			int num = (ushort)((checksum >> 16) & 0xFFFF);
			ushort num2 = (ushort)(checksum & 0xFFFF);
			num2 = (ushort)(num2 - remove + add);
			return (uint)(((ushort)(num - chunkSize * remove + num2 - 1) << 16) | num2);
		}
	}
}
