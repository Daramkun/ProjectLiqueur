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

		public static Manifold Identity
		{
			get { return new Manifold () { Points = new ManifoldPoint [ Settings.MaxManifoldPoints ] }; }
		}

		public static void GetPointStates()
		{

		}
	}

	public struct WorldManifold
	{
		public Vector2 Normal;
		public Vector2 [] Points;

		public static WorldManifold Identity
		{
			get { return new WorldManifold () { Points = new Vector2 [ Settings.MaxManifoldPoints ] }; }
		}

		public void Initialize ( Manifold manifold, Transform xfA, float radiusA, Transform xfB, float radiusB )
		{
			if ( manifold.PointCount == 0 ) return;

			switch ( manifold.Type )
			{
				case ManifoldType.Circles:
					{
						Normal = new Vector2 ( 1, 0 );
						Vector2 pointA = xfA * manifold.LocalPoint,
						pointB = xfB * manifold.Points [ 0 ].LocalPoint;
						if ( Math.Pow ( Vector2.Distance ( pointA, pointB ), 2 ) > float.Epsilon * float.Epsilon )
						{
							Normal = pointB - pointA;
							Normal.Normalize ();
						}

						Points [ 0 ] = 0.5f * ( ( pointA + radiusA * Normal ) + ( pointB - radiusB * Normal ) );
					}
					break;

				case ManifoldType.FaceA:
					{
						Normal = xfA.q * manifold.LocalNormal;
						Vector2 planePoint = xfA * manifold.LocalPoint;

						for ( int i = 0; i < manifold.PointCount; ++i )
						{
							Vector2 clipPoint = xfB * manifold.Points [ i ].LocalPoint;
							Vector2 cA = clipPoint + ( radiusA - Vector2.Dot ( clipPoint - planePoint, Normal ) ) * Normal;
							Vector2 cB = clipPoint - radiusB * Normal;
							Points [ i ] = 0.5f * ( cA + cB );
						}
					}
					break;

				case ManifoldType.FaceB:
					{
						Normal = xfB.q * manifold.LocalNormal;
						Vector2 planePoint = xfB * manifold.LocalPoint;

						for ( int i = 0; i < manifold.PointCount; ++i )
						{
							Vector2 clipPoint = xfA * manifold.Points [ i ].LocalPoint;
							Vector2 cB = clipPoint + ( radiusB - Vector2.Dot ( clipPoint - planePoint, Normal ) ) * Normal;
							Vector2 cA = clipPoint - radiusA * Normal;
							Points [ i ] = 0.5f * ( cA + cB );
						}

						Normal = -Normal;
					}
					break;
			}
		}
	}

	public enum PointState
	{
		NullState,
		AddState,
		PersistState,
		RemoveState,
	}

	public struct ClipVertex
	{
		public Vector2 Vertex;
		public ContactID Id;
	}

	public struct RayCastInput
	{
		public Vector2 Point1, Point2;
		public float MaxFraction;
	}

	public struct RayCastOutput
	{
		public Vector2 Normal;
		public float Fraction;
	}

	public struct AABB
	{
		public Vector2 UpperBound, LowerBound;

		public bool IsVaild
		{
			get
			{ 
				Vector2 d = UpperBound - LowerBound;
				bool valid = d.X >= 0.0f && d.Y >= 0.0f;
				valid = valid && LowerBound.IsValid && UpperBound.IsValid;
				return valid;
			}
		}

		public Vector2 Center { get { return 0.5f * ( LowerBound + UpperBound ); } }
		public Vector2 Extents { get { return 0.5f * ( UpperBound - LowerBound ); } }
		public float Perimeter { get {  return 2.0f * ( ( UpperBound.X - LowerBound.X ) + ( UpperBound.Y - LowerBound.Y ) ); } }

		public void Combine ( AABB aabb )
		{
			LowerBound = new Vector2 (
				( float ) Math.Min ( LowerBound.X, aabb.LowerBound.X ),
				( float ) Math.Min ( LowerBound.Y, aabb.LowerBound.Y )
			);
			UpperBound = new Vector2 (
				( float ) Math.Max ( UpperBound.X, aabb.UpperBound.X ),
				( float ) Math.Max ( UpperBound.Y, aabb.UpperBound.Y )
			);
		}

		public void Combine ( AABB aabb1, AABB aabb2 )
		{
			LowerBound = new Vector2 (
				( float ) Math.Min ( aabb1.LowerBound.X, aabb2.LowerBound.X ),
				( float ) Math.Min ( aabb1.LowerBound.Y, aabb2.LowerBound.Y )
				);
			UpperBound = new Vector2 (
				( float ) Math.Max ( aabb1.UpperBound.X, aabb2.UpperBound.X ),
				( float ) Math.Max ( aabb1.UpperBound.Y, aabb2.UpperBound.Y )
				);
		}

		public bool Contains ( AABB aabb )
		{
			bool result = true;
			result = result && LowerBound.X <= aabb.LowerBound.X;
			result = result && LowerBound.Y <= aabb.LowerBound.Y;
			result = result && aabb.UpperBound.X <= UpperBound.X;
			result = result && aabb.UpperBound.Y <= UpperBound.Y;
			return result;
		}

		public bool RayCast ( out RayCastOutput output, RayCastInput input )
		{
			float tmin = -float.MaxValue;
			float tmax = float.MaxValue;

			Vector2 p = input.Point1;
			Vector2 d = input.Point2 - input.Point1;
			Vector2 absD = new Vector2 ( Math.Abs ( d.X ), Math.Abs ( d.Y ) );

			Vector2 normal = new Vector2 ();

			for ( int i = 0; i < 2; ++i )
			{
				if ( absD [ i ] < float.Epsilon )
				{
					if ( p [ i ] < LowerBound [ i ] || UpperBound [ i ] < p [ i ] )
					{
						output = default ( RayCastOutput );
						return false;
					}
				}
				else
				{
					float inv_d = 1.0f / d [ i ];
					float t1 = ( LowerBound [ i ] - p [ i ] ) * inv_d;
					float t2 = ( UpperBound [ i ] - p [ i ] ) * inv_d;

					// Sign of the normal vector.
					float s = -1.0f;

					if ( t1 > t2 )
					{
						float temp = t1;
						t1 = t2;
						t2 = temp;
						s = 1.0f;
					}

					// Push the min up
					if ( t1 > tmin )
					{
						normal = new Vector2 ();
						normal [ i ] = s;
						tmin = t1;
					}

					// Pull the max down
					tmax = ( float ) Math.Min ( tmax, t2 );

					if ( tmin > tmax )
					{
						output = default ( RayCastOutput );
						return false;
					}
				}
			}

			// Does the ray start inside the box?
			// Does the ray intersect beyond the max fraction?
			if ( tmin < 0.0f || input.MaxFraction < tmin )
			{
				output = default ( RayCastOutput );
				return false;
			}

			output = new RayCastOutput () { Fraction = tmin, Normal = normal };
			return true;
		}
	}
}
