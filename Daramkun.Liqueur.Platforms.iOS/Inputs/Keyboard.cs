using System;
using System.Collections.Generic;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Inputs.State;

namespace Daramkun.Liqueur.Inputs
{
	public class Keyboard : RawDevice.KeyboardDevice
	{
		protected override bool IsSupportMultiPlayers { get { return false; } }
		public override bool IsSupport { get { return true; } }
		public override bool IsConnected { get { return true; } }

		public Keyboard ( IWindow window )
		{

		}

		protected override KeyboardState GenerateState ()
		{
			return new KeyboardState (  );
		}
	}
}

