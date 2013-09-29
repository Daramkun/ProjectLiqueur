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
		public static void CollidePolygons ( out Manifold manifold, PolygonShape polyA, Transform xfA, PolygonShape polyB, Transform xfB )
		{
			manifold = Manifold.Identity;
			manifold.PointCount = 0;
			float totalRadius = polyA.m_radius + polyB.m_radius;

			int edgeA = 0;
			float separationA = FindMaxSeparation ( out edgeA, polyA, xfA, polyB, xfB );
			if ( separationA > totalRadius )
				return;

			int edgeB = 0;
			float separationB = FindMaxSeparation ( out edgeB, polyB, xfB, polyA, xfA );
			if ( separationB > totalRadius )
				return;

			PolygonShape poly1;	// reference polygon
			PolygonShape poly2;	// incident polygon
			Transform xf1, xf2;
			int edge1;		// reference edge
			byte flip;
			const float k_relativeTol = 0.98f;
			const float k_absoluteTol = 0.001f;

			if ( separationB > k_relativeTol * separationA + k_absoluteTol )
			{
				poly1 = polyB;
				poly2 = polyA;
				xf1 = xfB;
				xf2 = xfA;
				edge1 = edgeB;
				manifold.Type = ManifoldType.FaceB;
				flip = 1;
			}
			else
			{
				poly1 = polyA;
				poly2 = polyB;
				xf1 = xfA;
				xf2 = xfB;
				edge1 = edgeA;
				manifold.Type = ManifoldType.FaceA;
				flip = 0;
			}

			ClipVertex [] incidentEdge = new ClipVertex [ 2 ];
			FindIncidentEdge ( incidentEdge, poly1, xf1, edge1, poly2, xf2 );

			int count1 = poly1.m_vertexCount;
			Vector2 [] vertices1 = poly1.m_vertices;

			int iv1 = edge1;
			int iv2 = edge1 + 1 < count1 ? edge1 + 1 : 0;

			Vector2 v11 = vertices1 [ iv1 ];
			Vector2 v12 = vertices1 [ iv2 ];

			Vector2 localTangent = v12 - v11;
			localTangent.Normalize ();

			Vector2 localNormal = Vector2.Cross ( localTangent, 1.0f );
			Vector2 planePoint = 0.5f * ( v11 + v12 );

			Vector2 tangent = xf1.q * localTangent;
			Vector2 normal = Vector2.Cross ( tangent, 1.0f );

			v11 = xf1 * v11;
			v12 = xf1 * v12;

			// Face offset.
			float frontOffset = Vector2.Dot ( normal, v11 );

			// Side offsets, extended by polytope skin thickness.
			float sideOffset1 = -Vector2.Dot ( tangent, v11 ) + totalRadius;
			float sideOffset2 = Vector2.Dot ( tangent, v12 ) + totalRadius;

			// Clip incident edge against extruded edge1 side edges.
			ClipVertex [] clipPoints1 = new ClipVertex [ 2 ];
			ClipVertex [] clipPoints2 = new ClipVertex [ 2 ];
			int np;

			// Clip to box side 1
			np = ClipVertex.ClipSegmentToLine ( clipPoints1, incidentEdge, -tangent, sideOffset1, iv1 );

			if ( np < 2 )
				return;

			// Clip to negative box side 1
			np = ClipVertex.ClipSegmentToLine ( clipPoints2, clipPoints1, tangent, sideOffset2, iv2 );

			if ( np < 2 )
			{
				return;
			}

			// Now clipPoints2 contains the clipped points.
			manifold.LocalNormal = localNormal;
			manifold.LocalPoint = planePoint;

			int pointCount = 0;
			for ( int i = 0; i < Settings.MaxManifoldPoints; ++i )
			{
				float separation = Vector2.Dot ( normal, clipPoints2 [ i ].Vertex ) - frontOffset;

				if ( separation <= totalRadius )
				{
					ManifoldPoint cp = manifold.Points [ pointCount ];
					cp.LocalPoint = xf2 * clipPoints2 [ i ].Vertex;
					cp.Id = clipPoints2 [ i ].Id;
					if ( flip != 0 )
					{
						// Swap features
						ContactFeature cf = cp.Id.ContactFeature;
						cp.Id.ContactFeature.IndexA = cf.IndexB;
						cp.Id.ContactFeature.IndexB = cf.IndexA;
						cp.Id.ContactFeature.TypeA = cf.TypeB;
						cp.Id.ContactFeature.TypeB = cf.TypeA;
					}
					manifold.Points [ pointCount ] = cp;
					++pointCount;
				}
			}

			manifold.PointCount = pointCount;
		}

		static float EdgeSeparation ( PolygonShape poly1, Transform xfA, int edge1, PolygonShape poly2, Transform xfB )
		{
			Vector2 [] vertices1 = poly1.m_vertices;
			Vector2 [] normals1 = poly1.m_normals;

			int count2 = poly2.m_vertexCount;
			Vector2 [] vertices2 = poly2.m_vertices;

			// Convert normal from poly1's frame into poly2's frame.
			Vector2 normal1World = xfA.q * normals1 [ edge1 ];
			Vector2 normal1 = Rotation.TransposeMultiply ( xfB.q, normal1World );

			// Find support vertex on poly2 for -normal.
			int index = 0;
			float minDot = float.MaxValue;

			for ( int i = 0; i < count2; ++i )
			{
				float dot = Vector2.Dot ( vertices2 [ i ], normal1 );
				if ( dot < minDot )
				{
					minDot = dot;
					index = i;
				}
			}

			Vector2 v1 = xfA * vertices1 [ edge1 ];
			Vector2 v2 = xfB * vertices2 [ index ];
			float separation = Vector2.Dot ( v2 - v1, normal1World );
			return separation;
		}

		static float FindMaxSeparation ( out int edgeIndex, PolygonShape poly1, Transform xfA, PolygonShape poly2, Transform xfB )
		{
			int count1 = poly1.m_vertexCount;
			Vector2 [] normals1 = poly1.m_normals;

			Vector2 d = ( xfB * poly2.m_centroid ) - ( xfA * poly1.m_centroid );
			Vector2 dLocal1 = Rotation.TransposeMultiply ( xfA.q, d );

			// Find edge normal on poly1 that has the largest projection onto d.
			int edge = 0;
			float maxDot = -float.MaxValue;
			for ( int i = 0; i < count1; ++i )
			{
				float dot = Vector2.Dot ( normals1 [ i ], dLocal1 );
				if ( dot > maxDot )
				{
					maxDot = dot;
					edge = i;
				}
			}

			// Get the separation for the edge normal.
			float s = EdgeSeparation ( poly1, xfA, edge, poly2, xfB );

			// Check the separation for the previous edge normal.
			int prevEdge = edge - 1 >= 0 ? edge - 1 : count1 - 1;
			float sPrev = EdgeSeparation ( poly1, xfA, prevEdge, poly2, xfB );

			// Check the separation for the next edge normal.
			int nextEdge = edge + 1 < count1 ? edge + 1 : 0;
			float sNext = EdgeSeparation ( poly1, xfA, nextEdge, poly2, xfB );

			// Find the best edge and the search direction.
			int bestEdge;
			float bestSeparation;
			int increment;
			if ( sPrev > s && sPrev > sNext )
			{
				increment = -1;
				bestEdge = prevEdge;
				bestSeparation = sPrev;
			}
			else if ( sNext > s )
			{
				increment = 1;
				bestEdge = nextEdge;
				bestSeparation = sNext;
			}
			else
			{
				edgeIndex = edge;
				return s;
			}

			// Perform a local search for the best edge normal.
			for ( ; ; )
			{
				if ( increment == -1 )
					edge = bestEdge - 1 >= 0 ? bestEdge - 1 : count1 - 1;
				else
					edge = bestEdge + 1 < count1 ? bestEdge + 1 : 0;

				s = EdgeSeparation ( poly1, xfA, edge, poly2, xfB );

				if ( s > bestSeparation )
				{
					bestEdge = edge;
					bestSeparation = s;
				}
				else
				{
					break;
				}
			}

			edgeIndex = bestEdge;
			return bestSeparation;
		}

		static void FindIncidentEdge ( ClipVertex [] c, PolygonShape poly1, Transform xfA, int edge1, PolygonShape poly2, Transform xfB )
		{
			Vector2 [] normals1 = poly1.m_normals;

			int count2 = poly2.m_vertexCount;
			Vector2 [] vertices2 = poly2.m_vertices;
			Vector2 [] normals2 = poly2.m_normals;

			// Get the normal of the reference edge in poly2's frame.
			Vector2 normal1 = Rotation.TransposeMultiply ( xfB.q, xfA.q * normals1 [ edge1 ] );

			// Find the incident edge on poly2.
			int index = 0;
			float minDot = float.MaxValue;
			for ( int i = 0; i < count2; ++i )
			{
				float dot = Vector2.Dot ( normal1, normals2 [ i ] );
				if ( dot < minDot )
				{
					minDot = dot;
					index = i;
				}
			}

			// Build the clip vertices for the incident edge.
			int i1 = index;
			int i2 = i1 + 1 < count2 ? i1 + 1 : 0;

			c [ 0 ].Vertex = xfB * vertices2 [ i1 ];
			c [ 0 ].Id.ContactFeature.IndexA = edge1;
			c [ 0 ].Id.ContactFeature.IndexB = i1;
			c [ 0 ].Id.ContactFeature.TypeA = ContactFeatureType.Face;
			c [ 0 ].Id.ContactFeature.TypeB = ContactFeatureType.Vertex;

			c [ 1 ].Vertex = xfB * vertices2 [ i2 ];
			c [ 1 ].Id.ContactFeature.IndexA = edge1;
			c [ 1 ].Id.ContactFeature.IndexB = i2;
			c [ 1 ].Id.ContactFeature.TypeA = ContactFeatureType.Face;
			c [ 1 ].Id.ContactFeature.TypeB = ContactFeatureType.Vertex;
		}
	}
}
