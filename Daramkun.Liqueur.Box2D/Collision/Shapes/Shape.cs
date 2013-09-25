using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Box2D.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Collision.Shapes
{
	public struct MassData
	{
		public float Mass { get; set; }
		public Vector2 Center { get; set; }
		public float Inertia { get; set; }
	}

	public enum ShapeType
	{
		Circle = 0,
		Edge = 1,
		Polygon = 2,
		Chain = 3,
		TypeCount = 4
	}

	public abstract class Shape
	{
		internal float m_radius;

		public ShapeType ShapeType { get; internal set; }
		public abstract int ChildCount { get; }

		public abstract bool TestPoint ( Transform xf, Vector2 p );
		public abstract bool RayCast ( out RayCastOutput output, RayCastInput input, Transform transform, int childIndex );
		public abstract void ComputeAABB ( out AABB aabb, Transform xf, int childIndex );
		public abstract void ComputeMass ( out MassData massData, float density );

		public virtual Shape Clone () { return null; }
	}
}
