using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;
using OpenTK;

namespace Daramkun.Liqueur.Inputs.Devices
{
	public class Keyboard : RawKeyboard
	{
		IWindow window;

		public override bool IsConnected { get { return true; } }

		public Keyboard ( IWindow window )
			: base ( window )
		{
			this.window = window;
			GameWindow otkWindow = window.Handle as GameWindow;
			otkWindow.Keyboard.KeyDown += KeyDownEvent;
			otkWindow.Keyboard.KeyUp += KeyUpEvent;
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				GameWindow otkWindow = window.Handle as GameWindow;
				otkWindow.Keyboard.KeyDown -= KeyDownEvent;
				otkWindow.Keyboard.KeyUp -= KeyUpEvent;
			}

			base.Dispose ( isDisposing );
		}

		private void KeyDownEvent ( object sender, OpenTK.Input.KeyboardKeyEventArgs e )
		{
			KeyboardKey key = ConvertKeys ( e.Key );
			if ( key == KeyboardKey.Unknown ) return;
			if ( PressedKeys.Contains ( key ) ) return;
			PressedKeys.Add ( key );
		}

		private void KeyUpEvent ( object sender, OpenTK.Input.KeyboardKeyEventArgs e )
		{
			KeyboardKey key = ConvertKeys ( e.Key );
			if ( key == KeyboardKey.Unknown ) return;
			if ( PressedKeys.Contains ( key ) ) PressedKeys.Remove ( key );
		}

		#region Converter
		private KeyboardKey ConvertKeys ( OpenTK.Input.Key key )
		{
			switch ( key )
			{
				case OpenTK.Input.Key.A: return KeyboardKey.A;
				case OpenTK.Input.Key.B: return KeyboardKey.B;
				case OpenTK.Input.Key.C: return KeyboardKey.C;
				case OpenTK.Input.Key.D: return KeyboardKey.D;
				case OpenTK.Input.Key.E: return KeyboardKey.E;
				case OpenTK.Input.Key.F: return KeyboardKey.F;
				case OpenTK.Input.Key.G: return KeyboardKey.G;
				case OpenTK.Input.Key.H: return KeyboardKey.H;
				case OpenTK.Input.Key.I: return KeyboardKey.I;
				case OpenTK.Input.Key.J: return KeyboardKey.J;
				case OpenTK.Input.Key.K: return KeyboardKey.K;
				case OpenTK.Input.Key.L: return KeyboardKey.L;
				case OpenTK.Input.Key.M: return KeyboardKey.M;
				case OpenTK.Input.Key.N: return KeyboardKey.N;
				case OpenTK.Input.Key.O: return KeyboardKey.O;
				case OpenTK.Input.Key.P: return KeyboardKey.P;
				case OpenTK.Input.Key.Q: return KeyboardKey.Q;
				case OpenTK.Input.Key.R: return KeyboardKey.R;
				case OpenTK.Input.Key.S: return KeyboardKey.S;
				case OpenTK.Input.Key.T: return KeyboardKey.T;
				case OpenTK.Input.Key.U: return KeyboardKey.U;
				case OpenTK.Input.Key.V: return KeyboardKey.V;
				case OpenTK.Input.Key.W: return KeyboardKey.W;
				case OpenTK.Input.Key.X: return KeyboardKey.X;
				case OpenTK.Input.Key.Y: return KeyboardKey.Y;
				case OpenTK.Input.Key.Z: return KeyboardKey.Z;

				case OpenTK.Input.Key.F1: return KeyboardKey.F1;
				case OpenTK.Input.Key.F2: return KeyboardKey.F2;
				case OpenTK.Input.Key.F3: return KeyboardKey.F3;
				case OpenTK.Input.Key.F4: return KeyboardKey.F4;
				case OpenTK.Input.Key.F5: return KeyboardKey.F5;
				case OpenTK.Input.Key.F6: return KeyboardKey.F6;
				case OpenTK.Input.Key.F7: return KeyboardKey.F7;
				case OpenTK.Input.Key.F8: return KeyboardKey.F8;
				case OpenTK.Input.Key.F9: return KeyboardKey.F9;
				case OpenTK.Input.Key.F10: return KeyboardKey.F10;
				case OpenTK.Input.Key.F11: return KeyboardKey.F11;
				case OpenTK.Input.Key.F12: return KeyboardKey.F12;

				case OpenTK.Input.Key.Number0: return KeyboardKey.D0;
				case OpenTK.Input.Key.Number1: return KeyboardKey.D1;
				case OpenTK.Input.Key.Number2: return KeyboardKey.D2;
				case OpenTK.Input.Key.Number3: return KeyboardKey.D3;
				case OpenTK.Input.Key.Number4: return KeyboardKey.D4;
				case OpenTK.Input.Key.Number5: return KeyboardKey.D5;
				case OpenTK.Input.Key.Number6: return KeyboardKey.D6;
				case OpenTK.Input.Key.Number7: return KeyboardKey.D7;
				case OpenTK.Input.Key.Number8: return KeyboardKey.D8;
				case OpenTK.Input.Key.Number9: return KeyboardKey.D9;

				case OpenTK.Input.Key.Back: return KeyboardKey.Backspace;
				case OpenTK.Input.Key.Enter: return KeyboardKey.Return;
				case OpenTK.Input.Key.Tab: return KeyboardKey.Tab;
				case OpenTK.Input.Key.CapsLock: return KeyboardKey.Capital;
				case OpenTK.Input.Key.Escape: return KeyboardKey.Escape;
				case OpenTK.Input.Key.Space: return KeyboardKey.Space;

				case OpenTK.Input.Key.ControlLeft: return KeyboardKey.LeftControl;
				case OpenTK.Input.Key.ControlRight: return KeyboardKey.RightControl;
				case OpenTK.Input.Key.AltLeft: return KeyboardKey.LeftAlt;
				case OpenTK.Input.Key.AltRight: return KeyboardKey.RightAlt;
				case OpenTK.Input.Key.ShiftLeft: return KeyboardKey.LeftShift;
				case OpenTK.Input.Key.ShiftRight: return KeyboardKey.RightShift;
				case OpenTK.Input.Key.WinLeft: return KeyboardKey.LeftWin;
				case OpenTK.Input.Key.WinRight: return KeyboardKey.RightWin;

				case OpenTK.Input.Key.Left: return KeyboardKey.Left;
				case OpenTK.Input.Key.Right: return KeyboardKey.Right;
				case OpenTK.Input.Key.Up: return KeyboardKey.Up;
				case OpenTK.Input.Key.Down: return KeyboardKey.Down;

				case OpenTK.Input.Key.Insert: return KeyboardKey.Insert;
				case OpenTK.Input.Key.Delete: return KeyboardKey.Delete;
				case OpenTK.Input.Key.Home: return KeyboardKey.Home;
				case OpenTK.Input.Key.End: return KeyboardKey.End;
				case OpenTK.Input.Key.PageUp: return KeyboardKey.PageUp;
				case OpenTK.Input.Key.PageDown: return KeyboardKey.PageDown;

				case OpenTK.Input.Key.BracketLeft: return KeyboardKey.LeftBracket;
				case OpenTK.Input.Key.BracketRight: return KeyboardKey.RightBracket;
				case OpenTK.Input.Key.BackSlash: return KeyboardKey.BackSlash;
				case OpenTK.Input.Key.Comma: return KeyboardKey.Comma;
				case OpenTK.Input.Key.Period: return KeyboardKey.Period;
				case OpenTK.Input.Key.Minus: return KeyboardKey.Subtract;
				case OpenTK.Input.Key.Plus: return KeyboardKey.Equal;
				case OpenTK.Input.Key.Semicolon: return KeyboardKey.Semicolon;
				case OpenTK.Input.Key.Slash: return KeyboardKey.Slash;
				case OpenTK.Input.Key.Tilde: return KeyboardKey.Grave;
				case OpenTK.Input.Key.Quote: return KeyboardKey.Apostrophe;
			}

			return KeyboardKey.Unknown;
		}
		#endregion
	}
}
