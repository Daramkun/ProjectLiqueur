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

		protected abstract GamePadState GenerateState ( PlayerIndex playerIndex );

		public override GamePadState GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			return GenerateState ( playerIndex );
		}

		public abstract void Vibrate ( float leftMotorSpeed, float rightMotorSpeed );
	}
}
