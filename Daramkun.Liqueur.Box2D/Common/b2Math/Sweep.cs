using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Common
{
	public struct Sweep
	{
		Vector2 localCenter;
		Vector2 c0, c;
		float a0, a;

		float alpha0;

		public Transform GetTransform ( float beta )
		{
			Transform transform = new Transform ();
			transform.p = ( 1.0f - beta ) * c0 + beta * c;
			float angle = ( 1.0f - beta ) * a0 + beta * a;
			transform.q.Set ( angle );
			transform.p -= new Vector2 ( transform.q.Cosine * localCenter.X - transform.q.Sine * localCenter.Y,
				transform.q.Sine * localCenter.X + transform.q.Cosine * localCenter.Y );
			return transform;
		}

		public void Advance ( float alpha )
		{
			float beta = ( alpha - alpha0 ) / ( 1.0f - alpha0 );
			c0 = ( 1.0f - beta ) * c0 + beta * c;
			a0 = ( 1.0f - beta ) * a0 + beta * a;
			alpha0 = alpha;
		}

		public void Normalize ()
		{
			float twoPi = 2.0f * ( float ) Math.PI;
			float d = twoPi * ( float ) Math.Floor ( a0 / twoPi );
			a0 -= d;
			a -= d;
		}
	}
}
