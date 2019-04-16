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
			"Mandatory Options:",
			"-u:linxuUsername -p:password -com:number",
			"Optional",
			"-l:path to write calibrate csv results (default is ../results/)"
		};


	}
}
