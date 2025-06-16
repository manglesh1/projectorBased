using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Core.Logging;
using MHLab.Patch.Core.Utilities;
using MHLab.Patch.Core.Utilities.Asserts;

namespace MHLab.Patch.Core.Client.IO
{
	public static class DownloaderHelper
	{
		public static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool result = true;
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						if (!chain.Build((X509Certificate2)certificate))
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		public static bool ValidateDownloadedResult(List<DownloadEntry> entries, IFileSystem fileSystem, ILogger logger)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("The following files are not valid after debug validation: ");
			foreach (DownloadEntry entry in entries)
			{
				LocalFileInfo fileInfo = fileSystem.GetFileInfo((FilePath)entry.DestinationFile);
				if (fileInfo.Size != entry.Definition.Size)
				{
					stringBuilder.AppendLine($"[{entry.DestinationFile}] with expected size of [{entry.Definition.Size}]. Found [{fileInfo.Size}]");
					flag = true;
				}
				else if (entry.Definition.Hash != null)
				{
					string fileHash = Hashing.GetFileHash(entry.DestinationFile, fileSystem);
					if (fileHash != entry.Definition.Hash)
					{
						stringBuilder.AppendLine("[" + entry.DestinationFile + "] with expected hash of [" + entry.Definition.Hash + "]. Found [" + fileHash + "]");
						flag = true;
					}
				}
			}
			if (flag)
			{
				logger.Debug(stringBuilder.ToString(), "W:\\AxeApp\\AxeApp.Game.Unity\\AxeApp\\Assets\\MHLab\\Patch\\Launcher\\Scripts\\Core\\IO\\DownloaderHelper.cs", 77L, "ValidateDownloadedResult");
				return false;
			}
			return true;
		}

		public static HttpWebRequest GetRequest(string remoteUrl, IWebProxy proxy, NetworkCredential credentials)
		{
			HttpWebRequest httpWebRequest = WebRequest.CreateHttp(remoteUrl);
			httpWebRequest.Credentials = credentials;
			httpWebRequest.Proxy = proxy;
			httpWebRequest.KeepAlive = true;
			httpWebRequest.Accept = "*/*";
			httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			Assert.AlwaysNotNull(httpWebRequest, "Cannot create the request: the RemoteUrl (" + remoteUrl + ") is an invalid HTTP resource.");
			return httpWebRequest;
		}

		public static HttpWebRequest GetRequest(string remoteUrl, long offsetStart, long offsetEnd, IWebProxy proxy, NetworkCredential credentials)
		{
			HttpWebRequest request = GetRequest(remoteUrl, proxy, credentials);
			request.AddRange(offsetStart, offsetEnd);
			return request;
		}

		public static HttpWebResponse GetResponse(HttpWebRequest request)
		{
			return (HttpWebResponse)request.GetResponse();
		}
	}
}
