using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Logging.LogWriters
{
	public class ConsoleLogWriter : ILogWriter
	{
		public void WriteLog ( string message )
		{
			Console.WriteLine ( message );
		}
	}
}
