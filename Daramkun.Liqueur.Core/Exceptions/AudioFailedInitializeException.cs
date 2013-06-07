using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class AudioFailedInitializeException : CommonException
	{
		public AudioFailedInitializeException ()
			: base ()
		{

		}

		public AudioFailedInitializeException ( string message )
			: base ( message )
		{

		}
	}
}
