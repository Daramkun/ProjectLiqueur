using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.State
{
	public struct TouchState
	{
		public TouchPointer [] Pointers { get; private set; }

		public TouchState ( params TouchPointer [] pointers )
			: this ()
		{
			Pointers = new TouchPointer [ pointers.Length ];
			for ( int i = 0; i < pointers.Length; i++ )
				Pointers [ i ] = pointers [ i ];
		}
	}
}
