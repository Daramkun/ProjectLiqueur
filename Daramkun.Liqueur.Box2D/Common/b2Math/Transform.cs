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
	}
}
