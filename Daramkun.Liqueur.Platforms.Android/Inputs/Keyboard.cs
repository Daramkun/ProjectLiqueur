using System;
using System.Collections.Generic;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs
{
	public class Keyboard : RawDevice.KeyboardDevice
	{
		private class InternalKeyListener : Android.Views.View.IOnKeyListener
		{
			public List<KeyboardKey> pressedKeys = new List<KeyboardKey> ();

			public bool OnKey ( Android.Views.View v, Android.Views.Keycode keyCode, Android.Views.KeyEvent e )
			{
				if ( e.Action == Android.Views.KeyEventActions.Down )
				{
					KeyboardKey key = ConvertKeys ( keyCode );
					if ( key == KeyboardKey.Unknown ) return false;
					if ( pressedKeys.Contains ( key ) ) return false;
					pressedKeys.Add ( key );
				}
				else if ( e.Action == Android.Views.KeyEventActions.Up )
				{
					KeyboardKey key = ConvertKeys ( keyCode );
					if ( key == KeyboardKey.Unknown ) return false;
					if ( pressedKeys.Contains ( key ) ) pressedKeys.Remove ( key );
				}
				return true;
			}

			public void Dispose ()
			{

			}

			public IntPtr Handle { get { return new IntPtr ( 0 ); } }

			#region Converter
			private KeyboardKey ConvertKeys ( Android.Views.Keycode key )
			{
				switch ( key )
				{
					case Android.Views.Keycode.A: return KeyboardKey.A;
					case Android.Views.Keycode.B: return KeyboardKey.B;
					case Android.Views.Keycode.C: return KeyboardKey.C;
					case Android.Views.Keycode.D: return KeyboardKey.D;
					case Android.Views.Keycode.E: return KeyboardKey.E;
					case Android.Views.Keycode.F: return KeyboardKey.F;
					case Android.Views.Keycode.G: return KeyboardKey.G;
					case Android.Views.Keycode.H: return KeyboardKey.H;
					case Android.Views.Keycode.I: return KeyboardKey.I;
					case Android.Views.Keycode.J: return KeyboardKey.J;
					case Android.Views.Keycode.K: return KeyboardKey.K;
					case Android.Views.Keycode.L: return KeyboardKey.L;
					case Android.Views.Keycode.M: return KeyboardKey.M;
					case Android.Views.Keycode.N: return KeyboardKey.N;
					case Android.Views.Keycode.O: return KeyboardKey.O;
					case Android.Views.Keycode.P: return KeyboardKey.P;
					case Android.Views.Keycode.Q: return KeyboardKey.Q;
					case Android.Views.Keycode.R: return KeyboardKey.R;
					case Android.Views.Keycode.S: return KeyboardKey.S;
					case Android.Views.Keycode.T: return KeyboardKey.T;
					case Android.Views.Keycode.U: return KeyboardKey.U;
					case Android.Views.Keycode.V: return KeyboardKey.V;
					case Android.Views.Keycode.W: return KeyboardKey.W;
					case Android.Views.Keycode.X: return KeyboardKey.X;
					case Android.Views.Keycode.Y: return KeyboardKey.Y;
					case Android.Views.Keycode.Z: return KeyboardKey.Z;
					
					/*
					case Android.Views.Keycode.F1: return KeyboardKey.F1;
					case Android.Views.Keycode.F2: return KeyboardKey.F2;
					case Android.Views.Keycode.F3: return KeyboardKey.F3;
					case Android.Views.Keycode.F4: return KeyboardKey.F4;
					case Android.Views.Keycode.F5: return KeyboardKey.F5;
					case Android.Views.Keycode.F6: return KeyboardKey.F6;
					case Android.Views.Keycode.F7: return KeyboardKey.F7;
					case Android.Views.Keycode.F8: return KeyboardKey.F8;
					case Android.Views.Keycode.F9: return KeyboardKey.F9;
					case Android.Views.Keycode.F10: return KeyboardKey.F10;
					case Android.Views.Keycode.F11: return KeyboardKey.F11;
					case Android.Views.Keycode.F12: return KeyboardKey.F12;
					*/

					case Android.Views.Keycode.Num0: return KeyboardKey.D0;
					case Android.Views.Keycode.Num1: return KeyboardKey.D1;
					case Android.Views.Keycode.Num2: return KeyboardKey.D2;
					case Android.Views.Keycode.Num3: return KeyboardKey.D3;
					case Android.Views.Keycode.Num4: return KeyboardKey.D4;
					case Android.Views.Keycode.Num5: return KeyboardKey.D5;
					case Android.Views.Keycode.Num6: return KeyboardKey.D6;
					case Android.Views.Keycode.Num7: return KeyboardKey.D7;
					case Android.Views.Keycode.Num8: return KeyboardKey.D8;
					case Android.Views.Keycode.Num9: return KeyboardKey.D9;

					case Android.Views.Keycode.Back: return KeyboardKey.Backspace;
					case Android.Views.Keycode.Enter: return KeyboardKey.Return;
					case Android.Views.Keycode.Tab: return KeyboardKey.Tab;
					/*
					case Android.Views.Keycode.CapsLock: return KeyboardKey.Capital;
					case Android.Views.Keycode.Escape: return KeyboardKey.Escape;
					*/
					case Android.Views.Keycode.Space: return KeyboardKey.Space;

					case Android.Views.Keycode.SoftLeft: return KeyboardKey.LeftControl;
					case Android.Views.Keycode.SoftRight: return KeyboardKey.RightControl;
					case Android.Views.Keycode.AltLeft: return KeyboardKey.LeftAlt;
					case Android.Views.Keycode.AltRight: return KeyboardKey.RightAlt;
					case Android.Views.Keycode.ShiftLeft: return KeyboardKey.LeftShift;
					case Android.Views.Keycode.ShiftRight: return KeyboardKey.RightShift;
					//case Android.Views.Keycode.WinLeft: return KeyboardKey.LeftWin;
					//case Android.Views.Keycode.WinRight: return KeyboardKey.RightWin;

					case Android.Views.Keycode.DpadLeft: return KeyboardKey.Left;
					case Android.Views.Keycode.DpadRight: return KeyboardKey.Right;
					case Android.Views.Keycode.DpadUp: return KeyboardKey.Up;
					case Android.Views.Keycode.DpadDown: return KeyboardKey.Down;
					
					/*
					case Android.Views.Keycode.Insert: return KeyboardKey.Insert;
					case Android.Views.Keycode.Delete: return KeyboardKey.Delete;
					*/
					case Android.Views.Keycode.Home: return KeyboardKey.Home;
					//case Android.Views.Keycode.End: return KeyboardKey.End;
					case Android.Views.Keycode.PageUp: return KeyboardKey.PageUp;
					case Android.Views.Keycode.PageDown: return KeyboardKey.PageDown;

					case Android.Views.Keycode.LeftBracket: return KeyboardKey.LeftBracket;
					case Android.Views.Keycode.RightBracket: return KeyboardKey.RightBracket;
					case Android.Views.Keycode.Backslash: return KeyboardKey.BackSlash;
					case Android.Views.Keycode.Comma: return KeyboardKey.Comma;
					case Android.Views.Keycode.Period: return KeyboardKey.Period;
					case Android.Views.Keycode.Minus: return KeyboardKey.Subtract;
					case Android.Views.Keycode.Plus: return KeyboardKey.Equal;
					case Android.Views.Keycode.Semicolon: return KeyboardKey.Semicolon;
					case Android.Views.Keycode.Slash: return KeyboardKey.Slash;
					case Android.Views.Keycode.Grave: return KeyboardKey.Grave;
					case Android.Views.Keycode.Apostrophe: return KeyboardKey.Apostrophe;
				}

				return KeyboardKey.Unknown;
			}
			#endregion
		}

		protected override bool IsSupportMultiPlayers { get { return false; } }
		public override bool IsSupport { get { return true; } }
		public override bool IsConnected { get { return true; } }

		InternalKeyListener keyListener;

		public Keyboard ( IWindow window )
		{
			OpenTK.Platform.Android.AndroidGameView gameView = ( window.Handle as OpenTK.Platform.Android.AndroidGameView );
			gameView.SetOnKeyListener ( keyListener = new InternalKeyListener () );
		}

		protected override KeyboardState GenerateState ()
		{
			return new KeyboardState ( keyListener.pressedKeys.ToArray () );
		}
	}
}

