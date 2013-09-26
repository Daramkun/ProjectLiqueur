using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Common
{
	public struct Transform
	{
		internal Vector2 p;
		internal Rotation q;

		public Transform Identity { get { return new Transform () { p = new Vector2 (), q = Rotation.Identity }; } }

		public Transform ( Vector2 p, Rotation q )
			: this ()
		{
			this.p = p;
			this.q = q;
		}

		public void Set ( Vector2 position, float rotation )
		{
			p = position;
			q.Set ( rotation );
		}

		public static Vector2 operator * ( Transform transform, Vector2 v )
		{
			return new Vector2 (
				( transform.q.Cosine * v.X - transform.q.Sine * v.Y ) + transform.p.X,
			    ( transform.q.Sine * v.X + transform.q.Cosine * v.Y ) + transform.p.Y
			);
		}

		public static Transform operator * ( Transform xfA, Transform xfB )
		{
			Transform C = new Transform ();
			C.q = xfA.q * xfB.q;
			C.p = xfA.q * ( xfB.p - xfA.p );
			return C;
		}

		public static Transform TransposeMultiply ( Transform xfA, Transform xfB )
		{
			Transform C = new Transform ();
			C.q = Rotation.TransposeMultiply ( xfA.q, xfB.q );
			C.p = Rotation.TransposeMultiply ( xfA.q, xfB.p - xfA.p );
			return C;
		}

		public static Vector2 TransposeMultiply ( Transform T, Vector2 v )
		{
			float px = v.X - T.p.X;
			float py = v.Y - T.p.Y;
			float x = (T.q.cosine * px + T.q.sine * py);
			float y = (-T.q.sine * px + T.q.cosine * py);

			return new Vector2 ( x, y );
		}
	}
}
