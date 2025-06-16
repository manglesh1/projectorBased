using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Helpers;
using UnityEngine;

namespace Encryption
{
	public class EncryptServices
	{
		private const string FILE_NAME = "SystemHandler.Helper.dll";

		private string _fullPath;

		private LicenseEncryptionKey _encryptionKey;

		public string LicenseKey => _encryptionKey.LicenseKey;

		public EncryptServices()
		{
			_fullPath = DataPathHelpers.GetManagedFilePath(Application.persistentDataPath, "SystemHandler.Helper.dll");
			if (File.Exists(_fullPath))
			{
				try
				{
					_encryptionKey = JsonUtility.FromJson<LicenseEncryptionKey>(XOREncryptDecrypt(File.ReadAllText(_fullPath)));
					return;
				}
				catch
				{
					_encryptionKey = new LicenseEncryptionKey();
					return;
				}
			}
			_encryptionKey = new LicenseEncryptionKey();
		}

		public string AESDecryption(string inputData)
		{
			AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
			aesCryptoServiceProvider.BlockSize = 128;
			aesCryptoServiceProvider.KeySize = 256;
			aesCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(_encryptionKey.PrimaryKey);
			aesCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(_encryptionKey.SecondaryKey);
			aesCryptoServiceProvider.Mode = CipherMode.CBC;
			aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			byte[] array = Convert.FromBase64String(inputData);
			byte[] bytes = aesCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
			return Encoding.ASCII.GetString(bytes);
		}

		public string AESEncryption(string inputData)
		{
			AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
			aesCryptoServiceProvider.BlockSize = 128;
			aesCryptoServiceProvider.KeySize = 256;
			aesCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(_encryptionKey.PrimaryKey);
			aesCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(_encryptionKey.SecondaryKey);
			aesCryptoServiceProvider.Mode = CipherMode.CBC;
			aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			byte[] bytes = Encoding.ASCII.GetBytes(inputData);
			return Convert.ToBase64String(aesCryptoServiceProvider.CreateEncryptor(aesCryptoServiceProvider.Key, aesCryptoServiceProvider.IV).TransformFinalBlock(bytes, 0, bytes.Length));
		}

		public void DeleteLicenseFile()
		{
			if (File.Exists(_fullPath))
			{
				File.Delete(_fullPath);
				_encryptionKey = new LicenseEncryptionKey();
			}
		}

		public void SaveLicense(string ValidLicense)
		{
			if (_encryptionKey.LicenseKey?.Trim() != ValidLicense.Trim())
			{
				_encryptionKey.LicenseKey = ValidLicense.Trim();
				_encryptionKey.PrimaryKey = CreateEncryptionKeys(_encryptionKey.LicenseKey, 32);
				_encryptionKey.SecondaryKey = CreateEncryptionKeys(_encryptionKey.LicenseKey, 16);
				File.WriteAllText(_fullPath, XOREncryptDecrypt(JsonUtility.ToJson(_encryptionKey)));
			}
		}

		public string XOREncryptDecrypt(string inputData)
		{
			StringBuilder stringBuilder = new StringBuilder(inputData.Length);
			for (int i = 0; i < inputData.Length; i++)
			{
				char value = Convert.ToChar(inputData[i] ^ 0x4D2);
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		private string CreateEncryptionKeys(string KeyText, int KeyLength)
		{
			StringBuilder stringBuilder = new StringBuilder(KeyLength);
			if (KeyText.Length < KeyLength)
			{
				stringBuilder.Append(KeyText);
				stringBuilder.Append(RandomStringGenerator(KeyLength - KeyText.Length));
			}
			else
			{
				stringBuilder.Append(KeyText.Substring(0, KeyLength));
			}
			return stringBuilder.ToString();
		}

		private string RandomStringGenerator(int Length)
		{
			StringBuilder stringBuilder = new StringBuilder(Length);
			for (int i = 0; i < Length; i++)
			{
				stringBuilder.Append("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789"[UnityEngine.Random.Range(0, "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789".Length)]);
			}
			return stringBuilder.ToString();
		}
	}
}
