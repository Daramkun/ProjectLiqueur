using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.RawDevices
{
	public abstract class RawTouchPanel : IInputDevice<TouchState>
	{
		public virtual bool IsConnected { get { return false; } }

		protected List<TouchPoint> TouchPoints { get; private set; }

		public RawTouchPanel ( IWindow window )
		{
			TouchPoints = new List<TouchPoint> ();
		}

		~RawTouchPanel ()
		{
			Dispose ( false );
		}

		public virtual TouchState GetState ()
		{
			return new TouchState ( TouchPoints.ToArray () );
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
