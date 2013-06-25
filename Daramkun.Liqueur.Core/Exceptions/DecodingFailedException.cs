using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class DecodingFailedException : CommonException
	{
		public DecodingFailedException ()
			: base ()
		{

		}

		public DecodingFailedException ( string message )
			: base ( message )
		{

		}
	}
}
