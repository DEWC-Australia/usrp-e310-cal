using System;
using System.Collections.Generic;
using System.Text;

namespace usrp_e310_cal
{
	class AssemblyDetails
	{
		public string GetVersion => GetType().Assembly.GetName().Version.ToString();
		public string GetName => GetType().Assembly.GetName().Name.ToString();
	}
}
