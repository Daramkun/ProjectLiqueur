using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Nodes
{
	public class InputHelper : Node
	{
		public static InputHelper Instance { get; private set; }

		public static KeyboardState LastKeyboardState { get; private set; }
		public static KeyboardState CurrentKeyboardState { get; private set; }

		public static MouseState LastMouseState { get; private set; }
		public static MouseState CurrentMouseState { get; private set; }

		public static GamePadState [] LastGamePadState { get; private set; }
		public static GamePadState [] CurrentGamePadState { get; private set; }

		public static TouchState LastTouchState { get; private set; }
		public static TouchState CurrentTouchState { get; private set; }

		public static AccelerometerState LastAccelerometerState { get; private set; }
		public static AccelerometerState CurrentAccelerometerState { get; private set; }

		public static bool IsKeyboardEnabled { get; set; }
		public static bool IsMouseEnabled { get; set; }
		public static bool IsGamePadEnabled { get; set; }
		public static bool IsTouchEnabled { get; set; }
		public static bool IsAccelerometerEnabled { get; set; }

		public static bool IsKeyboardKeyDownRightNow ( KeyboardKey key )
		{
			return CurrentKeyboardState.IsKeyDown ( key ) &&
				LastKeyboardState.IsKeyUp ( key );
		}

		public static bool IsKeyboardKeyUpRightNow ( KeyboardKey key )
		{
			return CurrentKeyboardState.IsKeyUp ( key ) &&
				LastKeyboardState.IsKeyDown ( key );
		}

		public static bool IsMouseButtonDownRightNow ( MouseButton button )
		{
			return CurrentMouseState.IsButtonDown ( button ) &&
				LastMouseState.IsButtonUp ( button );
		}

		public static bool IsMouseButtonUpRightNow ( MouseButton button )
		{
			return CurrentMouseState.IsButtonUp ( button ) &&
				LastMouseState.IsButtonDown ( button );
		}

		public static bool IsGamePadButtonDownRightNow ( GamePadButton button, PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			return CurrentGamePadState [ ( int ) playerIndex ].IsButtonDown ( button ) &&
				LastGamePadState [ ( int ) playerIndex ].IsButtonUp ( button );
		}

		public static bool IsGamePadButtonUpRightNow ( GamePadButton button, PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			return CurrentGamePadState [ ( int ) playerIndex ].IsButtonUp ( button ) &&
				LastGamePadState [ ( int ) playerIndex ].IsButtonDown ( button );
		}

		static InputHelper ()
		{
			LastGamePadState = new GamePadState [ 4 ];
			CurrentGamePadState = new GamePadState [ 4 ];
		}

		public static InputHelper CreateInstance ()
		{
			if ( Instance == null )
			{
				Instance = new InputHelper ();

				if ( LiqueurSystem.Keyboard != null ) IsKeyboardEnabled = true;
				else IsKeyboardEnabled = false;

				if ( LiqueurSystem.Mouse != null ) IsMouseEnabled = true;
				else IsMouseEnabled = false;

				if ( LiqueurSystem.GamePad != null ) IsGamePadEnabled = true;
				else IsGamePadEnabled = false;

				if ( LiqueurSystem.TouchPanel != null ) IsTouchEnabled = true;
				else IsTouchEnabled = false;

				if ( LiqueurSystem.Accelerometer != null ) IsAccelerometerEnabled = true;
				else IsAccelerometerEnabled = false;
			}
			return Instance;
		}

		private InputHelper ()
		{
			IsVisible = false;
		}

		public override void Update ( GameTime gameTime )
		{
			if ( LiqueurSystem.Keyboard != null && IsKeyboardEnabled )
			{
				LastKeyboardState = CurrentKeyboardState;
				CurrentKeyboardState = LiqueurSystem.Keyboard.GetState ();
			}

			if ( LiqueurSystem.Mouse != null && IsMouseEnabled )
			{
				LastMouseState = CurrentMouseState;
				CurrentMouseState = LiqueurSystem.Mouse.GetState ();
			}

			if ( LiqueurSystem.GamePad != null && IsGamePadEnabled )
			{
				for ( int i = 0; i < 4; i++ )
				{
					LastGamePadState [ i ] = CurrentGamePadState [ i ];
					CurrentGamePadState [ i ] = LiqueurSystem.GamePad.GetState ();
				}
			}

			if ( LiqueurSystem.TouchPanel != null && IsTouchEnabled )
			{
				LastTouchState = CurrentTouchState;
				CurrentTouchState = LiqueurSystem.TouchPanel.GetState ();
			}

			if ( LiqueurSystem.Accelerometer != null && IsAccelerometerEnabled )
			{
				LastAccelerometerState = CurrentAccelerometerState;
				CurrentAccelerometerState = LiqueurSystem.Accelerometer.GetState ();
			}

			base.Update ( gameTime );
		}
	}
}
