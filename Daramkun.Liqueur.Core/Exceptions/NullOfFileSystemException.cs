using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class NullOfFileSystemException : CommonException
	{
		public NullOfFileSystemException ()
			: base ()
		{

		}

		public NullOfFileSystemException ( string message )
			: base ( message )
		{

		}
	}
}
