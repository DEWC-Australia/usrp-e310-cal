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

			if(serialPort != null && serialPort.IsOpen)
			{
				serialPort.Close();
			}

			if(!invalid)
				Console.WriteLine("Error detected. For help with command line inputs use --help");

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
				Console.WriteLine($"Application: {new AssemblyDetails().GetName}");
				Console.WriteLine($"Version: {new AssemblyDetails().GetVersion}");

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

				int comport = 0;

				bool isInt = int.TryParse(GetName(args[2]), out comport);

				if (!isInt)
				{
					Console.WriteLine("COM Port must be an integer.");
					return status;
				}
				

				serialPort = new SerialPortFacade(
				comport,
				Constants.CommPortSettings.baud,
				Constants.CommPortSettings.parity,
				Constants.CommPortSettings.dataBits,
				Constants.CommPortSettings.stopBits,
				Constants.CommPortSettings.handshake,
				new UsrpE310Receiver());

				if (!serialPort.Configured)
					return status;

				if (!serialPort.Open())
					return status;

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

			if (serialPort.Receiver.state == 
				Constants.ReceiverStates.LoggedIn)
				return valid;

			if(serialPort.Receiver.state ==
				Constants.ReceiverStates.Booted)
			{
				valid = serialPort.Write(username);

				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.Booted);
			}

			if(serialPort.Receiver.state == 
				Constants.ReceiverStates.UsernameEntered)
			{
				valid = serialPort.Write(password);

				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.UsernameEntered) ;
			}
				

			return valid && 
				(serialPort.Receiver.state ==
				Constants.ReceiverStates.LoggedIn);
		}

		static private bool HandleCal(SerialPortFacade serialPort)
		{
			bool valid = false;
			if (serialPort.Receiver.state ==
				Constants.ReceiverStates.LoggedIn)
			{
				valid = serialPort.Write(Constants.UHD_Cals.uhd_cal_rx_iq_balance);
				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.LoggedIn) ;
			}

			if (serialPort.Receiver.state ==
				Constants.ReceiverStates.RxIqCalComplete)
			{
				valid = serialPort.Write(Constants.UHD_Cals.uhd_cal_tx_iq_balance);
				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.RxIqCalComplete) ;
			}

			if (serialPort.Receiver.state ==
				Constants.ReceiverStates.TxIqCalComplete)
			{
				valid = serialPort.Write(Constants.UHD_Cals.uhd_cal_tx_dc_offset);
				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.TxIqCalComplete) ;
			}

			return valid &&
				(serialPort.Receiver.state ==
					Constants.ReceiverStates.TxDcOffsetCalComplete);
		}

		static private bool GetReports(SerialPortFacade serialPort, string outputPath)
		{
			bool valid = false;

			if (serialPort.Receiver.state ==
				Constants.ReceiverStates.TxDcOffsetCalComplete)
			{
				// maybe get all reports in a list, then iterate through the list
				valid = serialPort.Write(Constants.UHD_Cal_Reports.reportDir);
				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.TxDcOffsetCalComplete) ;
			}

			if (serialPort.Receiver.state ==
				Constants.ReceiverStates.RxIqReportDownloaded)
			{
				valid = serialPort.Write(Constants.UHD_Cal_Reports.reportDir);
				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.RxIqReportDownloaded) ;
			}

			if (serialPort.Receiver.state ==
				Constants.ReceiverStates.TxIqReportDownloaded)
			{
				valid = serialPort.Write(Constants.UHD_Cal_Reports.reportDir);
				while (serialPort.Receiver.state ==
					Constants.ReceiverStates.TxIqReportDownloaded) ;
			}

			return valid &&
				(serialPort.Receiver.state ==
					Constants.ReceiverStates.TxDcOffsetReportDownloaded);
		}
	}
}
