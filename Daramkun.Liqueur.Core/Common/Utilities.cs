using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	public static class Utilities
	{
		public static bool IsSubtypeOf ( Type majorType, Type minorType )
		{
			if ( majorType == minorType || majorType.IsSubclassOf ( minorType ) )
				return true;
			else if ( minorType.IsInterface )
			{
				foreach ( Type type in majorType.GetInterfaces () )
					if ( type == minorType )
						return true;
			}
			return false;
		}
	}
}
