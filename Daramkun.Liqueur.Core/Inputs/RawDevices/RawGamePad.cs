using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.RawDevices
{
	public abstract class RawGamePad : IInputDevice<GamePadState>
	{
		public virtual bool IsConnected { get { return false; } }
		public virtual bool IsSupportVibrator { get { return false; } }

		protected Vector2 LeftThumbstick { get; set; }
		protected Vector2 RightThumbstick { get; set; }
		protected Vector2 Trigger { get; set; }
		protected GamePadButton Buttons { get; set; }

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

		public abstract void Vibrate ( TimeSpan vibrateTime, float leftSpeed, float rightSpeed );

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
