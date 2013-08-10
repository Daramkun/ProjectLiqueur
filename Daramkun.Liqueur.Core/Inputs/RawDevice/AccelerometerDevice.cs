using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs.RawDevice
{
	public abstract class AccelerometerDevice : CommonDevice<AccelerometerState>
	{
		public abstract void Start ();
		public abstract void Stop ();

		protected abstract AccelerometerState GenerateState ();

		public override AccelerometerState GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			if ( playerIndex != PlayerIndex.Player1 )
				throw new NotSupportedException ();
			return GenerateState ();
		}
	}
}
