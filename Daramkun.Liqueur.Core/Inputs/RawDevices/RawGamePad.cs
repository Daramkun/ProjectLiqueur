using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.RawDevices
{
	public abstract class RawGamePad : IInputDevice<GamePadState>
	{
		public virtual bool IsConnected { get { return false; } }
		public virtual bool IsSupportVibrator { get { return false; } }

		protected Vector2 LeftThumbstick = new Vector2 ();
		protected Vector2 RightThumbstick = new Vector2 ();
		protected Vector2 Trigger = new Vector2 ();
		protected GamePadButton Buttons = GamePadButton.None;

		public RawGamePad ( IWindow window, PlayerIndex playerIndex )
		{
			
		}

		~RawGamePad ()
		{
			Dispose ( false );
		}

		public virtual GamePadState GetState ()
		{
			return new GamePadState ( LeftThumbstick, RightThumbstick,
				Trigger.X, Trigger.Y, Buttons );
		}

		public abstract void Vibrate ( float leftSpeed, float rightSpeed );

		protected virtual void Dispose ( bool isDisposing )
		{

		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( true );
		}
	}
}
