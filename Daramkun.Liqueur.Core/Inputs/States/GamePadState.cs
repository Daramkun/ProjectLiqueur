using Daramkun.Liqueur.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.States
{
	public struct GamePadState
	{
		GamePadButton buttons;
		Vector2 leftThumbstick, rightThumbstick;
		float leftTrigger, rightTrigger;

		public GamePadButton PressedButtons { get { return buttons; } }
		public Vector2 LeftThumbstick { get { return leftThumbstick; } }
		public Vector2 RightThumbstick { get { return rightThumbstick; } }
		public float LeftTrigger { get { return leftTrigger; } }
		public float RightTrigger { get { return rightTrigger; } }

		public bool IsButtonDown ( GamePadButton button )
		{
			return ( buttons & button ) != 0;
		}

		public bool IsButtonUp ( GamePadButton button )
		{
			return !IsButtonDown ( button );
		}

		internal GamePadState ( Vector2 leftThumbstick, Vector2 rightThumbstick,
			float leftTrigger, float rightTrigger, params GamePadButton [] pressedButtons )
		{
			this.leftThumbstick = leftThumbstick;
			this.rightThumbstick = rightThumbstick;
			this.leftTrigger = leftTrigger;
			this.rightTrigger = rightTrigger;
			this.buttons = GamePadButton.None;
			foreach ( GamePadButton button in pressedButtons )
				this.buttons |= button;
		}
	}
}
