using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Platforms;
using SharpDX.XInput;

namespace Daramkun.Liqueur.Inputs
{
	public class GamePad : GamePadDevice
	{
		Controller [] controllers;

		public override bool IsSupport { get { return true; } }
		public override bool IsConnected { get { foreach ( Controller c in controllers ) if ( c.IsConnected ) return true; return false; } }

		public GamePad ( IWindow window )
		{
			controllers = new Controller [ 4 ];
			for ( int i = 0; i < 4; i++ ) controllers [ i ] = new Controller ( ( UserIndex ) i );
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				controllers = null;
			}
			base.Dispose ( isDisposing );
		}

		public override bool IsConnectedPlayer ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			return controllers [ ( int ) playerIndex ].IsConnected;
		}

		public override bool IsSupportVibration { get { return true; } }

		public override void Vibrate ( PlayerIndex playerIndex, float leftMotorSpeed, float rightMotorSpeed )
		{
			controllers [ ( int ) playerIndex ].SetVibration ( new Vibration ()
			{
				LeftMotorSpeed = ( ushort ) ( leftMotorSpeed * 65535 ),
				RightMotorSpeed = ( ushort ) ( rightMotorSpeed * 65535 )
			} );
		}

		protected override GamePadState GenerateState ( PlayerIndex playerIndex )
		{
			Gamepad gamepad = controllers [ ( int ) playerIndex ].GetState ().Gamepad;

			GamePadButton gamePadButton = GamePadButton.None;
			
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.DPadLeft ) ) gamePadButton |= GamePadButton.DPadLeft;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.DPadRight ) ) gamePadButton |= GamePadButton.DPadRight;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.DPadUp ) ) gamePadButton |= GamePadButton.DPadUp;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.DPadDown ) ) gamePadButton |= GamePadButton.DPadDown;

			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.A ) ) gamePadButton |= GamePadButton.A;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.B ) ) gamePadButton |= GamePadButton.B;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.X ) ) gamePadButton |= GamePadButton.X;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.Y ) ) gamePadButton |= GamePadButton.Y;

			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.Start ) ) gamePadButton |= GamePadButton.Start;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.Back ) ) gamePadButton |= GamePadButton.Back;

			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.LeftShoulder ) ) gamePadButton |= GamePadButton.LeftBumper;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.RightShoulder ) ) gamePadButton |= GamePadButton.RightBumper;

			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.LeftThumb ) ) gamePadButton |= GamePadButton.LeftThumbStick;
			if ( gamepad.Buttons.HasFlag ( GamepadButtonFlags.RightThumb ) ) gamePadButton |= GamePadButton.RightThumbStick;

			if ( gamepad.LeftTrigger > 255 / 2 ) gamePadButton |= GamePadButton.LeftTrigger;
			if ( gamepad.RightTrigger > 255 / 2 ) gamePadButton |= GamePadButton.RightTrigger;
			
			return new GamePadState (
				new Vector2 ( gamepad.LeftThumbX / 32767.0f, gamepad.LeftThumbY / 32767.0f ),
				new Vector2 ( gamepad.LeftThumbX / 32767.0f, gamepad.LeftThumbY / 32767.0f ),
				gamepad.LeftTrigger / 255.0f,
				gamepad.RightTrigger / 255.0f,
				gamePadButton
			);
		}
	}
}
