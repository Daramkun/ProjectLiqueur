using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class CompressionProcessException : CommonException
	{
		public CompressionProcessException ()
			: base ()
		{

		}

		public CompressionProcessException ( string message )
			: base ( message )
		{

		}
	}
}
