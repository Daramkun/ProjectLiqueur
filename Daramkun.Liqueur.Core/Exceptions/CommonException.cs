using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class CommonException : Exception
	{
		public CommonException ()
			: base ()
		{

		}

		public CommonException ( string message )
			: base ( message )
		{

		}
	}
}
