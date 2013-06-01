using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.RawDevices
{
	public abstract class RawMouse : IInputDevice<MouseState>
	{
		public virtual bool IsConnected { get { return false; } }

		protected MouseButton MouseButton { get; set; }
		protected Vector2 Position { get; set; }
		protected float Wheel { get; set; }

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
