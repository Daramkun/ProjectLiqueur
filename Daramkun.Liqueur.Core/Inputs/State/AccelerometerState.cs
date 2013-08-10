using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Inputs.State
{
	public struct AccelerometerState
	{
		public Vector3 Acceleration { get; private set; }

		public AccelerometerState ( Vector3 accel )
			: this ()
		{
			Acceleration = accel;
		}
	}
}
