using System;
using System.IO;
using System.Net;

namespace MHLab.Patch.Core.Client.IO
{
	internal class DownloadData
	{
		private HttpWebResponse _response;

		private Stream _stream;

		private long _size;

		private long _start;

		public long Start => _start;

		public long Size => _size;

		public HttpWebResponse Response
		{
			get
			{
				return _response;
			}
			set
			{
				_response = value;
			}
		}

		public Stream DownloadStream
		{
			get
			{
				if (_start == _size)
				{
					return Stream.Null;
				}
				if (_stream == null)
				{
					_stream = _response.GetResponseStream();
				}
				return _stream;
			}
		}

		public static DownloadData Create(string url, Stream stream, NetworkCredential credentials, IWebProxy proxy)
		{
			DownloadData downloadData = new DownloadData();
			int num = 0;
			while (num < 3)
			{
				try
				{
					HttpWebRequest request = downloadData.GetRequest(url, credentials, proxy);
					if (stream.Length > 0)
					{
						downloadData._start = stream.Length;
						request.AddRange((int)downloadData._start);
					}
					downloadData._response = (HttpWebResponse)request.GetResponse();
					downloadData._size = downloadData._response.ContentLength;
					if (downloadData.Response.StatusCode != HttpStatusCode.PartialContent)
					{
						stream.Seek(0L, SeekOrigin.Begin);
						downloadData._start = 0L;
					}
					num = 3;
				}
				catch (WebException)
				{
					num++;
					if (num >= 3)
					{
						throw;
					}
					continue;
				}
				catch (Exception ex2)
				{
					throw new ArgumentException($"Error downloading \"{url}\": {ex2.Message}", ex2);
				}
				break;
			}
			ValidateResponse(downloadData._response, url);
			return downloadData;
		}

		private DownloadData()
		{
		}

		private static void ValidateResponse(HttpWebResponse response, string url)
		{
			HttpStatusCode statusCode = response.StatusCode;
		}

		private long GetFileSize(string url, NetworkCredential credentials, IWebProxy proxy)
		{
			HttpWebResponse httpWebResponse = null;
			long result = -1L;
			try
			{
				httpWebResponse = (HttpWebResponse)GetRequest(url, credentials, proxy).GetResponse();
				result = httpWebResponse.ContentLength;
			}
			finally
			{
				httpWebResponse?.Close();
			}
			return result;
		}

		private HttpWebRequest GetRequest(string url, NetworkCredential credentials, IWebProxy proxy)
		{
			if (url == null)
			{
				throw new ArgumentException("The URL parameter for this WebRequest is empty!", "url");
			}
			ServicePointManager.ServerCertificateValidationCallback = DownloaderHelper.RemoteCertificateValidationCallback;
			if (!(WebRequest.Create(url) is HttpWebRequest httpWebRequest))
			{
				throw new ArgumentException("The URL parameter is not an HTTP endpoint!", "url");
			}
			httpWebRequest.Credentials = credentials;
			httpWebRequest.Proxy = proxy;
			httpWebRequest.KeepAlive = true;
			return httpWebRequest;
		}

		public void Close()
		{
			_response.Close();
		}
	}
}
