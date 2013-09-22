using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Common
{
	public struct Rotation
	{
		float sine, cosine;

		public static Rotation Identity { get { return new Rotation () { sine = 0, cosine = 1 }; } }

		public float Sine { get { return sine; } }
		public float Cosine { get { return cosine; } }

		public float Angle { get { return ( float ) Math.Atan2 ( sine, cosine ); } }
		public Vector2 XAxis { get { return new Vector2 ( cosine, sine ); } }
		public Vector2 YAxis { get { return new Vector2 ( -sine, cosine ); } }

		public Rotation ( float angle )
			: this ()
		{
			Set ( angle );
		}

		public void Set ( float angle )
		{
			sine = ( float ) Math.Sin ( angle );
			cosine = ( float ) Math.Cos ( angle );
		}
	}
}
