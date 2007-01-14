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
			bool debug = false;
			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-debug":
						debug = true;
						break;
				}
			}

			Settings settings = new Settings();
			UHasseltWifiHttpLogin login = new UHasseltWifiHttpLogin(settings.MacAddress);
			login.Username = settings.Username;
			login.Password = settings.Password;

			if (debug)
			{
				Console.WriteLine(@"Logging in user: " + login.Username);
				if (login.Password != "")
					Console.WriteLine(@"Using password");
				Console.WriteLine(@"HTTP request url: " + login.URL);
			}
			
			login.MakeRequest();

			if (debug)
			{
				Console.WriteLine(@"Should be logged in now");
			}
		}
	}
}
