using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.States
{
	public struct KeyboardState
	{
		KeyboardKey [] pressedKeys;

		public KeyboardKey [] PressedKeys { get { return pressedKeys; } }

		public bool IsKeyDown ( KeyboardKey key )
		{
			if ( pressedKeys.Contains<KeyboardKey> ( key ) ) return true;
			else return false;
		}

		public bool IsKeyUp ( KeyboardKey key )
		{
			return !IsKeyDown ( key );
		}

		internal KeyboardState ( params KeyboardKey [] pressedKeys )
		{
			this.pressedKeys = pressedKeys;
		}
	}
}
