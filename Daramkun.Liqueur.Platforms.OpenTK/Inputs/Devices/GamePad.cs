using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.Devices
{
	public class GamePad : RawGamePad
	{
		public override bool IsConnected { get { return false; } }
		public override bool IsSupportVibrator { get { return false; } }

		public GamePad ( IWindow window, PlayerIndex playerIndex )
			: base ( window, playerIndex )
		{

		}

		public override void Vibrate ( float leftSpeed, float rightSpeed )
		{

		}
	}
}
