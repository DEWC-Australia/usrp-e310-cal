using System;
using System.Collections.Generic;
using System.Text;

namespace usrp_e310_cal
{
	abstract class SerialPortReceiver
	{
		public int state = -1;
		public abstract void Process(string data);
	}

	class UsrpE310Receiver : SerialPortReceiver
	{
		public List<string> Stack { get; private set; }
		// state 0 = start
		// state 1 = E310 booted and ready for login
		// state 1 = username entered correctly
		// state 2 = password entered correct and logged in (currently E310 is not asking for password)
		public UsrpE310Receiver()
		{
			Stack = new List<string>();
		}
		public override void Process(string data)
		{
			Stack.Add(data);

			state = StateMachine(state, data);
		}

		private int StateMachine(int currentState, string data)
		{
			switch (currentState)
			{
				
				case Constants.ReceiverStates.StartUp: // need to confirm the board is booted
					if (data.Contains(Constants.UHD_Cal_Msg.Booted))
						return Constants.ReceiverStates.Booted; // E310 Booted
					else if (data.Contains(Constants.UHD_Cal_Msg.LoggedIn))
						return Constants.ReceiverStates.LoggedIn; // E310 Logged in
					break;
				case Constants.ReceiverStates.Booted:
					if (data.Contains(Constants.UHD_Cal_Msg.Password))
						return Constants.ReceiverStates.UsernameEntered;
					else if (data.Contains(Constants.UHD_Cal_Msg.LoggedIn))
						return Constants.ReceiverStates.LoggedIn; // E310 logged in
					break;
				case Constants.ReceiverStates.UsernameEntered:
					if (data.Contains(Constants.UHD_Cal_Msg.LoggedIn)) // E310 currently doesn't require password
						return Constants.ReceiverStates.LoggedIn;
					break;
				//uhd_cal_rx_iq_balance
				case Constants.ReceiverStates.LoggedIn:
					if (data.Contains(Constants.UHD_Cal_Msg.BoardError)) // start running cals
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if (data.StartsWith(Constants.UHD_Cal_Msg.UnknownError)) // unrecognised Error
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if(data.Equals(Constants.UHD_Cal_Msg.Success))
					{
						return Constants.ReceiverStates.RxIqCalComplete;
					}
					break;
				case Constants.ReceiverStates.RxIqCalComplete:
					if (data.Contains(Constants.UHD_Cal_Msg.BoardError)) // start running cals
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if (data.StartsWith(Constants.UHD_Cal_Msg.UnknownError)) // unrecognised Error
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if (data.Equals(Constants.UHD_Cal_Msg.Success))
					{
						return Constants.ReceiverStates.TxIqCalComplete;
					}
					break;
				case Constants.ReceiverStates.TxIqCalComplete:
					if (data.Contains(Constants.UHD_Cal_Msg.BoardError)) // start running cals
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if (data.StartsWith(Constants.UHD_Cal_Msg.UnknownError)) // unrecognised Error
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if (data.Equals(Constants.UHD_Cal_Msg.Success))
					{
						return Constants.ReceiverStates.TxDcOffsetCalComplete;
					}
					break;
				case Constants.ReceiverStates.TxDcOffsetCalComplete:
					if (data.Contains(Constants.UHD_Cal_Msg.BoardError)) // start running cals
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if (data.StartsWith(Constants.UHD_Cal_Msg.UnknownError)) // unrecognised Error
					{
						Console.WriteLine(data);
						return Constants.ReceiverStates.LoggedIn; // return to start of cal
					}
					else if (data.Equals(Constants.UHD_Cal_Msg.Success))
					{
						return Constants.ReceiverStates.CalsComplete;
					}
					break;
				case Constants.ReceiverStates.CalsComplete:
					break;
				case Constants.ReceiverStates.RxIqReportDownloaded:
					// get reports
					//keep reading until reports are read
					// need to write these to file
					break;
				case Constants.ReceiverStates.TxIqReportDownloaded:
					// get reports
					//keep reading until reports are read
					// need to write these to file
					break;
				case Constants.ReceiverStates.TxDcOffsetReportDownloaded:
					// get reports
					//keep reading until reports are read
					// need to write these to file
					break;

				default:
					return -1;

			}

			return currentState;
		}
	}


}
