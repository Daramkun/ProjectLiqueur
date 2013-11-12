using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs
{
	public class Keyboard : KeyboardDevice
	{
		IWindow window;

		public override bool IsSupport { get { return true; } }
		public override bool IsConnected { get { return true; } }
		protected override bool IsSupportMultiPlayers { get { return false; } }

		private List<KeyboardKey> pressedKeys;

		public Keyboard ( IWindow window )
		{
			this.window = window;
			pressedKeys = new List<KeyboardKey> ();

			Form otkWindow = window.Handle as Form;
			otkWindow.KeyDown += KeyDownEvent;
			otkWindow.KeyUp += KeyUpEvent;	
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				Form otkWindow = window.Handle as Form;
				otkWindow.KeyDown -= KeyDownEvent;
				otkWindow.KeyUp -= KeyUpEvent;
			}
			base.Dispose ( isDisposing );
		}

		private void KeyDownEvent ( object sender, KeyEventArgs e )
		{
			KeyboardKey key = ConvertKeys ( e.KeyCode );
			if ( key == KeyboardKey.Unknown ) return;
			if ( pressedKeys.Contains ( key ) ) return;
			pressedKeys.Add ( key );
		}

		private void KeyUpEvent ( object sender, KeyEventArgs e )
		{
			KeyboardKey key = ConvertKeys ( e.KeyCode );
			if ( key == KeyboardKey.Unknown ) return;
			if ( pressedKeys.Contains ( key ) ) pressedKeys.Remove ( key );
		}

		protected override KeyboardState GenerateState ()
		{
			if ( pressedKeys.Count == 0 ) return new KeyboardState ();
			return new KeyboardState ( pressedKeys.ToArray () );
		}

		#region Converter
		private KeyboardKey ConvertKeys ( Keys keys )
		{
			switch ( keys )
			{
				case Keys.A: return KeyboardKey.A;
				case Keys.B: return KeyboardKey.B;
				case Keys.C: return KeyboardKey.C;
				case Keys.D: return KeyboardKey.D;
				case Keys.E: return KeyboardKey.E;
				case Keys.F: return KeyboardKey.F;
				case Keys.G: return KeyboardKey.G;
				case Keys.H: return KeyboardKey.H;
				case Keys.I: return KeyboardKey.I;
				case Keys.J: return KeyboardKey.J;
				case Keys.K: return KeyboardKey.K;
				case Keys.L: return KeyboardKey.L;
				case Keys.M: return KeyboardKey.M;
				case Keys.N: return KeyboardKey.N;
				case Keys.O: return KeyboardKey.O;
				case Keys.P: return KeyboardKey.P;
				case Keys.Q: return KeyboardKey.Q;
				case Keys.R: return KeyboardKey.R;
				case Keys.S: return KeyboardKey.S;
				case Keys.T: return KeyboardKey.T;
				case Keys.U: return KeyboardKey.U;
				case Keys.V: return KeyboardKey.V;
				case Keys.W: return KeyboardKey.W;
				case Keys.X: return KeyboardKey.X;
				case Keys.Y: return KeyboardKey.Y;
				case Keys.Z: return KeyboardKey.Z;

				case Keys.F1: return KeyboardKey.F1;
				case Keys.F2: return KeyboardKey.F2;
				case Keys.F3: return KeyboardKey.F3;
				case Keys.F4: return KeyboardKey.F4;
				case Keys.F5: return KeyboardKey.F5;
				case Keys.F6: return KeyboardKey.F6;
				case Keys.F7: return KeyboardKey.F7;
				case Keys.F8: return KeyboardKey.F8;
				case Keys.F9: return KeyboardKey.F9;
				case Keys.F10: return KeyboardKey.F10;
				case Keys.F11: return KeyboardKey.F11;
				case Keys.F12: return KeyboardKey.F12;

				case Keys.D0: return KeyboardKey.D0;
				case Keys.D1: return KeyboardKey.D1;
				case Keys.D2: return KeyboardKey.D2;
				case Keys.D3: return KeyboardKey.D3;
				case Keys.D4: return KeyboardKey.D4;
				case Keys.D5: return KeyboardKey.D5;
				case Keys.D6: return KeyboardKey.D6;
				case Keys.D7: return KeyboardKey.D7;
				case Keys.D8: return KeyboardKey.D8;
				case Keys.D9: return KeyboardKey.D9;

				case Keys.Back: return KeyboardKey.Backspace;
				case Keys.Enter: return KeyboardKey.Return;
				case Keys.Tab: return KeyboardKey.Tab;
				case Keys.CapsLock: return KeyboardKey.Capital;
				case Keys.Escape: return KeyboardKey.Escape;
				case Keys.Space: return KeyboardKey.Space;

				/*case Keys.ControlLeft: return KeyboardKey.LeftControl;
				case Keys.ControlRight: return KeyboardKey.RightControl;
				case Keys.AltLeft: return KeyboardKey.LeftAlt;
				case Keys.AltRight: return KeyboardKey.RightAlt;
				case Keys.ShiftLeft: return KeyboardKey.LeftShift;
				case Keys.ShiftRight: return KeyboardKey.RightShift;
				case Keys.WinLeft: return KeyboardKey.LeftWin;
				case Keys.WinRight: return KeyboardKey.RightWin;*/

				case Keys.Left: return KeyboardKey.Left;
				case Keys.Right: return KeyboardKey.Right;
				case Keys.Up: return KeyboardKey.Up;
				case Keys.Down: return KeyboardKey.Down;

				case Keys.Insert: return KeyboardKey.Insert;
				case Keys.Delete: return KeyboardKey.Delete;
				case Keys.Home: return KeyboardKey.Home;
				case Keys.End: return KeyboardKey.End;
				case Keys.PageUp: return KeyboardKey.PageUp;
				case Keys.PageDown: return KeyboardKey.PageDown;

				/*case Keys.BracketLeft: return KeyboardKey.LeftBracket;
				case Keys.BracketRight: return KeyboardKey.RightBracket;
				case Keys.BackSlash: return KeyboardKey.BackSlash;
				case Keys.Comma: return KeyboardKey.Comma;
				case Keys.Period: return KeyboardKey.Period;
				case Keys.Minus: return KeyboardKey.Subtract;
				case Keys.Plus: return KeyboardKey.Equal;
				case Keys.Semicolon: return KeyboardKey.Semicolon;
				case Keys.Slash: return KeyboardKey.Slash;
				case Keys.Tilde: return KeyboardKey.Grave;
				case Keys.Quote: return KeyboardKey.Apostrophe;*/
			}

			return KeyboardKey.Unknown;
		}
		#endregion
	}
}
