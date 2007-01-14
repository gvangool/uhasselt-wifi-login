using System;
using System.Collections.Generic;
using System.Text;

namespace UHasseltWifi
{
	public class UHasseltWifiHttpLogin : HttpLogin
	{
		#region Members
		private const string IpLowRange = @"193.190.0.1";
		private const string IpHighRange = @"193.190.5.254";
		private string _macAddress = @"00:00:00:00:00:00";
		#endregion

		#region Constructor
		public UHasseltWifiHttpLogin()
			: base(@"UHasselt")
		{
			SetUrl();
		}

		public UHasseltWifiHttpLogin(string macAddress)
			: base(@"UHasselt")
		{
			MacAddress = macAddress;
			SetUrl();
		}

		public UHasseltWifiHttpLogin(string username, string password)
			: base(@"UHasselt")
		{
			Username = username;
			Password = password;
			SetUrl();
		}

		public UHasseltWifiHttpLogin(string macAddress, string username, string password)
			: base(@"UHasselt")
		{
			MacAddress = macAddress;
			Username = username;
			Password = password;
			SetUrl();
		}
		#endregion

		#region Properties
		public string MacAddress
		{
			get { return _macAddress; }
			protected set { _macAddress = value; }
		}
		#endregion

		#region Method
		protected override void SetData()
		{
			StringBuilder postData = new StringBuilder(@"cmd=authenticate");
			postData.Append(@"&user=");
			postData.Append(Username);
			postData.Append(@"&password=");
			postData.Append(Password);

			PostData = postData.ToString();
			Method = @"POST";
		}

		private void SetUrl()
		{
			StringBuilder url = new StringBuilder(@"https://uhasselt-wifi.uhasselt.be/cgi-bin/login?cmd=login&mac=");
			url.Append(MacAddress);
			url.Append(@"&ip=");
			url.Append(GetDhcpIpAddress(IpLowRange, IpHighRange));
			url.Append(@"&essid=UHasselt-Public");
			URL = url.ToString();
		}
		#endregion
	}
}
