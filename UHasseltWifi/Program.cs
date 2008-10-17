using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using UHasseltWifi.Properties;

namespace UHasseltWifi
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();

            StringBuilder postData = new StringBuilder("cmd=authenticate");
            postData.Append(@"&user=");
            postData.Append(settings.Username);
            postData.Append(@"&password=");
            postData.Append(settings.Password);
            byte[] data = (new ASCIIEncoding()).GetBytes(postData.ToString());

            // https://securelogin.arubanetworks.com/cgi-bin/login?cmd=login&mac=00:1f:e1:83:b7:d6&ip=10.5.253.216&essid=UHasselt-Public
            StringBuilder url = new StringBuilder(@"https://securelogin.arubanetworks.com/cgi-bin/login?cmd=login&mac");
            // url.Append(settings.MacAddress);
            url.Append(GetWirelessMAC());
            url.Append(@"&ip=");
            url.Append(GetLocalIpAddress());
            url.Append(@"&essid=UHasselt-Public");

			System.Console.WriteLine("Loging in user: " + settings.Username);
			System.Console.WriteLine("Http request url: " + url);

            // Prepare web request...
			// ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
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
            IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());
            string lowRange = "10.5.0.0";
            string highRange = "10.5.255.255";
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

        private static string GetWirelessMAC()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty) // only return MAC Address from first card
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }
            return MACAddress;
        }
    }
}
