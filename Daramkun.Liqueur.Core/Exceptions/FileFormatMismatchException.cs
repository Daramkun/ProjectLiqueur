using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class FileFormatMismatchException : Exception
	{
		public FileFormatMismatchException ()
			: base ()
		{

		}

		public FileFormatMismatchException ( string message )
			: base ( message )
		{

		}
	}
}
