using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs.RawDevice
{
	public abstract class TouchDevice : CommonDevice<TouchState>
	{
		public abstract int MaximumTouchCount { get; }

		protected abstract TouchState GenerateState ();

		public override TouchState GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			if ( playerIndex != PlayerIndex.Player1 )
				throw new NotSupportedException ();
			return GenerateState ();
		}
	}
}
