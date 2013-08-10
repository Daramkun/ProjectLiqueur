using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs.RawDevice
{
	public abstract class KeyboardDevice : CommonDevice<KeyboardState>
	{
		protected abstract bool IsSupportMultiPlayers { get; }
		protected abstract KeyboardState GenerateState ();

		public override KeyboardState GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			if ( ( playerIndex != PlayerIndex.Player1 ) && !IsSupportMultiPlayers )
				throw new NotSupportedException ();
			return GenerateState ();
		}
	}
}
