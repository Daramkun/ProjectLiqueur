using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Exceptions
{
	public class EffectConfigurationException : CommonException
	{
		public EffectConfigurationException ()
			: base ()
		{

		}

		public EffectConfigurationException ( string message )
			: base ( message )
		{

		}
	}
}
