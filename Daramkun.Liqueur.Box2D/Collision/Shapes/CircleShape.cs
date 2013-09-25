using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Box2D.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Collision.Shapes
{
	public class CircleShape : Shape
	{
		Vector2 m_p;

		public override int ChildCount { get { return 1; } }

		public CircleShape ()
		{
			ShapeType = ShapeType.Circle;
			m_radius = 0;
			m_p = new Vector2 ();
		}

		public override Shape Clone ()
		{
			CircleShape shape = new CircleShape ();
			shape.ShapeType = Shapes.ShapeType.Circle;
			shape.m_p = m_p;
			shape.m_radius = m_radius;
			return shape;
		}

		public override bool TestPoint ( Transform xf, Vector2 p )
		{
			Vector2 center = xf.p + ( xf.q * m_p );
			Vector2 d = p - center;
			return Vector2.Dot ( d, d ) <= m_radius * m_radius;
		}

		public override bool RayCast ( out RayCastOutput output, RayCastInput input, Common.Transform transform, int childIndex )
		{
			output = new RayCastOutput ();

			Vector2 position = transform.p + ( transform.q * m_p );
			Vector2 s = input.Point1 - position;
			float b = Vector2.Dot ( s, s ) - m_radius * m_radius;

			Vector2 r = input.Point2 - input.Point1;
			float c = Vector2.Dot ( s, r );
			float rr = Vector2.Dot ( r, r );
			float sigma = c * c - rr * b;

			if ( sigma < 0 || rr < float.Epsilon ) return false;

			float a = -( c + ( float ) Math.Sqrt ( sigma ) );

			if ( 0 <= a && a <= input.MaxFraction * rr )
			{
				a /= rr;
				output.Fraction = a;
				output.Normal = ( s + a * r ).Normalize ();
				return true;
			}

			return false;
		}

		public override void ComputeAABB ( out AABB aabb, Transform xf, int childIndex )
		{
			Vector2 p = xf.p + ( xf.q * m_p );
			aabb = new AABB ();
			aabb.LowerBound = new Vector2 ( p.X - m_radius, p.Y - m_radius );
			aabb.UpperBound = new Vector2 ( p.X + m_radius, p.Y + m_radius );
		}

		public override void ComputeMass ( out MassData massData, float density )
		{
			massData = new MassData ();
			massData.Mass = density * ( float ) Math.PI * m_radius * m_radius;
			massData.Center = m_p;
			massData.Inertia = massData.Mass * ( 0.5f * m_radius * m_radius + Vector2.Dot ( m_p, m_p ) );
		}

		public int GetSupport ( Vector2 d ) { return 0; }

		public Vector2 GetSupportVertex ( Vector2 d )
		{
			return m_p;
		}

		public Vector2 GetVertex ( int index )
		{
			return m_p;
		}

		public int VertexCount { get { return 1; } }
	}
}
