using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class ShaderCompilationException : CommonException
	{
		public ShaderCompilationException ()
			: base ()
		{

		}

		public ShaderCompilationException ( string message )
			: base ( message )
		{

		}
	}
}
