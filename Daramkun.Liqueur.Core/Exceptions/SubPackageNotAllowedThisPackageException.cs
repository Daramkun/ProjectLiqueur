using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class SubPackageNotAllowedThisPackageException : CommonException
	{
		public SubPackageNotAllowedThisPackageException ()
			: base ()
		{

		}

		public SubPackageNotAllowedThisPackageException ( string message )
			: base ( message )
		{

		}
	}
}
