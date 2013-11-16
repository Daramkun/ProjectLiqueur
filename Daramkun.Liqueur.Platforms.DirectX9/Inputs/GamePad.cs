using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs
{
	public class GamePad : GamePadDevice
	{
		SharpDX.DirectInput.DirectInput dInput;
		SharpDX.DirectInput.Joystick device;

		public override bool IsSupport { get { return true; } }
		public override bool IsSupportVibration { get { return false; } }

		public GamePad ( IWindow window )
		{
			dInput = new SharpDX.DirectInput.DirectInput ();

			Guid gamePadGuid = Guid.Empty;
			foreach ( var deviceInstance in dInput.GetDevices ( SharpDX.DirectInput.DeviceType.Gamepad,
				SharpDX.DirectInput.DeviceEnumerationFlags.AllDevices ) )
				gamePadGuid = deviceInstance.InstanceGuid;
			if ( gamePadGuid != Guid.Empty )
			{
				device = new SharpDX.DirectInput.Joystick ( dInput, gamePadGuid );
				device.SetCooperativeLevel ( ( window.Handle as Form ).Handle,
					SharpDX.DirectInput.CooperativeLevel.Background | SharpDX.DirectInput.CooperativeLevel.NonExclusive );
				device.Properties.BufferSize = 128;
				device.Acquire ();
			}
		}

		protected override void Dispose ( bool isDisposing )
		{
			device.Unacquire ();
			device.Dispose ();
			base.Dispose ( isDisposing );
		}

		public override bool IsConnectedPlayer ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			if ( device == null ) return false;
			if ( playerIndex != PlayerIndex.Player1 ) return false;
			return true;
		}

		protected override GamePadState GenerateState ( PlayerIndex playerIndex )
		{
			device.Poll ();
			var state = device.GetCurrentState ();
			GamePadButton pressedButtons = GamePadButton.None;
			if ( state.Buttons [ 0 ] ) pressedButtons |= GamePadButton.A;
			if ( state.Buttons [ 1 ] ) pressedButtons |= GamePadButton.B;
			if ( state.Buttons [ 2 ] ) pressedButtons |= GamePadButton.X;
			if ( state.Buttons [ 3 ] ) pressedButtons |= GamePadButton.Y;
			if ( state.Buttons [ 4 ] ) pressedButtons |= GamePadButton.LeftBumper;
			if ( state.Buttons [ 5 ] ) pressedButtons |= GamePadButton.RightBumper;
			if ( state.Buttons [ 6 ] ) pressedButtons |= GamePadButton.Back;
			if ( state.Buttons [ 7 ] ) pressedButtons |= GamePadButton.Start;
			if ( state.Buttons [ 8 ] ) pressedButtons |= GamePadButton.LeftThumbStick;
			if ( state.Buttons [ 9 ] ) pressedButtons |= GamePadButton.RightThumbStick;

			if ( state.PointOfViewControllers [ 0 ] == 0 ) pressedButtons |= GamePadButton.DPadUp;
			if ( state.PointOfViewControllers [ 0 ] == 9000 ) pressedButtons |= GamePadButton.DPadRight;
			if ( state.PointOfViewControllers [ 0 ] == 13500 ) pressedButtons |= GamePadButton.DPadDown;
			if ( state.PointOfViewControllers [ 0 ] == 22500 ) pressedButtons |= GamePadButton.DPadLeft;

			if ( state.PointOfViewControllers [ 0 ] == 4500 ) pressedButtons |= GamePadButton.DPadUp | GamePadButton.DPadRight;
			if ( state.PointOfViewControllers [ 0 ] == 13500 ) pressedButtons |= GamePadButton.DPadDown | GamePadButton.DPadRight;
			if ( state.PointOfViewControllers [ 0 ] == 22500 ) pressedButtons |= GamePadButton.DPadDown | GamePadButton.DPadLeft;
			if ( state.PointOfViewControllers [ 0 ] == 31500 ) pressedButtons |= GamePadButton.DPadUp | GamePadButton.DPadLeft;

			GamePadState gamePadState = new GamePadState ( new Vector2 ( state.X / 32767.0f, state.Y / 32767.0f ),
				new Vector2 ( state.RotationX / 32767.0f, state.RotationY / 32767.0f ),
				( state.Z - 32767 ) / 32767.0f, ( 32767 - state.Z ) / 32767.0f, pressedButtons );
			return gamePadState;
		}

		public override void Vibrate ( PlayerIndex playerIndex, float leftMotorSpeed, float rightMotorSpeed )
		{

		}
	}
}
