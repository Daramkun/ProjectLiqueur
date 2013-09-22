using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Box2D.Common;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Collision
{
	public enum ContactFeatureType
	{
		Vertex = 0,
		Face = 1,
	}

	public struct ContactFeature
	{
		public byte IndexA, IndexB;
		public byte TypeA, TypeB;
	}

	public struct ContactID
	{
		public ContactFeature ContactFeature;
		public uint Key;
	}

	public struct ManifoldPoint
	{
		public Vector2 LocalPoint;
		public float NormalImpulse;
		public float TangentImpulse;
		public ContactID Id;
	}

	public enum ManifoldType
	{
		Circles,
		FaceA,
		FaceB,
	}

	public struct Manifold
	{
		public ManifoldPoint [] Points;
		public Vector2 LocalNormal;
		public Vector2 LocalPoint;
		public ManifoldType Type;
		public int PointCount;
	}

	public struct WorldManifold
	{
		public Vector2 Normal;
		public Vector2 [] Points;

		public void Initialize ( Manifold manifold, Transform xfA, float radiusA, Transform xfB, float radiusB )
		{
			
		}
	}

	public enum PointState
	{
		NullState,
		AddState,
		PersistState,
		RemoveState,
	}


}
