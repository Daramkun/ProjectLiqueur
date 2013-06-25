using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public struct Vector4 : IComparer<Vector4>
	{
		public static readonly Vector4 Zero = new Vector4 ( 0 );

		public float W, X, Y, Z;

		public Vector4 ( float x, float y, float z, float w )
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4 ( Vector3 value, float w )
		{
			X = value.X;
			Y = value.Y;
			Z = value.Z;
			W = w;
		}

		public Vector4 ( float value )
		{
			X = value;
			Y = value;
			Z = value;
			W = value;
		}

		public float Length { get { return ( float ) System.Math.Sqrt ( X * X + Y * Y + Z * Z + W * W ); } }

		public static float Distance ( Vector4 v1, Vector4 v2 )
		{
			return ( float ) System.Math.Sqrt ( System.Math.Pow ( v2.X - v1.X, 2 ) +
				System.Math.Pow ( v2.Y - v1.Y, 2 ) + System.Math.Pow ( v2.Z - v1.Z, 2 ) +
				System.Math.Pow ( v2.W - v1.W, 2 ) );
		}
		public static Vector4 operator + ( Vector4 value1, Vector4 value2 )
		{
			value1.W += value2.W;
			value1.X += value2.X;
			value1.Y += value2.Y;
			value1.Z += value2.Z;
			return value1;
		}

		public static Vector4 operator - ( Vector4 value1, Vector4 value2 )
		{
			value1.W -= value2.W;
			value1.X -= value2.X;
			value1.Y -= value2.Y;
			value1.Z -= value2.Z;
			return value1;
		}

		public static Vector4 operator * ( Vector4 value1, Vector4 value2 )
		{
			value1.W *= value2.W;
			value1.X *= value2.X;
			value1.Y *= value2.Y;
			value1.Z *= value2.Z;
			return value1;
		}

		public static Vector4 operator * ( Vector4 value1, float scaleFactor )
		{
			value1.W *= scaleFactor;
			value1.X *= scaleFactor;
			value1.Y *= scaleFactor;
			value1.Z *= scaleFactor;
			return value1;
		}

		public static Vector4 operator * ( float scaleFactor, Vector4 value1 )
		{
			value1.W *= scaleFactor;
			value1.X *= scaleFactor;
			value1.Y *= scaleFactor;
			value1.Z *= scaleFactor;
			return value1;
		}

		public static Vector4 operator / ( Vector4 value1, Vector4 value2 )
		{
			value1.W /= value2.W;
			value1.X /= value2.X;
			value1.Y /= value2.Y;
			value1.Z /= value2.Z;
			return value1;
		}

		public static Vector4 operator / ( Vector4 value1, float divider )
		{
			float factor = 1f / divider;
			value1.W *= factor;
			value1.X *= factor;
			value1.Y *= factor;
			value1.Z *= factor;
			return value1;
		}

		public override bool Equals ( object obj )
		{
			if ( !( obj is Vector4 ) ) return false;
			return Length == ( ( Vector4 ) obj ).Length;
		}

		public override int GetHashCode ()
		{
			return ( int ) ( Length );
		}

		public static bool operator == ( Vector4 v1, Vector4 v2 )
		{
			return ( v1.X == v2.X && v1.Y == v2.Y ) && ( v1.Z == v2.Z && v1.W == v2.W );
		}

		public static bool operator != ( Vector4 v1, Vector4 v2 )
		{
			return !( v1 == v2 );
		}

		public int Compare ( Vector4 x, Vector4 y )
		{
			float xl = x.Length;
			float yl = y.Length;
			if ( xl < yl )
				return 1;
			else if ( xl > yl )
				return -1;
			else
				return 0;
		}
	}
}
