using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class PlatformNotSupportException : CommonException
	{
		public PlatformNotSupportException ()
			: base ()
		{

		}

		public PlatformNotSupportException ( string message )
			: base ( message )
		{

		}
	}
}
