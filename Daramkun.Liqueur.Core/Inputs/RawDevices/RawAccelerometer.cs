using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.RawDevices
{
	public abstract class RawAccelerometer : IInputDevice<AccelerometerState>
	{
		public virtual bool IsConnected { get { return false; } }

		protected Vector3 AccelerometerValue = new Vector3 ();

		public RawAccelerometer ( IWindow window )
		{
			
		}

		~RawAccelerometer ()
		{
			Dispose ( false );
		}

		public abstract void Start ();
		public abstract void Stop ();

		public virtual AccelerometerState GetState ()
		{
			return new AccelerometerState ( AccelerometerValue.X,
				AccelerometerValue.Y, AccelerometerValue.Z );
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