﻿using System;

namespace Daramkun.Liqueur.Common
{
	public class GameTime
	{
		int lastTickCount;
		TimeSpan totalTimeSpan;
		TimeSpan elapsedTimeSpan;

		public TimeSpan TotalGameTime { get { return totalTimeSpan; } }
		public TimeSpan ElapsedGameTime { get { return elapsedTimeSpan; } }

		public GameTime ()
		{
			lastTickCount = Environment.TickCount;
			totalTimeSpan = new TimeSpan ();
			elapsedTimeSpan = new TimeSpan ();
		}

		public void Update ()
		{
			int nowTickCount = Environment.TickCount;
			elapsedTimeSpan = TimeSpan.FromMilliseconds ( nowTickCount - lastTickCount );
			totalTimeSpan += elapsedTimeSpan;
			lastTickCount = nowTickCount;
		}

		public void Reset ()
		{
			elapsedTimeSpan = new TimeSpan ();
		}
	}
}
