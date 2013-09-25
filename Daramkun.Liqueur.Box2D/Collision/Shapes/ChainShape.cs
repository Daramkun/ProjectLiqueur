using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Box2D.Common;

namespace Daramkun.Liqueur.Box2D.Collision.Shapes
{
	public class ChainShape : Shape
	{
		Vector2 [] m_vertices;
		int m_count;
		
		Vector2 m_prevVertex, m_nextVertex;
		bool m_hasPrevVertex, m_hasNextVertex;

		public ChainShape ()
		{
			ShapeType = ShapeType.Chain;
			m_radius = Settings.PolygonRadius;
		}

		public void CreateLoop ( Vector2 [] vertices )
		{
			m_count = vertices.Length + 1;
			m_vertices = new Vector2 [ m_count ];
			vertices.CopyTo ( m_vertices, 0 );
			m_vertices [ vertices.Length ] = m_vertices [ 0 ];
			m_prevVertex = m_vertices [ m_count - 2 ];
			m_nextVertex = m_vertices [ 1 ];
			m_hasPrevVertex = true;
			m_hasNextVertex = true;
		}

		public void CreateChain ( Vector2 [] vertices )
		{
			m_count = vertices.Length;
			m_vertices = new Vector2 [ m_count ];
			vertices.CopyTo ( m_vertices, 0 );
			m_hasPrevVertex = false;
			m_hasNextVertex = false;
		}

		public void SetPreviousVertex ( Vector2 prevVertex )
		{
			m_prevVertex = prevVertex;
			m_hasPrevVertex = true;
		}

		public void SetNextVertex ( Vector2 nextVertex )
		{
			m_nextVertex = nextVertex;
			m_hasNextVertex = true;
		}

		public override Shape Clone ()
		{
			ChainShape clone = new ChainShape ();
			clone.CreateChain ( m_vertices );
			clone.m_prevVertex = m_prevVertex;
			clone.m_nextVertex = m_nextVertex;
			clone.m_hasPrevVertex = m_hasPrevVertex;
			clone.m_hasNextVertex = m_hasNextVertex;
			return clone;
		}

		public override int ChildCount { get { return m_count - 1; } }

		public EdgeShape ChildEdge
		{
			get
			{
				EdgeShape edge = new EdgeShape ();
				edge.ShapeType = Shapes.ShapeType.Edge;
				edge.m_radius = m_radius;
				return edge;
			}
		}

		public override bool TestPoint ( Transform xf, Vector2 p )
		{
			return false;
		}

		public override bool RayCast ( out RayCastOutput output, RayCastInput input, Transform transform, int childIndex )
		{
			EdgeShape edgeShape = new EdgeShape ();
			int i1 = childIndex;
			int i2 = childIndex + 1;
			if ( i2 == m_count ) i2 = 0;

			return edgeShape.RayCast ( out output, input, transform, 0 );
		}

		public override void ComputeAABB ( out AABB aabb, Transform xf, int childIndex )
		{
			int i1 = childIndex;
			int i2 = childIndex + 1;
			
			if ( i2 == m_count ) i2 = 0;

			Vector2 v1 = xf * m_vertices [ i1 ];
			Vector2 v2 = xf * m_vertices [ i2 ];

			aabb = new AABB ();
			aabb.LowerBound = new Vector2 ( ( float ) Math.Min ( v1.X, v2.X ), ( float ) Math.Min ( v1.Y, v2.Y ) );
			aabb.UpperBound = new Vector2 ( ( float ) Math.Max ( v1.X, v2.X ), ( float ) Math.Max ( v1.Y, v2.Y ) );
		}

		public override void ComputeMass ( out MassData massData, float density )
		{
			massData = new MassData ();
			massData.Mass = 0;
			massData.Center = new Vector2 ();
			massData.Inertia = 0;
		}
	}
}
