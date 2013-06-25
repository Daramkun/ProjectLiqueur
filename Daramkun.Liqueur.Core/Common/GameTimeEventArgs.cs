using System;

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
