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

	struct SimplexVertex
	{
		public Vector2 wA, wB, w;
		public float a;
		public int IndexA, IndexB;
	}

	struct Simplex
	{
		int m_count;
		SimplexVertex m_v1, m_v2, m_v3;

		public void ReadCache ( SimplexCache cache, DistanceProxy proxyA, Transform transformA, DistanceProxy proxyB, Transform transformB )
		{
			if ( cache.Count <= 3 ) throw new Exception ();

			m_count = cache.Count;
			SimplexVertex [] vertices = new SimplexVertex [ 3 ] { m_v1, m_v2, m_v3 };
			for ( int i = 0; i < m_count; ++i )
			{
				SimplexVertex v = vertices [ i ];
				v.IndexA = cache.IndexA [ i ];
				v.IndexB = cache.IndexB [ i ];
				Vector2 wALocal = proxyA.GetVertex ( v.IndexA );
				Vector2 wBLocal = proxyB.GetVertex ( v.IndexB );
				v.wA = transformA * wALocal;
				v.wB = transformB * wBLocal;
				v.w = v.wB - v.wA;
				v.a = 0;
			}
			m_v1 = vertices [ 0 ];
			m_v2 = vertices [ 1 ];
			m_v3 = vertices [ 2 ];

			if ( m_count > 1 )
			{
				float metric1 = cache.Metric;
				float metric2 = GetMetric ();
				if ( metric2 < 0.5f * metric1 || 2.0f * metric1 < metric2 || metric2 < float.Epsilon )
					m_count = 0;
			}

			if ( m_count == 0 )
			{
				SimplexVertex v = m_v1;
				v.IndexA = 0;
				v.IndexB = 0;
				Vector2 wALocal = proxyA.GetVertex ( 0 );
				Vector2 wBLocal = proxyB.GetVertex ( 0 );
				v.wA = transformA * wALocal;
				v.wB = transformB * wBLocal;
				v.w = v.wB - v.wA;
				m_count = 1;
				m_v1 = v;
			}
		}

		public void WriteCache ( ref SimplexCache cache )
		{
			cache.Metric = GetMetric ();
			cache.Count = m_count;
			SimplexVertex [] vertices = new SimplexVertex [ 3 ] { m_v1, m_v2, m_v3 };
			for ( int i = 0; i < m_count; ++i )
			{
				cache.IndexA [ i ] = vertices [ i ].IndexA;
				cache.IndexB [ i ] = vertices [ i ].IndexB;
			}
			m_v1 = vertices [ 0 ];
			m_v2 = vertices [ 1 ];
			m_v3 = vertices [ 2 ];
		}

		public Vector2 GetSearchDirection ()
		{
			switch ( m_count )
			{
				case 1:
					return -m_v1.w;
				case 2:
					Vector2 e12 = m_v2.w - m_v1.w;
					float sgn = Vector2.Cross ( e12, -m_v1.w );
					if ( sgn > 0 )
						return Vector2.Cross ( 1, e12 );
					else
						return Vector2.Cross ( e12, 1 );
				default:
					return new Vector2 ();
			}
		}

		public Vector2 GetClosestPoint ()
		{
			switch ( m_count )
			{
				case 0: return new Vector2 ();
				case 1: return m_v1.w;
				case 2: return m_v1.a * m_v1.w + m_v2.a * m_v2.w;
				case 3: return new Vector2 ();
				default: return new Vector2 ();
			}
		}

		public void GetWitnessPoints ( ref Vector2 pA, ref Vector2 pB )
		{
			switch ( m_count )
			{
				case 0: break;
				case 1:
					pA = m_v1.wA;
					pB = m_v1.wB;
					break;
				case 2:
					pA = m_v1.a * m_v1.wA + m_v2.a * m_v2.wA;
					pB = m_v1.a * m_v1.wB + m_v2.a * m_v2.wB;
					break;
				case 3:
					pA = m_v1.a * m_v1.wA + m_v2.a * m_v2.wA + m_v3.a * m_v3.wA;
					pB = pA;
					break;
				default:
					break;
			}
		}

		public float GetMetric ()
		{
			switch ( m_count )
			{
				case 0: return 0;
				case 1: return 0;
				case 2: return Vector2.Distance ( m_v1.w, m_v2.w );
				case 3: return Vector2.Cross ( m_v2.w - m_v1.w, m_v3.w - m_v1.w );
				default: return 0;
			}
		}

		public void Solve2 ()
		{
			
		}

		public void Solve3 ()
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
