using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.States
{
	public struct TouchPoint
	{
		int index;
		float x, y;
		InputState inputState;

		public int Index { get { return index; } }
		public float X { get { return x; } }
		public float Y { get { return y; } }
		public InputState InputState { get { return inputState; } }

		public TouchPoint ( int index, InputState inputState, float x, float y )
		{
			this.index = index;
			this.x = x;
			this.y = y;
			this.inputState = inputState;
		}
	}
}
