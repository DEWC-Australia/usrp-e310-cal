using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace usrp_e310_cal
{

	class SerialPortFacade
	{
		private SerialPort _serialPort;
		private bool Configured = false;

		public SerialPortFacade(string comPort, int baudRate, Parity parity, int dataBit, StopBits stopBits, Handshake handshaking)
		{
			_serialPort = new SerialPort();
			Configured = Configure(comPort, baudRate, parity, dataBit, stopBits, handshaking);
		}

		public bool Open()
		{
			if (this.Configured)
			{
				_serialPort.DataReceived += DataReceivedHandler;

				_serialPort.Open();
			}
			else
			{
				if (Constants.VerboseMode)
					Console.WriteLine("Serial Port is not configured and cannot be opened.");
			}
 
			return _serialPort.IsOpen;
		}

		public bool Close()
		{
			_serialPort.Close();

			return _serialPort.IsOpen;
		}

		public bool Write(string data)
		{
			try
			{
				_serialPort.WriteLine(data);
				return true;
			}
			catch
			{
				return false;
			}
			
		}


		private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort sp = (SerialPort)sender;
			string indata = sp.ReadExisting();
			ProcessReceivedData(indata);
		}

		private void ProcessReceivedData(string data)
		{
			if (Constants.VerboseMode)
			{
				Console.WriteLine("Data Received:");
				Console.WriteLine(data);
			}
			// this maybe an composed class which uniquely handles the received data to make the serial port generic

		}

		private bool Configure(string comPort, int baudRate, Parity parity, int dataBit, StopBits stopBits, Handshake handshaking)
		{
			bool available = SetPortName(comPort);
			if (!available)
			{
				if (Constants.VerboseMode)
					Console.WriteLine("Comm Port Not Avaliable.");
			}

			SetPortBaudRate(baudRate);
			SetPortParity(parity);
			SetPortDataBits(dataBit);
			SetPortStopBits(stopBits);
			SetPortHandshake(handshaking);

			// Set the read/write timeouts
			_serialPort.ReadTimeout = 500;
			_serialPort.WriteTimeout = 500;

			return available;
		}

		private bool SetPortName(string portName)
		{

			if(Constants.VerboseMode)
				Console.WriteLine($"Testing for port: {portName}");

			bool portAvailable = false;
			foreach (string port in SerialPort.GetPortNames())
			{
				if (port.ToLower().Equals(port.ToLower()))
				{
					break;
				}
			}

			if (portAvailable || !(portName.ToLower()).StartsWith("com"))
			{

				_serialPort.PortName = portName;

				if (Constants.VerboseMode)
					Console.WriteLine($"Port: {portName} available and setup.");
			}

			return portAvailable;

		}

		// Display BaudRate values and prompt user to enter a value.
		private void SetPortBaudRate(int portBaudRate)
		{
			_serialPort.BaudRate = portBaudRate;

			if (Constants.VerboseMode)
				Console.WriteLine($"Port Rate Set To: {portBaudRate}.");
		}

		// Display PortParity values and prompt user to enter a value.
		private void SetPortParity(Parity portParity)
		{
			_serialPort.Parity = portParity;

			if (Constants.VerboseMode)
				Console.WriteLine($"Port Parity Set To {portParity.ToString()}.");
		}

		// Display DataBits values and prompt user to enter a value.
		private void SetPortDataBits(int portDataBits)
		{
			_serialPort.DataBits = portDataBits;

			if (Constants.VerboseMode)
				Console.WriteLine($"Port Data Bits Set To {portDataBits}.");

		}

		// Display StopBits values and prompt user to enter a value.
		private void SetPortStopBits(StopBits portStopBits)
		{
			_serialPort.StopBits = portStopBits;

			if (Constants.VerboseMode)
				Console.WriteLine($"Port Stop Bits Set To {portStopBits.ToString()}.");
		}
		private void SetPortHandshake(Handshake portHandshake)
		{
			_serialPort.Handshake = portHandshake;

			if (Constants.VerboseMode)
				Console.WriteLine($"Port Stop Bits Set To {portHandshake.ToString()}.");
		}
	}
}
