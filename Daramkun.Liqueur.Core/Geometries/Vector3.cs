﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Geometries
{
	public struct Vector3 : IComparer<Vector3>, ICollision<Vector3>
	{
		public static readonly Vector3 Zero = new Vector3 ( 0 );

		public float X, Y, Z;

		//public float X { get { return x; } set { x = value; } }
		//public float Y { get { return y; } set { y = value; } }
		//public float Z { get { return z; } set { z = value; } }
		
		public Vector3 ( float value )
		{
			X = Y = Z = value;
		}

		public Vector3 ( float x, float y, float z )
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public float Length { get { return ( float ) Math.Sqrt ( X * X + Y * Y + Z * Z ); } }

		public static Vector3 operator + ( Vector3 v1, Vector3 v2 )
		{
			return new Vector3 ( v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z );
		}

		public static Vector3 operator - ( Vector3 v1, Vector3 v2 )
		{
			return new Vector3 ( v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z );
		}

		public static Vector3 operator - ( Vector3 v )
		{
			return new Vector3 ( -v.X, -v.Y, -v.Z );
		}

		public static Vector3 operator * ( Vector3 v1, float v2 )
		{
			return new Vector3 ( v1.X * v2, v1.Y * v2, v1.Z * v2 );
		}

		public static Vector3 operator * ( Vector3 v1, Vector3 v2 )
		{
			return Cross ( v1, v2 );
		}

		public static Vector3 operator * ( float v1, Vector3 v2 )
		{
			return new Vector3 ( v2.X * v1, v2.Y * v1, v2.Z * v1 );
		}

		public static Vector3 operator / ( Vector3 v1, float v2 )
		{
			return new Vector3 ( v1.X / v2, v1.Y / v2, v1.Z / v2 );
		}

		public static Vector3 operator / ( Vector3 v1, Vector3 v2 )
		{
			return new Vector3 ( v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z );
		}

		public Vector3 Normalize ()
		{
			float length = Length;
			X /= length;
			Y /= length;
			Z /= length;
			return this;
		}

		public static float Dot ( Vector3 v1, Vector3 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		public static Vector3 Cross ( Vector3 v1, Vector3 v2 )
		{
			return new Vector3 (
				v1.Y * v2.Z - v1.Z * v2.Y,
				v1.Z * v2.X - v1.X * v2.Z,
				v1.X * v2.Y - v1.Y * v2.X
				);
		}

		public static Vector3 Normalize ( Vector3 value )
		{
			return value.Normalize ();
		}

		public static float Distance ( Vector3 v1, Vector3 v2 )
		{
			return ( float ) Math.Sqrt ( Math.Pow ( v2.X - v1.X, 2 ) +
				Math.Pow ( v2.Y - v1.Y, 2 ) + Math.Pow ( v2.Z - v1.Z, 2 ) );
		}

		public int Compare ( Vector3 x, Vector3 y )
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

		public override bool Equals ( object obj )
		{
			if ( !( obj is Vector3 ) ) return false;
			return Length == ( ( Vector3 ) obj ).Length;
		}

		public static bool operator == ( Vector3 v1, Vector3 v2 )
		{
			return v1.Equals ( v2 );
		}

		public static bool operator != ( Vector3 v1, Vector3 v2 )
		{
			return !v1.Equals ( v2 );
		}

		public override int GetHashCode ()
		{
			return ( int ) ( Length );
		}

		public override string ToString ()
		{
			return String.Format ( "{{X:{0}, Y:{1}, Z:{2}}}", X, Y, Z );
		}

		public bool IsCollisionTo ( Vector3 obj )
		{
			return ( obj.X == X && obj.Y == Y && obj.Z == Z );
		}
	}
}