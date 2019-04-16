using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace usrp_e310_cal
{

	class SerialPortFacade
	{
		public SerialPortReceiver Receiver { get; set; }

		private SerialPort _serialPort;
		public bool Configured { get; private set; }

		public SerialPortFacade(int comPort, int baudRate, Parity parity, int dataBit, StopBits stopBits, Handshake handshaking, SerialPortReceiver receiver)
		{
			Receiver = receiver;
			_serialPort = new SerialPort();
			Configured = false;
			Configured = Configure(comPort, baudRate, parity, dataBit, stopBits, handshaking);
		}

		public bool Open()
		{
			if (this.Configured)
			{
				_serialPort.DataReceived += DataReceivedHandler;

				try
				{
					_serialPort.Open();
				}catch(Exception ex)
				{
					if (Constants.VerboseMode)
						Console.WriteLine(ex.Message);

					return false;
				}
				
			}
			else
			{
				if (Constants.VerboseMode)
					Console.WriteLine("Serial Port is not configured and cannot be opened.");
			}

			FindReceiverState();

			return _serialPort.IsOpen;
		}

		public void FindReceiverState()
		{
			if (_serialPort.IsOpen)
				Write("");

			while (Receiver.state == Constants.ReceiverStates.StartUp) ;
		}

		public bool IsOpen => _serialPort.IsOpen;

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
			Receiver.Process(indata);
			if (Constants.VerboseMode)
			{
				Console.WriteLine("Data Received:");
				Console.WriteLine(indata);
			}
		}


		private bool Configure(int comPort, int baudRate, Parity parity, int dataBit, StopBits stopBits, Handshake handshaking)
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

		private bool SetPortName(int port)
		{

			if(Constants.VerboseMode)
				Console.WriteLine($"Testing for port: COM{port}");

			bool portAvailable = false;
			foreach (string portName in SerialPort.GetPortNames())
			{
				if (portName.ToUpper().Equals($"COM{port}"))
				{
					portAvailable = true;
					break;
				}
			}

			if (portAvailable)
			{

				_serialPort.PortName = $"COM{port}";

				if (Constants.VerboseMode)
					Console.WriteLine($"Port: COM{port} available and setup.");
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
