using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	/// <summary>
	/// Timer class
	/// - time difference calculator
	/// </summary>
	public struct Timer
	{
		TimeSpan delay;
		TimeSpan lastTimeSpan;

		/// <summary>
		/// Delay value
		/// </summary>
		public TimeSpan Delay { get { return delay; } set { delay = value; } }
		/// <summary>
		/// Is time difference greater than delay value?
		/// </summary>
		public bool Check { get { return delay <= lastTimeSpan; } }

		/// <summary>
		/// Constructor of timer class
		/// </summary>
		/// <param name="delay">Delay value</param>
		public Timer ( TimeSpan delay )
		{
			this.delay = delay;
			this.lastTimeSpan = new TimeSpan ();
		}

		/// <summary>
		/// Calculating the time difference
		/// </summary>
		/// <param name="gameTime">Game time</param>
		public void Update ( GameTime gameTime )
		{
			lastTimeSpan += gameTime.ElapsedGameTime;
		}

		/// <summary>
		/// Reset time difference
		/// </summary>
		public void Reset ()
		{
			lastTimeSpan -= delay;
		}

		/// <summary>
		/// time difference set 00:00:00
		/// </summary>
		public void Clear ()
		{
			lastTimeSpan = new TimeSpan ();
		}
	}
}
