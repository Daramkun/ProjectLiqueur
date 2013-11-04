using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class ScriptLanguageMismatchException : CommonException
	{
		public ScriptLanguageMismatchException ()
			: base ()
		{

		}

		public ScriptLanguageMismatchException ( string message )
			: base ( message )
		{

		}
	}
}
