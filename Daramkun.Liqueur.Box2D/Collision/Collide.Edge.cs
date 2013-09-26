using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Box2D.Collision.Shapes;
using Daramkun.Liqueur.Box2D.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Collision
{
	public static partial class Collide
	{
		public static void CollideEdgeAndCircle ( out Manifold manifold, EdgeShape edgeA, Transform xfA, CircleShape circleB, Transform xfB )
		{
			manifold = Manifold.Identity;
			manifold.PointCount = 0;

			Vector2 Q = Transform.TransposeMultiply ( xfA, xfB * circleB.m_p );

			Vector2 A = edgeA.m_vertex1, B = edgeA.m_vertex2;
			Vector2 e = B - A;

			// Barycentric coordinates
			float u = Vector2.Dot ( e, B - Q );
			float v = Vector2.Dot ( e, Q - A );

			float radius = edgeA.m_radius + circleB.m_radius;

			ContactFeature cf = new ContactFeature ();
			cf.IndexB = 0;
			cf.TypeB = ContactFeatureType.Vertex;

			// Region A
			if ( v <= 0.0f )
			{
				Vector2 P = A;
				Vector2 d = Q - P;
				float dd = Vector2.Dot ( d, d );
				if ( dd > radius * radius )
				{
					return;
				}

				// Is there an edge connected to A?
				if ( edgeA.m_hasVertex0 )
				{
					Vector2 A1 = edgeA.m_vertex0;
					Vector2 B1 = A;
					Vector2 e1 = B1 - A1;
					float u1 = Vector2.Dot ( e1, B1 - Q );

					// Is the circle in Region AB of the previous edge?
					if ( u1 > 0.0f )
					{
						return;
					}
				}

				cf.IndexA = 0;
				cf.TypeA = ContactFeatureType.Vertex;
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.Circles;
				manifold.LocalNormal = new Vector2 ();
				manifold.LocalPoint = P;
				manifold.Points [ 0 ].Id.Key = 0;
				manifold.Points [ 0 ].Id.ContactFeature = cf;
				manifold.Points [ 0 ].LocalPoint = circleB.m_p;
				return;
			}

			// Region B
			if ( u <= 0.0f )
			{
				Vector2 P = B;
				Vector2 d = Q - P;
				float dd = Vector2.Dot ( d, d );
				if ( dd > radius * radius )
				{
					return;
				}

				// Is there an edge connected to B?
				if ( edgeA.m_hasVertex3 )
				{
					Vector2 B2 = edgeA.m_vertex3;
					Vector2 A2 = B;
					Vector2 e2 = B2 - A2;
					float v2 = Vector2.Dot ( e2, Q - A2 );

					// Is the circle in Region AB of the next edge?
					if ( v2 > 0.0f )
					{
						return;
					}
				}

				cf.IndexA = 1;
				cf.TypeA = ContactFeatureType.Vertex;
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.Circles;
				manifold.LocalNormal = new Vector2 ();
				manifold.LocalPoint = P;
				manifold.Points [ 0 ].Id.Key = 0;
				manifold.Points [ 0 ].Id.ContactFeature = cf;
				manifold.Points [ 0 ].LocalPoint = circleB.m_p;
				return;
			}

			// Region AB
			float den = Vector2.Dot ( e, e );
			Vector2 P2 = ( 1.0f / den ) * ( u * A + v * B );
			Vector2 d2 = Q - P2;
			float dd2 = Vector2.Dot ( d2, d2 );
			if ( dd2 > radius * radius )
			{
				return;
			}

			Vector2 n = new Vector2 ( -e.Y, e.X );
			if ( Vector2.Dot ( n, Q - A ) < 0.0f )
			{
				n = new Vector2 ( -n.X, -n.Y );
			}
			n.Normalize ();

			cf.IndexA = 0;
			cf.TypeA = ContactFeatureType.Face;
			manifold.PointCount = 1;
			manifold.Type = ManifoldType.FaceA;
			manifold.LocalNormal = n;
			manifold.LocalPoint = A;
			manifold.Points [ 0 ].Id.Key = 0;
			manifold.Points [ 0 ].Id.ContactFeature = cf;
			manifold.Points [ 0 ].LocalPoint = circleB.m_p;
		}

		public static void CollideEdgeAndPolygon ( out Manifold manifold, EdgeShape edgeA, Transform xfA, PolygonShape polygonB, Transform xfB )
		{
			EPCollider collider = new EPCollider ();
			collider.m_polygonB.Initialize ();
			collider.Collide ( out manifold, edgeA, xfA, polygonB, xfB );
		}

		enum EPAxisType
		{
			Unknown,
			EdgeA,
			EdgeB
		}

		struct EPAxis
		{
			public EPAxisType Type;
			public int Index;
			public float Separation;
		}

		struct TempPolygon
		{
			public Vector2 [] Vertices;
			public Vector2 [] Normals;
			public int Count;

			public void Initialize ()
			{
				Vertices = new Vector2 [ Settings.MaxPolygonVertices ];
				Normals = new Vector2 [ Settings.MaxPolygonVertices ];
				Count = 0;
			}
		}

		struct ReferenceFace
		{
			public int I1, I2;
			public Vector2 V1, V2;
			public Vector2 Normal;
			public Vector2 SideNormal1;
			public float SideOffset1;
			public Vector2 SideNormal2;
			public float SideOffset2;
		}

		struct EPCollider
		{
			internal TempPolygon m_polygonB;

			Transform m_xf;
			Vector2 m_centroidB;
			Vector2 m_v0, m_v1, m_v2, m_v3;
			Vector2 m_normal0, m_normal1, m_normal2;
			Vector2 m_normal;

			enum VertexType
			{
				Isolated,
				Concave,
				Convex,
			}

			//VertexType m_type1, m_type2;
			Vector2 m_lowerLimit, m_upperLimit;
			float m_radius;
			bool m_front;

			public void Collide ( out Manifold manifold, EdgeShape edgeA, Transform xfA, PolygonShape polygonB, Transform xfB )
			{
				manifold = Manifold.Identity;

				m_xf = Transform.TransposeMultiply ( xfA, xfB );

				m_centroidB = m_xf * polygonB.m_centroid;

				m_v0 = edgeA.m_vertex0;
				m_v1 = edgeA.m_vertex1;
				m_v2 = edgeA.m_vertex2;
				m_v3 = edgeA.m_vertex3;

				bool hasVertex0 = edgeA.m_hasVertex0;
				bool hasVertex3 = edgeA.m_hasVertex3;

				Vector2 edge1 = ( m_v2 - m_v1 ).Normalize ();
				m_normal1 = new Vector2 ( edge1.Y, -edge1.X );
				float offset1 = Vector2.Dot ( m_normal1, m_centroidB - m_v1 );
				float offset0 = 0.0f, offset2 = 0.0f;
				bool convex1 = false, convex2 = false;

				if ( hasVertex0 )
				{
					Vector2 edge0 = ( m_v1 - m_v0 ).Normalize ();
					m_normal0 = new Vector2 ( edge0.Y, -edge0.X );
					convex1 = Vector2.Cross ( edge0, edge1 ) >= 0.0f;
					offset0 = Vector2.Dot ( m_normal0, m_centroidB - m_v0 );
				}

				if ( hasVertex3 )
				{
					Vector2 edge2 = ( m_v3 - m_v2 ).Normalize ();
					m_normal2 = new Vector2 ( edge2.Y, -edge2.X );
					convex2 = Vector2.Cross ( edge1, edge2 ) > 0.0f;
					offset2 = Vector2.Dot ( m_normal2, m_centroidB - m_v2 );
				}

				if ( hasVertex0 && hasVertex3 )
				{
					if ( convex1 && convex2 )
					{
						m_front = offset0 >= 0.0f || offset1 >= 0.0f || offset2 >= 0.0f;
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = m_normal0;
							m_upperLimit = m_normal2;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = -m_normal1;
							m_upperLimit = -m_normal1;
						}
					}
					else if ( convex1 )
					{
						m_front = offset0 >= 0.0f || ( offset1 >= 0.0f && offset2 >= 0.0f );
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = m_normal0;
							m_upperLimit = m_normal1;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = -m_normal2;
							m_upperLimit = -m_normal1;
						}
					}
					else if ( convex2 )
					{
						m_front = offset2 >= 0.0f || ( offset0 >= 0.0f && offset1 >= 0.0f );
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = m_normal1;
							m_upperLimit = m_normal2;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = -m_normal1;
							m_upperLimit = -m_normal0;
						}
					}
					else
					{
						m_front = offset0 >= 0.0f && offset1 >= 0.0f && offset2 >= 0.0f;
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = m_normal1;
							m_upperLimit = m_normal1;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = -m_normal2;
							m_upperLimit = -m_normal0;
						}
					}
				}
				else if ( hasVertex0 )
				{
					if ( convex1 )
					{
						m_front = offset0 >= 0.0f || offset1 >= 0.0f;
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = m_normal0;
							m_upperLimit = -m_normal1;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = m_normal1;
							m_upperLimit = -m_normal1;
						}
					}
					else
					{
						m_front = offset0 >= 0.0f && offset1 >= 0.0f;
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = m_normal1;
							m_upperLimit = -m_normal1;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = m_normal1;
							m_upperLimit = -m_normal0;
						}
					}
				}
				else if ( hasVertex3 )
				{
					if ( convex2 )
					{
						m_front = offset1 >= 0.0f || offset2 >= 0.0f;
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = -m_normal1;
							m_upperLimit = m_normal2;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = -m_normal1;
							m_upperLimit = m_normal1;
						}
					}
					else
					{
						m_front = offset1 >= 0.0f && offset2 >= 0.0f;
						if ( m_front )
						{
							m_normal = m_normal1;
							m_lowerLimit = -m_normal1;
							m_upperLimit = m_normal1;
						}
						else
						{
							m_normal = -m_normal1;
							m_lowerLimit = -m_normal2;
							m_upperLimit = m_normal1;
						}
					}
				}
				else
				{
					m_front = offset1 >= 0.0f;
					if ( m_front )
					{
						m_normal = m_normal1;
						m_lowerLimit = -m_normal1;
						m_upperLimit = -m_normal1;
					}
					else
					{
						m_normal = -m_normal1;
						m_lowerLimit = m_normal1;
						m_upperLimit = m_normal1;
					}
				}

				// Get polygonB in frameA
				m_polygonB.Count = polygonB.m_vertexCount;
				for ( int i = 0; i < polygonB.m_vertexCount; ++i )
				{
					m_polygonB.Vertices [ i ] = m_xf * polygonB.m_vertices [ i ];
					m_polygonB.Normals [ i ] = m_xf.q * polygonB.m_normals [ i ];
				}

				m_radius = 2.0f * Settings.PolygonRadius;

				manifold.PointCount = 0;

				EPAxis edgeAxis = ComputeEdgeSeparation ();

				// If no valid normal can be found than this edge should not collide.
				if ( edgeAxis.Type == EPAxisType.Unknown )
				{
					return;
				}

				if ( edgeAxis.Separation > m_radius )
				{
					return;
				}

				EPAxis polygonAxis = ComputePolygonSeparation ();
				if ( polygonAxis.Type != EPAxisType.Unknown && polygonAxis.Separation > m_radius )
				{
					return;
				}

				// Use hysteresis for jitter reduction.
				float k_relativeTol = 0.98f;
				float k_absoluteTol = 0.001f;

				EPAxis primaryAxis;
				if ( polygonAxis.Type == EPAxisType.Unknown )
				{
					primaryAxis = edgeAxis;
				}
				else if ( polygonAxis.Separation > k_relativeTol * edgeAxis.Separation + k_absoluteTol )
				{
					primaryAxis = polygonAxis;
				}
				else
				{
					primaryAxis = edgeAxis;
				}

				ClipVertex [] ie = new ClipVertex [ 2 ];
				ReferenceFace rf = new ReferenceFace ();
				if ( primaryAxis.Type == EPAxisType.EdgeA )
				{
					manifold.Type = ManifoldType.FaceA;

					// Search for the polygon normal that is most anti-parallel to the edge normal.
					int bestIndex = 0;
					float bestValue = Vector2.Dot ( m_normal, m_polygonB.Normals [ 0 ] );
					for ( int i = 1; i < m_polygonB.Count; ++i )
					{
						float value = Vector2.Dot ( m_normal, m_polygonB.Normals [ i ] );
						if ( value < bestValue )
						{
							bestValue = value;
							bestIndex = i;
						}
					}

					int i1 = bestIndex;
					int i2 = i1 + 1 < m_polygonB.Count ? i1 + 1 : 0;

					ie [ 0 ].Vertex = m_polygonB.Vertices [ i1 ];
					ie [ 0 ].Id.ContactFeature.IndexA = 0;
					ie [ 0 ].Id.ContactFeature.IndexB = i1;
					ie [ 0 ].Id.ContactFeature.TypeA = ContactFeatureType.Face;
					ie [ 0 ].Id.ContactFeature.TypeB = ContactFeatureType.Vertex;

					ie [ 1 ].Vertex = m_polygonB.Vertices [ i2 ];
					ie [ 1 ].Id.ContactFeature.IndexA = 0;
					ie [ 1 ].Id.ContactFeature.IndexB = i2;
					ie [ 1 ].Id.ContactFeature.TypeA = ContactFeatureType.Face;
					ie [ 1 ].Id.ContactFeature.TypeB = ContactFeatureType.Vertex;

					if ( m_front )
					{
						rf.I1 = 0;
						rf.I2 = 1;
						rf.V1 = m_v1;
						rf.V2 = m_v2;
						rf.Normal = m_normal1;
					}
					else
					{
						rf.I1 = 1;
						rf.I2 = 0;
						rf.V1 = m_v2;
						rf.V2 = m_v1;
						rf.Normal = -m_normal1;
					}
				}
				else
				{
					manifold.Type = ManifoldType.FaceB;

					ie [ 0 ].Vertex = m_v1;
					ie [ 0 ].Id.ContactFeature.IndexA = 0;
					ie [ 0 ].Id.ContactFeature.IndexB = primaryAxis.Index;
					ie [ 0 ].Id.ContactFeature.TypeA = ContactFeatureType.Vertex;
					ie [ 0 ].Id.ContactFeature.TypeB = ContactFeatureType.Face;

					ie [ 1 ].Vertex = m_v2;
					ie [ 1 ].Id.ContactFeature.IndexA = 0;
					ie [ 1 ].Id.ContactFeature.IndexB = primaryAxis.Index;
					ie [ 1 ].Id.ContactFeature.TypeA = ContactFeatureType.Vertex;
					ie [ 1 ].Id.ContactFeature.TypeB = ContactFeatureType.Face;

					rf.I1 = primaryAxis.Index;
					rf.I2 = rf.I1 + 1 < m_polygonB.Count ? rf.I1 + 1 : 0;
					rf.V1 = m_polygonB.Vertices [ rf.I1 ];
					rf.V2 = m_polygonB.Vertices [ rf.I2 ];
					rf.Normal = m_polygonB.Normals [ rf.I1 ];
				}

				rf.SideNormal1 = new Vector2 ( rf.Normal.Y, -rf.Normal.X );
				rf.SideNormal2 = -rf.SideNormal1;
				rf.SideOffset1 = Vector2.Dot ( rf.SideNormal1, rf.V1 );
				rf.SideOffset2 = Vector2.Dot ( rf.SideNormal2, rf.V2 );

				// Clip incident edge against extruded edge1 side edges.
				ClipVertex [] clipPoints1 = new ClipVertex [ 2 ];
				ClipVertex [] clipPoints2 = new ClipVertex [ 2 ];
				int np;

				// Clip to box side 1
				np = ClipVertex.ClipSegmentToLine ( clipPoints1, ie, rf.SideNormal1, rf.SideOffset1, rf.I1 );

				if ( np < Settings.MaxManifoldPoints )
				{
					return;
				}

				// Clip to negative box side 1
				np = ClipVertex.ClipSegmentToLine ( clipPoints2, clipPoints1, rf.SideNormal2, rf.SideOffset2, rf.I2 );

				if ( np < Settings.MaxManifoldPoints )
				{
					return;
				}

				// Now clipPoints2 contains the clipped points.
				if ( primaryAxis.Type == EPAxisType.EdgeA )
				{
					manifold.LocalNormal = rf.Normal;
					manifold.LocalPoint = rf.V1;
				}
				else
				{
					manifold.LocalNormal = polygonB.m_normals [ rf.I1 ];
					manifold.LocalPoint = polygonB.m_vertices [ rf.I1 ];
				}

				int pointCount = 0;
				for ( int i = 0; i < Settings.MaxManifoldPoints; ++i )
				{
					float separation;

					separation = Vector2.Dot ( rf.Normal, clipPoints2 [ i ].Vertex - rf.V1 );

					if ( separation <= m_radius )
					{
						ManifoldPoint cp = manifold.Points [ pointCount ];

						if ( primaryAxis.Type == EPAxisType.EdgeA )
						{
							cp.LocalPoint = Transform.TransposeMultiply ( m_xf, clipPoints2 [ i ].Vertex );
							cp.Id = clipPoints2 [ i ].Id;
						}
						else
						{
							cp.LocalPoint = clipPoints2 [ i ].Vertex;
							cp.Id.ContactFeature.TypeA = clipPoints2 [ i ].Id.ContactFeature.TypeB;
							cp.Id.ContactFeature.TypeB = clipPoints2 [ i ].Id.ContactFeature.TypeA;
							cp.Id.ContactFeature.IndexA = clipPoints2 [ i ].Id.ContactFeature.IndexB;
							cp.Id.ContactFeature.IndexB = clipPoints2 [ i ].Id.ContactFeature.IndexA;
						}

						manifold.Points [ pointCount ] = cp;
						++pointCount;
					}
				}

				manifold.PointCount = pointCount;
			}

			public EPAxis ComputeEdgeSeparation ()
			{
				EPAxis axis = new EPAxis ();
				axis.Type = EPAxisType.EdgeA;
				axis.Index = m_front ? 0 : 1;
				axis.Separation = float.MaxValue;

				for ( int i = 0; i < m_polygonB.Count; ++i )
				{
					float s = Vector2.Dot ( m_normal, m_polygonB.Vertices [ i ] - m_v1 );
					if ( s < axis.Separation )
					{
						axis.Separation = s;
					}
				}

				return axis;
			}

			public EPAxis ComputePolygonSeparation ()
			{
				EPAxis axis = new EPAxis ();
				axis.Type = EPAxisType.Unknown;
				axis.Index = -1;
				axis.Separation = -float.MaxValue;

				Vector2 perp = new Vector2 ( -m_normal.Y, m_normal.X );

				for ( int i = 0; i < m_polygonB.Count; ++i )
				{
					Vector2 n = -m_polygonB.Normals [ i ];

					float s1 = Vector2.Dot ( n, m_polygonB.Vertices [ i ] - m_v1 );
					float s2 = Vector2.Dot ( n, m_polygonB.Vertices [ i ] - m_v2 );
					float s = Math.Min ( s1, s2 );

					if ( s > m_radius )
					{
						// No collision
						axis.Type = EPAxisType.EdgeB;
						axis.Index = i;
						axis.Separation = s;
						return axis;
					}

					// Adjacency
					if ( Vector2.Dot ( n, perp ) >= 0.0f )
					{
						if ( Vector2.Dot ( n - m_upperLimit, m_normal ) < -Settings.AngularSlop )
						{
							continue;
						}
					}
					else
					{
						if ( Vector2.Dot ( n - m_lowerLimit, m_normal ) < -Settings.AngularSlop )
						{
							continue;
						}
					}

					if ( s > axis.Separation )
					{
						axis.Type = EPAxisType.EdgeB;
						axis.Index = i;
						axis.Separation = s;
					}
				}

				return axis;
			}
		}
	}
}
