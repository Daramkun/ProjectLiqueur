using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.States
{
	public struct AccelerometerState
	{
		float x, y, z;

		public float X { get { return x; } }
		public float Y { get { return y; } }
		public float Z { get { return z; } }

		internal AccelerometerState ( float x, float y, float z )
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
}
