using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace UHasseltWifi
{
	/// <summary>An abstract class to create your own http login scripts </summary>
	public abstract class HttpLogin
	{
		#region Members
		/// <summary>The name of this object</summary>
		private readonly string _name = @"No name";
		/// <summary>The URL</summary>
		private string _url = @"http://selentic.net";
		/// <summary>Extra POST data (not intended for GET)</summary>
		private string _postData = @"";
		/// <summary>The username</summary>
		private string _username = @"";
		/// <summary>The password</summary>
		private string _password = @"";
		/// <summary>What method to use</summary>
		private string _method = "POST";
		#endregion

		#region Constructor
		/// <summary>Create a new HttpLogin</summary>
		/// <param name="name">The name of the class</param>
		public HttpLogin(string name)
		{
			_name = name;
		}
		#endregion

		#region Properties
		/// <summary>Get the name</summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>Get/Set the URL to request</summary>
		public string URL
		{
			get { return _url; }
			protected set { _url = value; }
		}

		/// <summary>Get/Set the username</summary>
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		/// <summary>Get/Set the password</summary>
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		/// <summary>Get/Set the method</summary>
		/// <remarks>Should only be GET or POST</remarks>
		public string Method
		{
			get { return _method; }
			protected set { _method = value; }
		}

		/// <summary>Get/Set extra POST data</summary>
		protected string PostData
		{
			get { return _postData; }
			set { _postData = value; }
		}
		#endregion

		#region Method
		/// <summary></summary>
		/// <param name="lowRange">The lowest IP in the range</param>
		/// <param name="highRange">The highest IP in the range</param>
		/// <returns>Returns the IP of one of the network adapters (which is in the given range)</returns>
		public string GetDhcpIpAddress(string lowRange, string highRange)
		{
			IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());
			string result = null;

			for (int i = 0; i < addr.Length && result == null; ++i)
			{
				string temp = addr[i].ToString();
				if (temp.CompareTo(lowRange) >= 0 && temp.CompareTo(highRange) <= 0)
				{
					result = temp;
				}
			}

			return result;
		}

		/// <summary>Finish the request</summary>
		public void MakeRequest()
		{
			SetData();
			// convert post data to bytes
			byte[] data = (new ASCIIEncoding()).GetBytes(PostData.ToString());

			ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertificates);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
			req.Method = Method;
			req.ContentLength = data.Length;

			if (Method == @"POST")
			{
				Stream newStream = req.GetRequestStream();
				// Send the data.
				newStream.Write(data, 0, PostData.Length);
				newStream.Close();
			}
		}

		/// <summary>Intended to use as the last place to set POST data, will be called before making the request</summary>
		protected abstract void SetData();

		/// <summary>A function to accept all SSL certificate</summary>
		/// <param name="sender"></param>
		/// <param name="certificate"></param>
		/// <param name="chain"></param>
		/// <param name="sslPolicyErrors"></param>
		/// <returns>true</returns>
		private bool AcceptAllCertificates(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
		#endregion
	}
}
