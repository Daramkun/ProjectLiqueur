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
	}
}
