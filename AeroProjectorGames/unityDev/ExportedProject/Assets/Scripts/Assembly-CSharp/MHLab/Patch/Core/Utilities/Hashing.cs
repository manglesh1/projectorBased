using System;
using System.IO;
using System.Security.Cryptography;
using MHLab.Patch.Core.IO;

namespace MHLab.Patch.Core.Utilities
{
	public static class Hashing
	{
		private const long IOBufferSize = 8388608L;

		public static string GetFileHash(string filePath, IFileSystem fileSystem)
		{
			using (SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider())
			{
				return GetHash(filePath, hasher, fileSystem);
			}
		}

		private static string GetHash(string filePath, HashAlgorithm hasher, IFileSystem fileSystem)
		{
			using (Stream stream = fileSystem.GetFileStream((FilePath)filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				int bufferSize = (int)Math.Max(1L, Math.Min(stream.Length, 8388608L));
				using (BufferedStream s = new BufferedStream(stream, bufferSize))
				{
					return GetHash(s, hasher);
				}
			}
		}

		private static string GetHash(Stream s, HashAlgorithm hasher)
		{
			return Convert.ToBase64String(hasher.ComputeHash(s)).TrimEnd(new char[1] { '=' });
		}
	}
}
