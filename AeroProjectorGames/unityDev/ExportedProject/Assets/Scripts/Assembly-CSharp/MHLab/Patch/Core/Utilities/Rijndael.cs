using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MHLab.Patch.Core.Utilities
{
	public static class Rijndael
	{
		private static readonly byte[] InitVectorBytes = Encoding.ASCII.GetBytes("tq4vdeji340tcvu2");

		private const int KeySize = 256;

		public static string Encrypt(string plainText, string passPhrase)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			byte[] bytes2 = new PasswordDeriveBytes(passPhrase, null).GetBytes(32);
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.CBC;
			using ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes2, InitVectorBytes);
			using MemoryStream memoryStream = new MemoryStream();
			using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			return Convert.ToBase64String(memoryStream.ToArray());
		}

		public static string Decrypt(string cipherText, string passPhrase)
		{
			byte[] array = Convert.FromBase64String(cipherText);
			byte[] bytes = new PasswordDeriveBytes(passPhrase, null).GetBytes(32);
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.CBC;
			using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, InitVectorBytes);
			using MemoryStream stream = new MemoryStream(array);
			using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
			byte[] array2 = new byte[array.Length];
			int count = cryptoStream.Read(array2, 0, array2.Length);
			return Encoding.UTF8.GetString(array2, 0, count);
		}
	}
}
