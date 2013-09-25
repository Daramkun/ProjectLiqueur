using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Box2D.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Collision.Shapes
{
	public class EdgeShape : Shape
	{
		internal Vector2 m_vertex1, m_vertex2;
		internal Vector2 m_vertex0, m_vertex3;
		internal bool m_hasVertex0, m_hasVertex3;

		public override int ChildCount { get { return 1; } }

		public EdgeShape ()
		{
			ShapeType = Shapes.ShapeType.Edge;
			m_radius = Settings.PolygonRadius;
			m_vertex0 = new Vector2 ();
			m_vertex3 = new Vector2 ();
			m_hasVertex0 = false;
			m_hasVertex3 = false;
		}

		public override Shape Clone ()
		{
			EdgeShape edge = new EdgeShape ();
			edge.ShapeType = ShapeType.Edge;
			edge.m_radius = m_radius;
			edge.m_vertex0 = m_vertex0;
			edge.m_vertex1 = m_vertex1;
			edge.m_vertex2 = m_vertex2;
			edge.m_vertex3 = m_vertex3;
			edge.m_hasVertex0 = m_hasVertex0;
			edge.m_hasVertex3 = m_hasVertex3;
			return edge;
		}

		public void Set ( Vector2 v1, Vector2 v2 )
		{
			m_vertex1 = v1;
			m_vertex2 = v2;
			m_hasVertex0 = false;
			m_hasVertex3 = false;
		}

		public override bool TestPoint ( Common.Transform xf, Mathematics.Vector2 p )
		{
			return false;
		}

		public override bool RayCast ( out RayCastOutput output, RayCastInput input, Common.Transform transform, int childIndex )
		{
			output = new RayCastOutput ();

			Vector2 p1 = transform.q * ( input.Point1 - transform.p );
			Vector2 p2 = transform.q * ( input.Point2 - transform.p );
			Vector2 d = p2 - p1;

			Vector2 v1 = m_vertex1;
			Vector2 v2 = m_vertex2;
			Vector2 e = v2 - v1;
			Vector2 normal = new Vector2 ( e.Y, -e.X ).Normalize ();

			float numerator = Vector2.Dot ( normal, v1 - p1 );
			float denominator = Vector2.Dot ( normal, d );

			if ( denominator == 0.0f ) return false;

			float t = numerator / denominator;
			if ( t < 0.0f || input.MaxFraction < t ) return false;

			Vector2 q = p1 + t * d;

			Vector2 r = v2 - v1;
			float rr = Vector2.Dot ( r, r );
			if ( rr == 0.0f ) return false;

			float s = Vector2.Dot ( q - v1, r ) / rr;
			if ( s < 0 || 1 < s ) return false;

			output.Fraction = t;
			if ( numerator > 0 ) output.Normal = -normal;
			else output.Normal = normal;
			
			return true;
		}

		public override void ComputeAABB ( out AABB aabb, Common.Transform xf, int childIndex )
		{
			Vector2 v1 = xf * m_vertex1;
			Vector2 v2 = xf * m_vertex2;

			Vector2 lower = new Vector2 ( ( float ) Math.Min ( v1.X, v2.X ), ( float ) Math.Min ( v1.Y, v2.Y ) );
			Vector2 upper = new Vector2 ( ( float ) Math.Max ( v1.X, v2.X ), ( float ) Math.Max ( v1.Y, v2.Y ) );

			Vector2 r = new Vector2 ( m_radius );
			aabb = new AABB ();
			aabb.LowerBound = lower - r;
			aabb.UpperBound = upper + r;
		}

		public override void ComputeMass ( out MassData massData, float density )
		{
			massData = new MassData ();
			massData.Mass = 0.0f;
			massData.Center = 0.5f * ( m_vertex1 + m_vertex2 );
			massData.Inertia = 0.0f;
		}
	}
}
