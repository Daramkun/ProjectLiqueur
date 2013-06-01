using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.States;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Daramkun.Liqueur.Platforms
{
	public class Window : IWindow, IFrameworkView
	{
		internal CoreWindow coreWindow;
		List<KeyboardKey> pressedKeys = new List<KeyboardKey> ();

		MouseButton mouseButton;
		Vector2 mousePosition = new Vector2 ();
		float mouseWheel;

		List<TouchPoint> touchPoints = new List<TouchPoint> ();

		SharpDX.XInput.Controller [] xboxController = new SharpDX.XInput.Controller [ 4 ];

		Windows.Devices.Sensors.Accelerometer accelerometer = Windows.Devices.Sensors.Accelerometer.GetDefault ();

		public Window ()
		{

		}

		#region IWindow
		public string Title { get { return null; } set { } }
		public Vector2 ClientSize
		{
			get
			{
				return new Vector2 ( ( float ) coreWindow.Bounds.Width,
					( float ) coreWindow.Bounds.Height );
			}
			set { }
		}
		public bool IsCursorVisible
		{
			get { return coreWindow.PointerCursor != null; }
			set
			{
				if ( value ) coreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Arrow, 0 );
				else coreWindow.PointerCursor = null;
			}
		}
		public bool IsResizable { get { return false; } set { } }

		public bool IsSupportKeyboard { get { return true; } }
		public bool IsSupportMouse { get { return true; } }
		public bool IsSupportTouch { get { return true; } }
		public bool IsSupportGamePad { get { return true; } }
		public bool IsSupportAccelerometer { get { return true; } }

		public bool IsConnectedGamePad ( PlayerIndex playerIndex )
		{
			return xboxController [ ( int ) playerIndex ].IsConnected;
		}

		public void Vibrate ( PlayerIndex playerIndex, float leftSpeed, float rightSpeed )
		{
			xboxController [ ( int ) playerIndex ].SetVibration ( new SharpDX.XInput.Vibration ()
			{
				LeftMotorSpeed = ( short ) leftSpeed,
				RightMotorSpeed = ( short ) rightSpeed,
			} );
		}
		/*
		public GamePadState GetGamePadState ( PlayerIndex playerIndex )
		{
			if ( !IsConnectedGamePad ( playerIndex ) ) return new GamePadState ();

			SharpDX.XInput.State state = xboxController [ ( int ) playerIndex ].GetState ();
			Vector2 leftTbumb = new Vector2 ( state.Gamepad.LeftThumbX / 32767.0f, state.Gamepad.LeftThumbY / 32767.0f ),
				rightThumb = new Vector2 ( state.Gamepad.RightThumbX / 32767.0f, state.Gamepad.RightThumbX / 32767.0f );
			SharpDX.XInput.GamepadButtonFlags buttons = state.Gamepad.Buttons;

			List<GamePadButton> pressedButtons = new List<GamePadButton> ();
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.A ) != 0 ) pressedButtons.Add ( GamePadButton.A );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.B ) != 0 ) pressedButtons.Add ( GamePadButton.B );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.X ) != 0 ) pressedButtons.Add ( GamePadButton.X );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.Y ) != 0 ) pressedButtons.Add ( GamePadButton.Y );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.Back ) != 0 ) pressedButtons.Add ( GamePadButton.Back );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.Start ) != 0 ) pressedButtons.Add ( GamePadButton.Start );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.DPadDown ) != 0 ) pressedButtons.Add ( GamePadButton.DPadDown );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.DPadUp ) != 0 ) pressedButtons.Add ( GamePadButton.DPadUp );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.DPadLeft ) != 0 ) pressedButtons.Add ( GamePadButton.DPadLeft );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.DPadRight ) != 0 ) pressedButtons.Add ( GamePadButton.DPadRight );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.LeftShoulder ) != 0 ) pressedButtons.Add ( GamePadButton.LeftBumper );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.RightShoulder ) != 0 ) pressedButtons.Add ( GamePadButton.RightBumper );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.LeftThumb ) != 0 ) pressedButtons.Add ( GamePadButton.LeftThumbStick );
			if ( ( buttons & SharpDX.XInput.GamepadButtonFlags.RightThumb ) != 0 ) pressedButtons.Add ( GamePadButton.RightThumbStick );

			return new GamePadState ( leftTbumb, rightThumb,
				state.Gamepad.LeftTrigger / 255.0f, state.Gamepad.RightTrigger / 255.0f,
				pressedButtons.ToArray () );
		}

		public AccelerometerState GetAccelerometerState ()
		{
			Windows.Devices.Sensors.AccelerometerReading reading = accelerometer.GetCurrentReading();
			return new AccelerometerState ( ( float ) reading.AccelerationX,
				( float ) reading.AccelerationY, ( float ) reading.AccelerationZ );
		}*/
		#endregion

		#region IFrameworkView
		public void Initialize ( CoreApplicationView applicationView )
		{

		}

		public void Load ( string entryPoint )
		{
			Game.Renderer = new Renderer ( this );
		}

		public event EventHandler UpdateFrame, RenderFrame;

		public void Run ()
		{
			coreWindow.Activate ();
			while ( true )
			{
				coreWindow.Dispatcher.ProcessEvents ( CoreProcessEventsOption.ProcessAllIfPresent );
				UpdateFrame ( this, EventArgs.Empty );
				RenderFrame ( this, EventArgs.Empty );
				touchPoints.Clear ();
			}
		}

		public void SetWindow ( CoreWindow window )
		{
			coreWindow = window;
			coreWindow.KeyDown += ( CoreWindow sender, KeyEventArgs e ) =>
			{
				KeyboardKey key = ConvertKeys ( e.VirtualKey );
				if ( key == KeyboardKey.Unknown ) return;
				if ( pressedKeys.Contains ( key ) ) return;
				pressedKeys.Add ( key );
			};
			coreWindow.KeyUp += ( CoreWindow sender, KeyEventArgs e ) =>
			{
				KeyboardKey key = ConvertKeys ( e.VirtualKey );
				if ( key == KeyboardKey.Unknown ) return;
				if ( pressedKeys.Contains ( key ) ) pressedKeys.Remove ( key );
			};
			coreWindow.PointerPressed += ( CoreWindow sender, PointerEventArgs e ) =>
			{
				if ( e.CurrentPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse )
				{
					if ( e.CurrentPoint.Properties.IsLeftButtonPressed )
						mouseButton |= MouseButton.Left;
					if ( e.CurrentPoint.Properties.IsRightButtonPressed )
						mouseButton |= MouseButton.Right;
					if ( e.CurrentPoint.Properties.IsMiddleButtonPressed )
						mouseButton |= MouseButton.Middle;
					mousePosition = new Vector2 ( ( int ) e.CurrentPoint.Position.X, ( int ) e.CurrentPoint.Position.Y );
				}
				else
				{
					TouchPoint touchPoint = new TouchPoint ( ( int ) e.CurrentPoint.PointerId, InputState.Pressed,
						( float ) e.CurrentPoint.Position.X, ( float ) e.CurrentPoint.Position.Y );
					touchPoints.Add ( touchPoint );
				}
			};
			coreWindow.PointerReleased += ( CoreWindow sender, PointerEventArgs e ) =>
			{
				if ( e.CurrentPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse )
				{
					if ( e.CurrentPoint.Properties.IsLeftButtonPressed )
						mouseButton &= MouseButton.Left;
					if ( e.CurrentPoint.Properties.IsRightButtonPressed )
						mouseButton &= MouseButton.Right;
					if ( e.CurrentPoint.Properties.IsMiddleButtonPressed )
						mouseButton &= MouseButton.Middle;
					mousePosition = new Vector2 ( ( int ) e.CurrentPoint.Position.X, ( int ) e.CurrentPoint.Position.Y );
				}
				else
				{
					TouchPoint touchPoint = new TouchPoint ( ( int ) e.CurrentPoint.PointerId, InputState.Released,
						( float ) e.CurrentPoint.Position.X, ( float ) e.CurrentPoint.Position.Y );
					touchPoints.Add ( touchPoint );
				}
			};
			coreWindow.PointerMoved += ( CoreWindow sender, PointerEventArgs e ) =>
			{
				if ( e.CurrentPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse )
				{
					mousePosition = new Vector2 ( ( int ) e.CurrentPoint.Position.X, ( int ) e.CurrentPoint.Position.Y );
				}
				else
				{
					TouchPoint touchPoint = new TouchPoint ( ( int ) e.CurrentPoint.PointerId, InputState.Moved,
						( float ) e.CurrentPoint.Position.X, ( float ) e.CurrentPoint.Position.Y );
					touchPoints.Add ( touchPoint );
				}
			};
			coreWindow.PointerWheelChanged += ( CoreWindow sender, PointerEventArgs e ) =>
			{
				if ( e.CurrentPoint.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse )
				{
					mouseWheel = e.CurrentPoint.Properties.MouseWheelDelta;
					mousePosition = new Vector2 ( ( int ) e.CurrentPoint.Position.X, ( int ) e.CurrentPoint.Position.Y );
				}
			};

			xboxController [ 0 ] = new SharpDX.XInput.Controller ( SharpDX.XInput.UserIndex.One );
			xboxController [ 1 ] = new SharpDX.XInput.Controller ( SharpDX.XInput.UserIndex.Two );
			xboxController [ 2 ] = new SharpDX.XInput.Controller ( SharpDX.XInput.UserIndex.Three );
			xboxController [ 3 ] = new SharpDX.XInput.Controller ( SharpDX.XInput.UserIndex.Four );
		}

		public void Uninitialize ()
		{

		}
		#endregion

		#region Converter
		private KeyboardKey ConvertKeys ( Windows.System.VirtualKey key )
		{
			switch ( key )
			{
				case Windows.System.VirtualKey.A: return KeyboardKey.A;
				case Windows.System.VirtualKey.B: return KeyboardKey.B;
				case Windows.System.VirtualKey.C: return KeyboardKey.C;
				case Windows.System.VirtualKey.D: return KeyboardKey.D;
				case Windows.System.VirtualKey.E: return KeyboardKey.E;
				case Windows.System.VirtualKey.F: return KeyboardKey.F;
				case Windows.System.VirtualKey.G: return KeyboardKey.G;
				case Windows.System.VirtualKey.H: return KeyboardKey.H;
				case Windows.System.VirtualKey.I: return KeyboardKey.I;
				case Windows.System.VirtualKey.J: return KeyboardKey.J;
				case Windows.System.VirtualKey.K: return KeyboardKey.K;
				case Windows.System.VirtualKey.L: return KeyboardKey.L;
				case Windows.System.VirtualKey.M: return KeyboardKey.M;
				case Windows.System.VirtualKey.N: return KeyboardKey.N;
				case Windows.System.VirtualKey.O: return KeyboardKey.O;
				case Windows.System.VirtualKey.P: return KeyboardKey.P;
				case Windows.System.VirtualKey.Q: return KeyboardKey.Q;
				case Windows.System.VirtualKey.R: return KeyboardKey.R;
				case Windows.System.VirtualKey.S: return KeyboardKey.S;
				case Windows.System.VirtualKey.T: return KeyboardKey.T;
				case Windows.System.VirtualKey.U: return KeyboardKey.U;
				case Windows.System.VirtualKey.V: return KeyboardKey.V;
				case Windows.System.VirtualKey.W: return KeyboardKey.W;
				case Windows.System.VirtualKey.X: return KeyboardKey.X;
				case Windows.System.VirtualKey.Y: return KeyboardKey.Y;
				case Windows.System.VirtualKey.Z: return KeyboardKey.Z;

				case Windows.System.VirtualKey.F1: return KeyboardKey.F1;
				case Windows.System.VirtualKey.F2: return KeyboardKey.F2;
				case Windows.System.VirtualKey.F3: return KeyboardKey.F3;
				case Windows.System.VirtualKey.F4: return KeyboardKey.F4;
				case Windows.System.VirtualKey.F5: return KeyboardKey.F5;
				case Windows.System.VirtualKey.F6: return KeyboardKey.F6;
				case Windows.System.VirtualKey.F7: return KeyboardKey.F7;
				case Windows.System.VirtualKey.F8: return KeyboardKey.F8;
				case Windows.System.VirtualKey.F9: return KeyboardKey.F9;
				case Windows.System.VirtualKey.F10: return KeyboardKey.F10;
				case Windows.System.VirtualKey.F11: return KeyboardKey.F11;
				case Windows.System.VirtualKey.F12: return KeyboardKey.F12;

				case Windows.System.VirtualKey.Number0: return KeyboardKey.D0;
				case Windows.System.VirtualKey.Number1: return KeyboardKey.D1;
				case Windows.System.VirtualKey.Number2: return KeyboardKey.D2;
				case Windows.System.VirtualKey.Number3: return KeyboardKey.D3;
				case Windows.System.VirtualKey.Number4: return KeyboardKey.D4;
				case Windows.System.VirtualKey.Number5: return KeyboardKey.D5;
				case Windows.System.VirtualKey.Number6: return KeyboardKey.D6;
				case Windows.System.VirtualKey.Number7: return KeyboardKey.D7;
				case Windows.System.VirtualKey.Number8: return KeyboardKey.D8;
				case Windows.System.VirtualKey.Number9: return KeyboardKey.D9;

				case Windows.System.VirtualKey.Back: return KeyboardKey.Backspace;
				case Windows.System.VirtualKey.Enter: return KeyboardKey.Return;
				case Windows.System.VirtualKey.Tab: return KeyboardKey.Tab;
				case Windows.System.VirtualKey.CapitalLock: return KeyboardKey.Capital;
				case Windows.System.VirtualKey.Escape: return KeyboardKey.Escape;
				case Windows.System.VirtualKey.Space: return KeyboardKey.Space;

				case Windows.System.VirtualKey.LeftControl: return KeyboardKey.LeftControl;
				case Windows.System.VirtualKey.RightControl: return KeyboardKey.RightControl;
				case Windows.System.VirtualKey.LeftMenu: return KeyboardKey.LeftAlt;
				case Windows.System.VirtualKey.RightMenu: return KeyboardKey.RightAlt;
				case Windows.System.VirtualKey.LeftShift: return KeyboardKey.LeftShift;
				case Windows.System.VirtualKey.RightShift: return KeyboardKey.RightShift;
				case Windows.System.VirtualKey.LeftWindows: return KeyboardKey.LeftWin;
				case Windows.System.VirtualKey.RightWindows: return KeyboardKey.RightWin;

				case Windows.System.VirtualKey.Left: return KeyboardKey.Left;
				case Windows.System.VirtualKey.Right: return KeyboardKey.Right;
				case Windows.System.VirtualKey.Up: return KeyboardKey.Up;
				case Windows.System.VirtualKey.Down: return KeyboardKey.Down;

				case Windows.System.VirtualKey.Insert: return KeyboardKey.Insert;
				case Windows.System.VirtualKey.Delete: return KeyboardKey.Delete;
				case Windows.System.VirtualKey.Home: return KeyboardKey.Home;
				case Windows.System.VirtualKey.End: return KeyboardKey.End;
				case Windows.System.VirtualKey.PageUp: return KeyboardKey.PageUp;
				case Windows.System.VirtualKey.PageDown: return KeyboardKey.PageDown;

				default:
					if ( key == ( Windows.System.VirtualKey ) 219 ) return KeyboardKey.LeftBracket;
					if ( key == ( Windows.System.VirtualKey ) 221 ) return KeyboardKey.RightBracket;
					if ( key == ( Windows.System.VirtualKey ) 220 ) return KeyboardKey.BackSlash;
					if ( key == ( Windows.System.VirtualKey ) 188 ) return KeyboardKey.Comma;
					if ( key == ( Windows.System.VirtualKey ) 190 ) return KeyboardKey.Period;
					if ( key == ( Windows.System.VirtualKey ) 189 ) return KeyboardKey.Subtract;
					if ( key == ( Windows.System.VirtualKey ) 187 ) return KeyboardKey.Equal;
					if ( key == ( Windows.System.VirtualKey ) 186 ) return KeyboardKey.Semicolon;
					if ( key == ( Windows.System.VirtualKey ) 191 ) return KeyboardKey.Slash;
					if ( key == ( Windows.System.VirtualKey ) 192 ) return KeyboardKey.Grave;
					if ( key == ( Windows.System.VirtualKey ) 222 ) return KeyboardKey.Apostrophe;
					
					break;
			}

			return KeyboardKey.Unknown;
		}
		#endregion
	}
}