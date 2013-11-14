﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs
{
	public class GamePad : GamePadDevice
	{
		public override bool IsSupportVibration { get { return false; } }
		public override bool IsSupport { get { return true; } }

		public override bool IsConnectedPlayer ( PlayerIndex playerIndex = PlayerIndex.Player1 )
		{
			throw new NotImplementedException ();
		}

		protected override GamePadState GenerateState ( PlayerIndex playerIndex )
		{
			throw new NotImplementedException ();
		}

		public override void Vibrate ( PlayerIndex playerIndex, float leftMotorSpeed, float rightMotorSpeed )
		{

		}
	}
}
