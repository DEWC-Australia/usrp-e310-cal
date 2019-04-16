using System;

namespace usrp_e310_cal
{
	class Program
	{
		static void Main(string[] args)
		{
			SerialPortFacade serialPort = null;
			bool invalid = false;
			// output help
			if (args.Length == 1) {
				invalid = HandleSingleOption(args);
			}
			// username and password
			else if (args.Length == 3)
			{
				invalid = HandleThreeOptions(serialPort, args);
			}
			else if(args.Length == 4)
			{
				invalid = HandleFourOptions(serialPort, args);
			}

			if(!invalid)
				Console.WriteLine("Command line inputs not supported use --help for input format");

			return;
		}

		static private bool HandleSingleOption(string[] args)
		{
			if (args[0].StartsWith("-h"))
			{
				foreach (var line in Constants.help)
					Console.WriteLine(line);

				return true;
			}
			else if (args[0].StartsWith("-v"))
			{
				Console.WriteLine(new AssemblyVersion().Get);

				return true;
			}

			return false;
		}

		static private bool HandleThreeOptions(SerialPortFacade serialPort, string[] args)
		{
			bool status = false;
			if (args[0].StartsWith("-u") && args[1].StartsWith("-p") && args[2].StartsWith("-com"))
			{
				string username = GetName(args[0]);
				string password = GetName(args[1]);
				string comport = $"com{GetName(args[2])}";

				serialPort = new SerialPortFacade(
				comport,
				Constants.CommPortSettings.baud,
				Constants.CommPortSettings.parity,
				Constants.CommPortSettings.dataBits,
				Constants.CommPortSettings.stopBits,
				Constants.CommPortSettings.handshake);

				bool state = serialPort.Open();

				if (!state)
				{
					return false;
				}

				status = HandleLogin(serialPort, username, password);
				status = HandleCal(serialPort);

			}

			return status;
		}

		static private bool HandleFourOptions(SerialPortFacade serialPort, string[] args)
		{
			bool status = false;

			status = HandleThreeOptions(serialPort, args);

			var path = "";
			if (args[3].StartsWith("-l"))
			{
				path = GetName(args[2], Constants.DefaultReportPath);
			}

			status = GetReports(serialPort, path);

			return status;

		}

		static private string GetName(string arg, string defaultPath = "")
		{
			string result = defaultPath;
			var temp = arg.Split(":");

			if (temp.Length == 2)
				result = temp[1];

			return result;
		}

		static private bool HandleLogin(SerialPortFacade serialPort, string username, string password)
		{
			bool valid = false;
			valid = serialPort.Write(username);
			valid = serialPort.Write(password);
			return valid;
		}

		static private bool HandleCal(SerialPortFacade serialPort)
		{
			return false;
		}

		static private bool GetReports(SerialPortFacade serialPort, string outputPath)
		{
			return false;
		}
	}
}
