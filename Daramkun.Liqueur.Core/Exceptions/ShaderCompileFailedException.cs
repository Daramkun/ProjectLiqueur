using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class ShaderCompileFailedException : CommonException
	{
		public ShaderCompileFailedException ()
			: base ()
		{

		}

		public ShaderCompileFailedException ( string message )
			: base ( message )
		{

		}
	}
}
