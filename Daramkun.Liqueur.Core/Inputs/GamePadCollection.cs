using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs
{
	public class GamePadCollection
	{
		RawGamePad [] gamePads = new RawGamePad [ 4 ];

		public RawGamePad this [ PlayerIndex playerIndex ]
		{
			get { return gamePads [ ( int ) playerIndex ]; }
			set { gamePads [ ( int ) playerIndex ] = value; }
		}

		public void AddGamePad ( Type gamePad, IWindow window, PlayerIndex playerIndex )
		{
			this [ playerIndex ] = Activator.CreateInstance ( gamePad, window, playerIndex ) as RawGamePad;
		}
	}
}
