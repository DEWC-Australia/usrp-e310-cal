using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace usrp_e310_cal
{
	public class Constants
	{
		public const string DefaultReportPath = "../reports/";

		public const bool VerboseMode = true;

		public class CommPortSettings
		{
			public const int baud = 115200;
			public const Parity parity = Parity.None;
			public const int dataBits = 8;
			public const StopBits stopBits = StopBits.One;
			public const Handshake handshake = Handshake.None;
		}

		public static readonly string[] help = {
			"USRP E310 Calibration Tool",
			"Mandatory Options (must be in this order):",
			"-u:linxuUsername -p:password -com:number",
			"Optional (after Mandatory Options)",
			"-l:path to write calibrate csv results (default is ../results/)"
		};

		public class UHD_Cals
		{
			public const string uhd_cal_rx_iq_balance = "/usr/bin/uhd_cal_rx_iq_balance --verbose";
			public const string uhd_cal_tx_iq_balance = "/usr/bin/uhd_cal_tx_iq_balance --verbose";
			public const string uhd_cal_tx_dc_offset = "/usr/bin/uhd_cal_tx_dc_offset --verbose";
		}

		public class UHD_Cal_Reports
		{
			public const string uhd_cal_rx_iq_balance = "/usr/bin/uhd_cal_rx_iq_balance --verbose";
			public const string uhd_cal_tx_iq_balance = "/usr/bin/uhd_cal_tx_iq_balance --verbose";
			public const string uhd_cal_tx_dc_offset = "/usr/bin/uhd_cal_tx_dc_offset --verbose";
		}

		public class UHD_Cal_Msg
		{
			public const string Booted = "ettus-e3xx-sg3 login:";
			public const string Password = "ettus-e3xx-sg3 password:";
			public const string LoggedIn = "root@ettus-e3xx-sg3:";
			public const string BoardError = "Error: This board does not have the CAL antenna option, cannot self-calibrate.";
			public const string UnknownError = "Error:";
			public const string Success = "\r\nroot@ettus-e3xx-sg3:~# ";
		}

		public class ReceiverStates
		{
			public const int StartUp = -1;
			public const int Booted = 0;
			public const int UsernameEntered = 1;
			public const int LoggedIn = 2;
			public const int RxIqCalComplete = 3;
			public const int TxIqCalComplete = 4;
			public const int TxDcOffsetCalComplete = 5;
			public const int CalsComplete = 6;
			public const int RxIqReportDownloaded = 7;
			public const int TxIqReportDownloaded = 8;
			public const int TxDcOffsetReportDownloaded = 9;


		}


	}
}
