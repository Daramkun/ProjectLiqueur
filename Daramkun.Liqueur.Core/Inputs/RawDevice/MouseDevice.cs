using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs.RawDevice
{
	public abstract class MouseDevice : CommonDevice<MouseState>
	{
		protected abstract MouseState GenerateState ();

		public override MouseState GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			if ( playerIndex != PlayerIndex.Player1 )
				throw new NotSupportedException ();
			return GenerateState ();
		}
	}
}
