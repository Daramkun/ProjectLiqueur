using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.State
{
	public struct KeyboardState
	{
		public KeyboardKey [] PressedKeys { get; private set; }

		public KeyboardState ( params KeyboardKey [] keys )
			: this ()
		{
			PressedKeys = new KeyboardKey [ keys.Length ];
			for ( int i = 0; i < keys.Length; i++ )
				PressedKeys [ i ] = keys [ i ];
		}

		public bool IsKeyDown ( KeyboardKey key )
		{
			if ( PressedKeys == null )
				return false;
			return PressedKeys.Contains ( key );
		}

		public bool IsKeyUp ( KeyboardKey key )
		{
			if ( PressedKeys == null )
				return false;
			return !PressedKeys.Contains ( key );
		}
	}
}
