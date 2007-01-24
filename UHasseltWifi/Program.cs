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
			Console.WriteLine(ProgramInfo());

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

			try
			{
				login.MakeRequest();

				if (debug)
				{
					Console.WriteLine(@"Should be logged in now");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(@"Error message: {0}", e.Message);

				if (debug)
				{
					Console.WriteLine(e.StackTrace);
				}
			}
		}

		private static string ProgramInfo()
		{
			return System.Reflection.Assembly.GetCallingAssembly().GetName().Name + @" v" + System.Reflection.Assembly.GetCallingAssembly().GetName().Version;
		}
	}
}
