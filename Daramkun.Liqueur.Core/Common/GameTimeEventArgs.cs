using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	public class GameTimeEventArgs : EventArgs
	{
		public GameTime GameTime { get; private set; }

		public GameTimeEventArgs ( GameTime gameTime )
		{
			GameTime = gameTime;
		}
	}
}
