using System;
using System.Collections.Generic;
using System.Text;

namespace usrp_e310_cal
{
	class AssemblyVersion
	{
		public string Get => GetType().Assembly.GetName().Version.ToString();
	}
}
