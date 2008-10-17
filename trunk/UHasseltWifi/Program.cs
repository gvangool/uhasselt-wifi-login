using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Management;
using System.Diagnostics;

using UHasseltWifi.Properties;

namespace UHasseltWifi
{
	class Program
	{
		static void Main(string[] args)
		{
            if (Settings.Default.Username.Equals("") || Settings.Default.Password.Equals(""))
            {
                System.Console.WriteLine("It looks you run this for the first time.");
                System.Console.WriteLine("We need your username and password, hé hé.");
                changeSettings();
            }
            else if ((args.Length > 1 && args[0].Equals("reset")))
            {
                System.Console.WriteLine("You want to change your settings, so let's do that.");
                changeSettings();
            }


            StringBuilder postData = new StringBuilder("cmd=authenticate");
			postData.Append(@"&user=");
			postData.Append(Settings.Default.Username);
			postData.Append(@"&password=");
			postData.Append(Settings.Default.Password);
			byte[] data = (new ASCIIEncoding()).GetBytes(postData.ToString());

			// https://securelogin.arubanetworks.com/cgi-bin/login?cmd=login&mac=00:1f:e1:83:b7:d6&ip=10.5.253.216&essid=UHasselt-Public
			StringBuilder url = new StringBuilder(@"https://securelogin.arubanetworks.com/cgi-bin/login?cmd=login&mac");
			// url.Append(settings.MacAddress);
			url.Append(GetWirelessMAC());
			url.Append(@"&ip=");
			url.Append(GetLocalIpAddress());
			url.Append(@"&essid=UHasselt-Public");

			System.Console.WriteLine("Loging in user: " + Settings.Default.Username);
			System.Console.WriteLine("Http request url: " + url);

			// Prepare web request...
			// Always accept the security certificate, wathever it is. There is no other way to login.
			System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.ToString());

				req.Method = "POST";
				// req.ContentType="application/x-www-form-urlencoded";
				req.ContentLength = data.Length;
				Stream newStream = req.GetRequestStream();

				// Send the data.
				newStream.Write(data, 0, postData.Length);
				newStream.Close();
			}
			catch (System.Net.WebException)
			{
				System.Console.WriteLine("Connection coud not made!");
			}

			System.Console.WriteLine("End");
		}

		private static string GetLocalIpAddress()
		{
			IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());

			foreach (IPAddress addres in addresses)
			{
				string addresStr = addres.ToString();
				if (addresStr.CompareTo(Resources.startIP) >= 0 && 
					addresStr.CompareTo(Resources.endIP) <= 0 )
					return addresStr;
			}

			return "";
		}

		private static string GetWirelessMAC()
		{
			Debug.WriteLine("start finding mac");
			ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
			ManagementObjectCollection moc = mc.GetInstances();
			string MACAddress = String.Empty;
			foreach (ManagementObject mo in moc)
			{
				if (MACAddress == String.Empty) // only return MAC Address from first card
				{
					if ((bool)mo["IPEnabled"] == true &&
						(bool)mo["DHCPEnabled"] == true &&
						Resources.DNSDomain.Length > 0 && Resources.DNSDomain.Equals(mo["DNSDomain"]) &&
						Resources.ipDHCPserver.Length > 0 && Resources.ipDHCPserver.Equals(mo["DHCPServer"])
					   ) 
						MACAddress = mo["MacAddress"].ToString();
				}
				foreach (PropertyData o in mo.Properties)
				{
					Debug.WriteLineIf(mo[o.Name] != null, o.Name + ": " + mo[o.Name]);
				}
				mo.Dispose();
				Debug.WriteLine("-------------");
			}
			return MACAddress;
		}

        private static void changeSettings()
        {
            System.Console.Write("Your username: ");
            Settings.Default.Username = System.Console.ReadLine();
            System.Console.Write("Your Paswoord: ");
            Settings.Default.Password = System.Console.ReadLine();
            Settings.Default.Save();

            System.Console.WriteLine("You can change your settings at any time by" +
                                    "running this program with the reset attribute.");
        }
	}
}
