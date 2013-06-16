using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Platforms;
using SharpDX.XInput;

namespace Daramkun.Liqueur.Inputs.Devices
{
	public class GamePad : RawGamePad
	{
		Controller controller;

		public override bool IsConnected { get { return controller.IsConnected; } }
		public override bool IsSupportVibrator { get { return true; } }

		public GamePad ( IWindow window, PlayerIndex playerIndex )
			: base ( window, playerIndex )
		{
			controller = new Controller ( ( UserIndex ) playerIndex );
		}

		public override States.GamePadState GetState ()
		{
			Gamepad gamepad = controller.GetState ().Gamepad;
			
			LeftThumbstick = new Vector2 ( gamepad.LeftThumbX / 32767.0f, gamepad.LeftThumbY / 32767.0f );
			RightThumbstick = new Vector2 ( gamepad.RightThumbX / 32767.0f, gamepad.RightThumbY / 32767.0f );
			Trigger = new Vector2 ( gamepad.LeftTrigger / 255.0f, gamepad.RightTrigger / 255.0f );



			return base.GetState ();
		}

		public override void Vibrate ( float leftSpeed, float rightSpeed )
		{
			controller.SetVibration ( new Vibration ()
			{
				LeftMotorSpeed = ( short ) ( leftSpeed * 255 ),
				RightMotorSpeed = ( short ) ( rightSpeed * 255 )
			} );
		}
	}
}
