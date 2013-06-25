using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.RawDevices
{
	public abstract class RawMouse : IInputDevice<MouseState>
	{
		public virtual bool IsConnected { get { return false; } }

		protected MouseButton MouseButton = MouseButton.Unknown;
		protected Vector2 Position = new Vector2 ();
		protected float Wheel = 0;

		public RawMouse ( IWindow window )
		{
		
		}

		~RawMouse ()
		{
			Dispose ( false );
		}

		public virtual MouseState GetState ()
		{
			return new MouseState ( MouseButton, Position.X, Position.Y, Wheel );
		}

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
