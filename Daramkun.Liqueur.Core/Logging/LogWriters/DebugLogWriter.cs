using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Logging.LogWriters
{
	public class DebugLogWriter : ILogWriter
	{
		public void WriteLog ( string message )
		{
			Debug.WriteLine ( message );
		}
	}
}
