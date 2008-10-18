using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Management;
using System.Diagnostics;
using System.Text.RegularExpressions;

using UHasseltWiFi.Properties;

namespace UHasseltWiFi
{
	class Program
	{
		static void Main(string[] args)
		{
            //* for testing purposes, will be removed later --sv
            String smac, sip;
            GetHostInfo_experimental(out sip, out smac);
            Console.WriteLine("MAC: {0}\nIP: {1}", smac, sip);
            Environment.Exit(-1);
            //*/
            
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
            String error = "Error while searching for XXX address. " +
                    "Make sure you are connected to an access point.";
            String mac = GetWirelessMAC();
            if (mac == "")
            {
                System.Console.WriteLine(error.Replace("XXX", "MAC"));
                Environment.Exit(-1);
            }
            url.Append(mac);
            
            url.Append(@"&ip=");
            String ip = GetLocalIpAddress();
            if (ip == "")
            {
                System.Console.WriteLine(error.Replace("XXX", "IP"));
                Environment.Exit(-1);
            }
            url.Append(ip);
			url.Append(@"&essid=UHasselt-Public");

			System.Console.WriteLine("Logging in user: " + Settings.Default.Username);
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
				System.Console.WriteLine("Connection could not be made!");
                Environment.Exit(-1);
			}

            // test if we can reach Google
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.google.com/");
                req.GetResponse();
            }
            catch (System.Net.WebException)
            {
                System.Console.WriteLine("Something went wrong, I couldn't reach Google. " +
                    "Check ip ({0}) and MAC ({1}) address.", ip, mac);
                Environment.Exit(-1);
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

        // this function isn't tested
        // I'm not sure if it will work.
        // But hey, you can't know untill you test it :)
        private static void GetHostInfo_experimental(out String ip, out String mac)
        {
            ip = mac = "";
            try
            { 
                //HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.google.com");
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://lumumba.uhasselt.be/~sv/test_redirect/redirect.php");
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Uri uri = response.ResponseUri;
                Debug.WriteLine(uri.OriginalString);

                Regex mac_filter = new Regex("[?|&]mac=([^&]*)&?", RegexOptions.Compiled);
                MatchCollection mac_matches = mac_filter.Matches(uri.Query);
                if (mac_matches.Count > 0)
                {
                    mac = mac_matches[0].Groups[1].Value;
                }
                Regex ip_filter = new Regex("[?|&]ip=([^&]*)&?", RegexOptions.Compiled);
                MatchCollection ip_matches = ip_filter.Matches(uri.Query);
                if (ip_matches.Count > 0)
                {
                    ip = ip_matches[0].Groups[1].Value;
                }
            }
            catch (System.Net.WebException)
            {
                System.Console.WriteLine("Connection could not be made!");
            }
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
            System.Console.Write("Your password: ");
            Settings.Default.Password = System.Console.ReadLine();
            Settings.Default.Save();

            System.Console.WriteLine("You can change your settings at any time by " +
                                    "running this program with the reset attribute.");
        }
	}
}
