using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Box2D.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Collision.Shapes
{
	public class PolygonShape : Shape
	{
		Vector2 m_centroid;
		Vector2 [] m_vertices;
		Vector2 [] m_normals;
		int m_vertexCount;

		public override int ChildCount { get { return 1; } }

		public PolygonShape ()
		{
			ShapeType = Shapes.ShapeType.Polygon;
			m_radius = Settings.PolygonRadius;
			m_vertexCount = 0;
			m_centroid = new Vector2 ();
			m_vertices = new Vector2 [ Settings.MaxPolygonVertices ];
			m_normals = new Vector2 [ Settings.MaxPolygonVertices ];
		}

		public Vector2 GetVertex ( int index ) { return m_vertices [ index ]; }

		public override Shape Clone ()
		{
			PolygonShape poly = new PolygonShape ();
			poly.ShapeType = ShapeType.Polygon;
			poly.m_radius = m_radius;
			poly.m_centroid = m_centroid;
			poly.m_vertexCount = m_vertexCount;
			m_vertices.CopyTo ( poly.m_vertices, 0 );
			m_normals.CopyTo ( poly.m_normals, 0 );
			return poly;
		}

		public void SetAsBox ( float hx, float hy )
		{
			m_vertexCount = 4;
			m_vertices [ 0 ] = new Vector2 ( -hx, -hy );
			m_vertices [ 1 ] = new Vector2 ( hx, -hy );
			m_vertices [ 2 ] = new Vector2 ( hx, hy );
			m_vertices [ 3 ] = new Vector2 ( -hx, hy );
			m_normals [ 0 ] = new Vector2 ( 0, -1 );
			m_normals [ 1 ] = new Vector2 ( 1, 0 );
			m_normals [ 2 ] = new Vector2 ( 0, 1 );
			m_normals [ 3 ] = new Vector2 ( -1, 0 );
			m_centroid = new Vector2 ();
		}

		public void SetAsBox ( float hx, float hy, Vector2 center, float angle )
		{
			m_vertexCount = 4;
			m_vertices [ 0 ] = new Vector2 ( -hx, -hy );
			m_vertices [ 1 ] = new Vector2 ( hx, -hy );
			m_vertices [ 2 ] = new Vector2 ( hx, hy );
			m_vertices [ 3 ] = new Vector2 ( -hx, hy );
			m_normals [ 0 ] = new Vector2 ( 0, -1 );
			m_normals [ 1 ] = new Vector2 ( 1, 0 );
			m_normals [ 2 ] = new Vector2 ( 0, 1 );
			m_normals [ 3 ] = new Vector2 ( -1, 0 );
			m_centroid = center;

			Transform xf = new Transform () { p = center, q = new Rotation ( angle ) };

			for ( int i = 0; i < m_vertexCount; ++i )
			{
				m_vertices [ i ] = xf * m_vertices [ i ];
				m_normals [ i ] = xf.q * m_normals [ i ];
			}
		}

		Vector2 ComputeCentroid ( Vector2 [] vs, int count )
		{
			Vector2 c = new Vector2 ();
			float area = 0;

			Vector2 pRef = new Vector2 ();
			for ( int i = 0; i < count; ++i )
				pRef += vs [ i ];
			pRef *= 1.0f / count;

			float inv3 = 1 / 3.0f;
			for ( int i = 0; i < count; ++i )
			{
				Vector2 p1 = pRef;
				Vector2 p2 = vs [ i ];
				Vector2 p3 = i + 1 < count ? vs [ i + 1 ] : vs [ 0 ];

				Vector2 e1 = p2 - p1;
				Vector2 e2 = p3 - p1;

				float D = Vector2.Cross ( e1, e2 );

				float triangleArea = 0.5f * D;
				area += triangleArea;

				c += triangleArea * inv3 * ( p1 + p2 + p3 );
			}

			c *= 1.0f / area;
			return c;
		}

		public void Set ( Vector2 [] vertices )
		{
			m_vertexCount = vertices.Length;

			for ( int i = 0; i < m_vertexCount; ++i )
				m_vertices [ i ] = vertices [ i ];

			for ( int i = 0; i < m_vertexCount; ++i )
			{
				int i1 = i;
				int i2 = i + 1 < m_vertexCount ? i + 1 : 0;
				Vector2 edge = m_vertices [ i2 ] - m_vertices [ i1 ];
				m_normals [ i ] = Vector2.Cross ( edge, 1.0f ).Normalize ();
			}

			for ( int i = 0; i < m_vertexCount; ++i )
			{
				int i1 = i;
				int i2 = i + 1 < m_vertexCount ? i + 1 : 0;
				Vector2 edge = m_vertices [ i2 ] - m_vertices [ i1 ];

				for ( int j = 0; j < m_vertexCount; ++j )
				{
					if ( j == i1 || j == i2 ) continue;
					Vector2 r = m_vertices [ j ] - m_vertices [ i1 ];
					float s = Vector2.Cross ( edge, r );
				}
			}

			m_centroid = ComputeCentroid ( m_vertices, m_vertexCount );
		}

		public override bool TestPoint ( Transform xf, Vector2 p )
		{
			Vector2 pLocal = xf.q * ( p - xf.p );
			for ( int i = 0; i < m_vertexCount; ++i )
			{
				float dot = Vector2.Dot ( m_normals [ i ], pLocal - m_vertices [ i ] );
				if ( dot > 0.0f ) return false;
			}
			return true;
		}

		public override bool RayCast ( out RayCastOutput output, RayCastInput input, Common.Transform transform, int childIndex )
		{
			output = new RayCastOutput ();

			Vector2 p1 = transform.q * ( input.Point1 - transform.p );
			Vector2 p2 = transform.q * ( input.Point2 - transform.p );
			Vector2 d = p2 - p1;

			float lower = 0.0f, upper = input.MaxFraction;
			int index = -1;

			for ( int i = 0; i < m_vertexCount; ++i )
			{
				float numerator = Vector2.Dot ( m_normals [ i ], m_vertices [ i ] - p1 );
				float denominator = Vector2.Dot ( m_normals [ i ], d );

				if ( denominator == 0.0f )
				{
					if ( numerator < 0.0f )
						return false;
				}
				else
				{
					if ( denominator < 0.0f && numerator < lower * denominator )
					{
						lower = numerator / denominator;
						index = i;
					}
					else if ( denominator > 0.0f && numerator < upper * denominator )
						upper = numerator / denominator;
				}

				if ( upper < lower ) return false;
			}

			if ( index >= 0 )
			{
				output.Fraction = lower;
				output.Normal = transform.q * m_normals [ index ];
				return true;
			}

			return false;
		}

		public override void ComputeAABB ( out AABB aabb, Common.Transform xf, int childIndex )
		{
			Vector2 lower = xf * m_vertices [ 0 ];
			Vector2 upper = lower;

			for ( int i = 1; i < m_vertexCount; ++i )
			{
				Vector2 v = xf * m_vertices [ i ];
				lower = new Vector2 ( ( float ) Math.Min ( lower.X, v.X ), ( float ) Math.Min ( lower.Y, v.Y ) );
				upper = new Vector2 ( ( float ) Math.Max ( upper.X, v.X ), ( float ) Math.Max ( upper.Y, v.Y ) );
			}

			Vector2 r = new Vector2 ( m_radius, m_radius );
			aabb = new AABB ();
			aabb.LowerBound = lower - r;
			aabb.UpperBound = upper + r;
		}

		public override void ComputeMass ( out MassData massData, float density )
		{
			Vector2 center = new Vector2 ();
			float area = 0;
			float I = 0;

			Vector2 s = new Vector2 ( 0 );
			for ( int i = 0; i < m_vertexCount; ++i )
				s += m_vertices [ i ];
			s *= 1.0f / m_vertexCount;

			float k_inv3 = 1 / 3.0f;
			for ( int i = 0; i < m_vertexCount; ++i )
			{
				Vector2 e1 = m_vertices [ i ] - s;
				Vector2 e2 = i + 1 < m_vertexCount ? m_vertices [ i + 1 ] - s : m_vertices [ 0 ] - s;

				float D = Vector2.Cross ( e1, e2 );

				float triangleArea = 0.5f * D;
				area += triangleArea;

				center += triangleArea * k_inv3 * ( e1 + e2 );

				float ex1 = e1.X, ey1 = e1.Y;
				float ex2 = e2.X, ey2 = e2.Y;

				float intx2 = ex1 * ex1 + ex2 * ex1 + ex2 * ex2;
				float inty2 = ey1 * ey1 + ey2 * ey1 + ey2 * ey2;

				I += ( 0.25f * k_inv3 * D ) * ( intx2 + inty2 );
			}

			massData = new MassData ();
			massData.Mass = density * area;

			center *= 1.0f / area;
			massData.Center = center + s;

			massData.Inertia = density * I;

			massData.Inertia += massData.Mass * ( Vector2.Dot ( massData.Center, massData.Center ) - Vector2.Dot ( center, center ) );
		}
	}
}
