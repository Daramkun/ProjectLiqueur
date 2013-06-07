using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class VersionMismatchException : CommonException
	{
		public VersionMismatchException ()
			: base ()
		{

		}

		public VersionMismatchException ( string message )
			: base ( message )
		{

		}
	}
}
