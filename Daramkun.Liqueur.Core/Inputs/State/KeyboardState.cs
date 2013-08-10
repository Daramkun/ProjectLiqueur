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
			PressedKeys = keys.Clone () as KeyboardKey [];
		}

		public bool IsKeyDown ( KeyboardKey key )
		{
			return PressedKeys.Contains ( key );
		}

		public bool IsKeyUp ( KeyboardKey key )
		{
			return !PressedKeys.Contains ( key );
		}
	}
}
