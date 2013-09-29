using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Box2D.Collision.Shapes;
using Daramkun.Liqueur.Box2D.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Collision
{
	public struct DistanceProxy
	{
		Vector2 [] m_buffer;
		Vector2 [] m_vertices;
		int m_count;
		float m_radius;

		public static DistanceProxy Identity { get { return new DistanceProxy () { m_buffer = new Vector2 [ 2 ] }; } }

		public void Set ( Shape shape, int index )
		{
			switch ( shape.ShapeType )
			{
				case ShapeType.Circle:
					CircleShape circle = shape as CircleShape;
					m_vertices = new Vector2 [] { circle.m_p };
					m_count = 1;
					m_radius = circle.m_radius;
					break;

				case ShapeType.Polygon:
					PolygonShape polygon = shape as PolygonShape;
					m_vertices = polygon.m_vertices;
					m_count = polygon.m_vertexCount;
					m_radius = polygon.m_radius;
					break;

				case ShapeType.Chain:
					ChainShape chain = shape as ChainShape;
					m_buffer [ 0 ] = chain.m_vertices [ index ];
					if ( index + 1 < chain.m_count )
						m_buffer [ 1 ] = chain.m_vertices [ index + 1 ];
					else
						m_buffer [ 1 ] = chain.m_vertices [ 0 ];
					m_vertices = m_buffer;
					m_count = 2;
					m_radius = chain.m_radius;
					break;

				case ShapeType.Edge:
					EdgeShape edge = shape as EdgeShape;
					m_vertices = new Vector2 [] { edge.m_vertex1, edge.m_vertex2 };
					m_count = 2;
					m_radius = edge.m_radius;
					break;

				default:
					throw new ArgumentException ();
			}
		}

		public int GetSupport ( Vector2 d )
		{
			int bestIndex = 0;
			float bestValue = Vector2.Dot ( m_vertices [ 0 ], d );
			for ( int i = 1; i < m_count; ++i )
			{
				float value = Vector2.Dot ( m_vertices [ i ], d );
				if ( value > bestValue )
				{
					bestIndex = i;
					bestValue = value;
				}
			}
			return bestIndex;
		}

		public Vector2 GetSupportVertex ( Vector2 d )
		{
			int bestIndex = 0;
			float bestValue = Vector2.Dot ( m_vertices [ 0 ], d );
			for ( int i = 1; i < m_count; ++i )
			{
				float value = Vector2.Dot ( m_vertices [ i ], d );
				if ( value > bestValue )
				{
					bestIndex = i;
					bestValue = value;
				}
			}
			return m_vertices [ bestIndex ];
		}

		public int VertexCount { get { return m_count; } }

		public Vector2 GetVertex ( int index )
		{
			return m_vertices [ index ];
		}
	}

	public struct SimplexCache
	{
		public float Metric;
		public int Count;
		public int [] IndexA;
		public int [] IndexB;

		public SimplexCache Identity { get { return new SimplexCache () { IndexA = new int [ 3 ], IndexB = new int [ 3 ] }; } }
	}

	public struct DistanceOutput
	{
		public Vector2 PointA;
		public Vector2 PointB;
		public float Distance;
		public int Iterations;
	}

	struct SimplexVertrex
	{
		public Vector2 wA, wB, w;
		public float a;
		public int IndexA, IndexB;
	}

	struct Simplex
	{
		public void ReadCache ( SimplexCache cache, DistanceProxy proxyA, Transform transformA, DistanceProxy proxyB, Transform transformB )
		{
			
		}
	}

	public static class Distance
	{
		/*public static void Distance ( out DistanceOutput output, SimplexCache cache, DistanceInput input )
		{
			
		}*/
	}
}
