using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Logging.LogWriters
{
	public class StreamLogWriter : ILogWriter
	{
		public Stream BaseStream { get; set; }

		public StreamLogWriter ( Stream stream )
		{
			BaseStream = stream;
		}

		public void WriteLog ( string message )
		{
			if ( BaseStream != null )
			{
				byte [] messageData = Encoding.UTF8.GetBytes ( message + "\n" );
				BaseStream.Write ( messageData, 0, messageData.Length );
			}
		}
	}
}
