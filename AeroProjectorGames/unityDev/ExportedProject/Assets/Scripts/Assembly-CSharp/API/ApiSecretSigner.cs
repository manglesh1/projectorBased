using System;
using System.Security.Cryptography;
using System.Text;

namespace API
{
	public static class ApiSecretSigner
	{
		private const string GENERAL_SIGNING_SECRET = "LCCS3f9xbbbo7PY5";

		private const string SESSION_DATA_SIGNING_SECRET = "OztDJ1hOy3yCAml2";

		public static bool IsValidData(string stringToSign, string providedHash, ApiSecretSignerType keyType)
		{
			using (HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes(GetSigningSecret(keyType))))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(stringToSign);
				return Convert.ToBase64String(hMACSHA.ComputeHash(bytes)) == providedHash;
			}
		}

		private static string GetSigningSecret(ApiSecretSignerType keyType)
		{
			switch (keyType)
			{
			default:
				throw new InvalidOperationException($"Unexpected GetSigningSecret value: {keyType}");
			case ApiSecretSignerType.SessionDataKey:
				return "OztDJ1hOy3yCAml2";
			case ApiSecretSignerType.GeneralDataKey:
				return "LCCS3f9xbbbo7PY5";
			}
		}
	}
}
