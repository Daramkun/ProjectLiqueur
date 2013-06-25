using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Walnut.Exceptions
{
	public class GameMismatchException : CommonException
	{
		public GameMismatchException ()
			: base ()
		{

		}

		public GameMismatchException ( string message )
			: base ( message )
		{

		}
	}
}
