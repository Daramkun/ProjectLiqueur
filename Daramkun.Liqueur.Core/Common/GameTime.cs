using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	/// <summary>
	/// Game time class
	/// - For the time services
	/// </summary>
	public class GameTime
	{
		int lastTickCount;
		TimeSpan totalTimeSpan;
		TimeSpan elapsedTimeSpan;

		/// <summary>
		/// Total game time
		/// </summary>
		public TimeSpan TotalGameTime { get { return totalTimeSpan; } }
		/// <summary>
		/// Elapsed game time of frame
		/// </summary>
		public TimeSpan ElapsedGameTime { get { return elapsedTimeSpan; } }

		/// <summary>
		/// Constructor of Game time
		/// </summary>
		public GameTime ()
		{
			lastTickCount = Environment.TickCount;
			totalTimeSpan = new TimeSpan ();
			elapsedTimeSpan = new TimeSpan ();
		}

		/// <summary>
		/// Game time calculation
		/// </summary>
		public void Update ()
		{
			int nowTickCount = Environment.TickCount;
			elapsedTimeSpan = TimeSpan.FromMilliseconds ( nowTickCount - lastTickCount );
			totalTimeSpan += elapsedTimeSpan;
			lastTickCount = nowTickCount;
		}

		/// <summary>
		/// Reset the Elapsed game time
		/// </summary>
		public void Reset ()
		{
			elapsedTimeSpan = new TimeSpan ();
		}
	}

	/// <summary>
	/// Game time event arguments
	/// </summary>
	public class GameTimeEventArgs : EventArgs
	{
		/// <summary>
		/// Game time
		/// </summary>
		public GameTime GameTime { get; internal set; }
	}
}
