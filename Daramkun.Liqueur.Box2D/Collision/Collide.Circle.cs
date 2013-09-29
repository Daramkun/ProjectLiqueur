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
		public static void CollideCircles ( out Manifold manifold, CircleShape circleA, Transform xfA, CircleShape circleB, Transform xfB )
		{
			manifold = Manifold.Identity;
			manifold.PointCount = 0;

			Vector2 pA = xfA * circleA.m_p;
			Vector2 pB = xfB * circleB.m_p;

			Vector2 d = pB - pA;
			float distSqr = Vector2.Dot ( d, d );
			float rA = circleA.m_radius, rB = circleB.m_radius;
			float radius = rA + rB;
			if ( distSqr > radius * radius )
			{
				return;
			}

			manifold.Type = ManifoldType.Circles;
			manifold.LocalPoint = circleA.m_p;
			manifold.LocalNormal = new Vector2 ();
			manifold.PointCount = 1;

			manifold.Points [ 0 ].LocalPoint = circleB.m_p;
			manifold.Points [ 0 ].Id.Key = 0;
		}

		public static void CollidePolygonAndCircle ( out Manifold manifold, PolygonShape polygonA, Transform xfA, CircleShape circleB, Transform xfB )
		{
			manifold = Manifold.Identity;
			manifold.PointCount = 0;

			// Compute circle position in the frame of the polygon.
			Vector2 c = xfB * circleB.m_p;
			Vector2 cLocal = Transform.TransposeMultiply ( xfA, c );

			// Find the min separating edge.
			int normalIndex = 0;
			float separation = -float.MaxValue;
			float radius = polygonA.m_radius + circleB.m_radius;
			int vertexCount = polygonA.m_vertexCount;
			Vector2 [] vertices = polygonA.m_vertices;
			Vector2 [] normals = polygonA.m_normals;

			for ( int i = 0; i < vertexCount; ++i )
			{
				float s = Vector2.Dot ( normals [ i ], cLocal - vertices [ i ] );

				if ( s > radius )
				{
					// Early out.
					return;
				}

				if ( s > separation )
				{
					separation = s;
					normalIndex = i;
				}
			}

			// Vertices that subtend the incident face.
			int vertIndex1 = normalIndex;
			int vertIndex2 = vertIndex1 + 1 < vertexCount ? vertIndex1 + 1 : 0;
			Vector2 v1 = vertices [ vertIndex1 ];
			Vector2 v2 = vertices [ vertIndex2 ];

			// If the center is inside the polygon ...
			if ( separation < float.Epsilon )
			{
				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = normals [ normalIndex ];
				manifold.LocalPoint = 0.5f * ( v1 + v2 );
				manifold.Points [ 0 ].LocalPoint = circleB.m_p;
				manifold.Points [ 0 ].Id.Key = 0;
				return;
			}

			// Compute barycentric coordinates
			float u1 = Vector2.Dot ( cLocal - v1, v2 - v1 );
			float u2 = Vector2.Dot ( cLocal - v2, v1 - v2 );
			if ( u1 <= 0.0f )
			{
				if ( Math.Pow ( Vector2.Distance ( cLocal, v1 ), 2 ) > radius * radius )
				{
					return;
				}

				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = cLocal - v1;
				manifold.LocalNormal.Normalize ();
				manifold.LocalPoint = v1;
				manifold.Points [ 0 ].LocalPoint = circleB.m_p;
				manifold.Points [ 0 ].Id.Key = 0;
			}
			else if ( u2 <= 0.0f )
			{
				if ( Math.Pow ( Vector2.Distance ( cLocal, v2 ), 2 ) > radius * radius )
				{
					return;
				}

				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = cLocal - v2;
				manifold.LocalNormal.Normalize ();
				manifold.LocalPoint = v2;
				manifold.Points [ 0 ].LocalPoint = circleB.m_p;
				manifold.Points [ 0 ].Id.Key = 0;
			}
			else
			{
				Vector2 faceCenter = 0.5f * ( v1 + v2 );
				float separationn = Vector2.Dot ( cLocal - faceCenter, normals [ vertIndex1 ] );
				if ( separationn > radius )
				{
					return;
				}

				manifold.PointCount = 1;
				manifold.Type = ManifoldType.FaceA;
				manifold.LocalNormal = normals [ vertIndex1 ];
				manifold.LocalPoint = faceCenter;
				manifold.Points [ 0 ].LocalPoint = circleB.m_p;
				manifold.Points [ 0 ].Id.Key = 0;
			}
		}
	}
}
