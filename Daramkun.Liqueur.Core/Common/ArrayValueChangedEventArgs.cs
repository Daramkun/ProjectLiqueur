using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	public class ArrayValueChangedEventArgs : EventArgs
	{
		public int Index { get; private set; }

		public ArrayValueChangedEventArgs ( int index )
		{
			Index = index;
		}
	}
}
