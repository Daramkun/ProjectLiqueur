using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.States
{
	public struct MouseState
	{
		MouseButton mouseButton;
		float x, y;
		float wheel;

		public MouseButton MouseButton { get { return mouseButton; } }
		public float X { get { return x; } }
		public float Y { get { return y; } }
		public float Wheel { get { return wheel; } }

		public bool IsButtonDown ( MouseButton button )
		{
			return ( mouseButton & button ) != 0;
		}

		public bool IsButtonUp ( MouseButton button )
		{
			return !IsButtonDown ( button );
		}

		internal MouseState ( MouseButton mouseButton, float x, float y, float wheel )
		{
			this.mouseButton = mouseButton;
			this.x = x;
			this.y = y;
			this.wheel = wheel;
		}
	}
}
