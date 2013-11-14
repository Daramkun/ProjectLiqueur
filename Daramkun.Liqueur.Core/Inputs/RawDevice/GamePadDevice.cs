using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs.RawDevice
{
	public abstract class GamePadDevice : CommonDevice<GamePadState>
	{
		public abstract bool IsSupportVibration { get; }

		public override bool IsConnected
		{
			get
			{
				foreach ( PlayerIndex pi in new [] { PlayerIndex.Player1, PlayerIndex.Player2, PlayerIndex.Player3, PlayerIndex.Player4 } )
					if ( IsConnectedPlayer ( pi ) ) return true;
				return false;
			}
		}

		public abstract bool IsConnectedPlayer ( PlayerIndex playerIndex = PlayerIndex.Player1 );

		protected abstract GamePadState GenerateState ( PlayerIndex playerIndex );

		public override GamePadState GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			return GenerateState ( playerIndex );
		}

		public abstract void Vibrate ( PlayerIndex playerIndex, float leftMotorSpeed, float rightMotorSpeed );
	}
}
