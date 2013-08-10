using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Inputs.State
{
	public struct GamePadState
	{
		public Vector2 LeftThumbstick { get; private set; }
		public Vector2 RightThumbstick { get; private set; }
		public float LeftTrigger { get; private set; }
		public float RightTrigger { get; private set; }
		public GamePadButton PressedButtons { get; private set; }

		public GamePadState ( Vector2 leftThumbstick, Vector2 rightThumbstick,
			float leftTrigger, float rightTrigger, params GamePadButton [] pressedButtons )
			: this ()
		{
			LeftThumbstick = leftThumbstick;
			RightThumbstick = rightThumbstick;
			LeftTrigger = leftTrigger;
			RightTrigger = rightTrigger;
			PressedButtons = GamePadButton.None;
			foreach ( GamePadButton button in pressedButtons )
				PressedButtons |= button;
		}

		public bool IsButtonDown ( GamePadButton button )
		{
			return ( PressedButtons & button ) != 0;
		}

		public bool IsButtonUp ( GamePadButton button )
		{
			return ( PressedButtons & button ) == 0;
		}
	}
}
