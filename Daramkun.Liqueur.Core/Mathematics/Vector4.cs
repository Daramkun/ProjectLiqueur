using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics
{
	public struct Vector4 : IComparer<Vector4>, IVector
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

		public float LengthSquared { get { return X * X + Y * Y + Z * Z + W * W; } }
		public float Length { get { return ( float ) System.Math.Sqrt ( LengthSquared ); } }

		public static float Distance ( Vector4 v1, Vector4 v2 )
		{
			return ( float ) System.Math.Sqrt ( System.Math.Pow ( v2.X - v1.X, 2 ) +
				System.Math.Pow ( v2.Y - v1.Y, 2 ) + System.Math.Pow ( v2.Z - v1.Z, 2 ) +
				System.Math.Pow ( v2.W - v1.W, 2 ) );
		}
		public static Vector4 operator + ( Vector4 value1, Vector4 value2 )
		{
			return Add ( value1, value2 );
		}

		public static Vector4 Add ( Vector4 value1, Vector4 value2 )
		{
			value1.W += value2.W;
			value1.X += value2.X;
			value1.Y += value2.Y;
			value1.Z += value2.Z;
			return value1;
		}

		public static Vector4 operator - ( Vector4 value1, Vector4 value2 )
		{
			return Subtract ( value1, value2 );
		}

		public static Vector4 operator - ( Vector4 value )
		{
			return Negate ( value );
		}

		public static Vector4 Subtract ( Vector4 value1, Vector4 value2 )
		{
			value1.W -= value2.W;
			value1.X -= value2.X;
			value1.Y -= value2.Y;
			value1.Z -= value2.Z;
			return value1;
		}

		public static Vector4 Negate ( Vector4 value )
		{
			return new Vector4 ( -value.X, -value.Y, -value.Z, -value.W );
		}

		public static Vector4 operator * ( Vector4 value1, Vector4 value2 )
		{
			return Multiply ( value1, value2 );
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

		public static Vector4 Multiply ( Vector4 value1, Vector4 value2 )
		{
			value1.W *= value2.W;
			value1.X *= value2.X;
			value1.Y *= value2.Y;
			value1.Z *= value2.Z;
			return value1;
		}

		public static Vector4 Multiply ( Vector4 value1, float scaleFactor )
		{
			value1.W *= scaleFactor;
			value1.X *= scaleFactor;
			value1.Y *= scaleFactor;
			value1.Z *= scaleFactor;
			return value1;
		}

		public static Vector4 Multiply ( float scaleFactor, Vector4 value1 )
		{
			value1.W *= scaleFactor;
			value1.X *= scaleFactor;
			value1.Y *= scaleFactor;
			value1.Z *= scaleFactor;
			return value1;
		}

		public static Vector4 operator / ( Vector4 value1, Vector4 value2 )
		{
			return Divide ( value1, value2 );
		}

		public static Vector4 operator / ( Vector4 value1, float divider )
		{
			return Divide ( value1, divider );
		}

		public static Vector4 Divide ( Vector4 value1, Vector4 value2 )
		{
			value1.W /= value2.W;
			value1.X /= value2.X;
			value1.Y /= value2.Y;
			value1.Z /= value2.Z;
			return value1;
		}

		public static Vector4 Divide ( Vector4 value1, float divider )
		{
			float factor = 1f / divider;
			value1.W *= factor;
			value1.X *= factor;
			value1.Y *= factor;
			value1.Z *= factor;
			return value1;
		}

		public void Normalize ()
		{
			this = Normalize ( this );
		}

		public static Vector4 Normalize ( Vector4 v )
		{
			float len = v.Length;
			return v / len;
		}

		public static float Dot ( Vector4 value1, Vector4 value2 )
		{
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z + value1.W * value2.W;
		}

		public static Vector4 Cross ( Vector4 value1, Vector4 value2, Vector4 value3 )
		{
			return new Vector4 (
				value1.W * value3.Y - value2.W * value3.Z + value1.W * value3.W,
				-value1.W * value3.X + value1.X * value1.Y * value3.Z - value2.W * value3.W,
				value2.W * value3.X - value1.X * value1.Y * value3.Y + value1.W * value3.W,
				-value1.W * value3.X + value2.W * value3.Y - value1.W - value3.Z
			);
		}

		public static float Distance ( Vector4 v1, Vector4 v2 )
		{
			return ( float ) Math.Sqrt ( DistanceSquared ( v1, v2 ) );
		}

		public static float DistanceSquared ( Vector4 v1, Vector4 v2 )
		{
			return ( float ) ( System.Math.Pow ( v2.X - v1.X, 2 ) +
				System.Math.Pow ( v2.Y - v1.Y, 2 ) + System.Math.Pow ( v2.Z - v1.Z, 2 ) +
				System.Math.Pow ( v2.W - v1.W, 2 ) );
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

		public override string ToString ()
		{
			return string.Format ( "{{X={0}, Y={1}, Z={2}, W={3}}}", X, Y, Z, W );
		}

		public float [] ToArray () { return new float[] { X, Y, Z, W }; }

		public float this [ int index ]
		{
			get { return ( index == 0 ) ? X : ( ( index == 1 ) ? Y : ( ( index == 2 ) ? Z : ( ( index == 3 ) ? W : float.NaN ) ) ); }
			set
			{
				switch ( index )
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					case 2: Z = value; break;
					case 3: W = value; break;
					default: throw new IndexOutOfRangeException ();
				}
			}
		}
	}
}
