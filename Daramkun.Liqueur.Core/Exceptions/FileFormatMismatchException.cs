using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class FileFormatMismatchException : CommonException
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
