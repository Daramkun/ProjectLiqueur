using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Inputs.State
{
	public struct MouseState
	{
		public Vector2 Position { get; private set; }
		public MouseButton MouseButtons { get; private set; }
		public float Wheel { get; private set; }

		public MouseState ( Vector2 position, float wheel, params MouseButton [] buttons )
			: this ()
		{
			Position = position;
			foreach ( MouseButton button in buttons )
				MouseButtons |= button;
			Wheel = wheel;
		}

		public bool IsButtonDown ( MouseButton button )
		{
			return ( MouseButtons & button ) != 0;
		}

		public bool IsButtonUp ( MouseButton button )
		{
			return ( MouseButtons & button ) == 0;
		}
	}
}
