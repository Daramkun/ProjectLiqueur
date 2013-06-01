using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	public struct Timer
	{
		TimeSpan delay;
		TimeSpan lastTimeSpan;

		public TimeSpan Delay { get { return delay; } set { delay = value; } }
		public bool Check { get { return delay <= lastTimeSpan; } }

		public Timer ( TimeSpan delay )
		{
			this.delay = delay;
			this.lastTimeSpan = new TimeSpan ();
		}

		public void Update ( GameTime gameTime )
		{
			lastTimeSpan += gameTime.ElapsedGameTime;
		}

		public void Reset ()
		{
			lastTimeSpan -= delay;
		}

		public void Clear ()
		{
			lastTimeSpan = new TimeSpan ();
		}
	}
}
