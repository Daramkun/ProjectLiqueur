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
				area = area + ( triangleArea * inv3 * ( p1 + p2 + p3 ) );
			}

			c *= 1.0f / area;
			return c;
		}

		public override bool TestPoint ( Common.Transform xf, Mathematics.Vector2 p )
		{
			throw new NotImplementedException ();
		}

		public override bool RayCast ( out RayCastOutput output, RayCastInput input, Common.Transform transform, int childIndex )
		{
			throw new NotImplementedException ();
		}

		public override void ComputeAABB ( out AABB aabb, Common.Transform xf, int childIndex )
		{
			throw new NotImplementedException ();
		}

		public override void ComputeMass ( out MassData massData, float density )
		{
			throw new NotImplementedException ();
		}
	}
}
