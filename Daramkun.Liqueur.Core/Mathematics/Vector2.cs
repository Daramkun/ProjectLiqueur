﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;

namespace Daramkun.Liqueur.Mathematics
{
	public struct Vector2 : IComparer<Vector2>, ICollision<Vector2>, ICollision<Circle>, ICollision<Rectangle>, IVector
	{
		public static readonly Vector2 Zero = new Vector2 ( 0 );

		public float X, Y;

		public Vector2 ( float value )
		{
			X = Y = value;
		}

		public Vector2 ( float x, float y )
		{
			this.X = x;
			this.Y = y;
		}

		public void SetZero () { this = new Vector2 (); }

		public float LengthSquared { get { return X * X + Y * Y; } }
		public float Length { get { return ( float ) System.Math.Sqrt ( LengthSquared ); } }

		public static Vector2 operator + ( Vector2 v1, Vector2 v2 )
		{
			return Add ( v1, v2 );
		}

		public static Vector2 Add ( Vector2 v1, Vector2 v2 )
		{
			return new Vector2 ( v1.X + v2.X, v1.Y + v2.Y );
		}

		public static Vector2 operator - ( Vector2 v1, Vector2 v2 )
		{
			return Subtract ( v1, v2 );
		}

		public static Vector2 operator - ( Vector2 v )
		{
			return Negate ( v );
		}

		public static Vector2 Subtract ( Vector2 v1, Vector2 v2 )
		{
			return new Vector2 ( v1.X - v2.X, v1.Y - v2.Y );
		}

		public static Vector2 Negate ( Vector2 v )
		{
			return new Vector2 ( -v.X, -v.Y );
		}

		public static Vector2 operator * ( Vector2 v1, float v2 )
		{
			return Multiply ( v1, v2 );
		}

		public static Vector2 operator * ( float v1, Vector2 v2 )
		{
			return Multiply ( v1, v2 );
		}

		public static Vector2 operator * ( Vector2 v1, Vector2 v2 )
		{
			return Multiply ( v1, v2 );
		}

		public static Vector2 operator / ( Vector2 v1, float v2 )
		{
			return Divide ( v1, v2 );
		}

		public static Vector2 operator / ( Vector2 v1, Vector2 v2 )
		{
			return Divide ( v1, v2 );
		}

		public void Normalize ()
		{
			this = Normalize ( this );
		}

		public static Vector2 Normalize ( Vector2 value )
		{
			return value / value.Length;
		}

		public static float Dot ( Vector2 v1, Vector2 v2 )
		{
			return v1.X * v2.X + v1.Y * v2.Y;
		}

		public static float Cross ( Vector2 v1, Vector2 v2 )
		{
			return v1.X * v2.Y - v1.Y * v2.X;
		}

		public static Vector2 Cross ( Vector2 v1, float v2 )
		{
			return new Vector2 ( v1.Y * v2, v1.X * -v2 );
		}

		public static Vector2 Cross ( float v1, Vector2 v2 )
		{
			return new Vector2 ( -v1 * v2.Y, v1 * v2.X );
		}

		public static Vector2 Multiply ( Vector2 v1, Vector2 v2 )
		{
			return new Vector2 ( v1.X * v2.Y, v1.Y * v2.X );
		}

		public static Vector2 Multiply ( Vector2 v1, float v2 )
		{
			return new Vector2 ( v1.X * v2, v1.Y * v2 );
		}

		public static Vector2 Multiply ( float v1, Vector2 v2 )
		{
			return Multiply ( v2, v1 );
		}

		public static Vector2 Divide ( Vector2 v1, Vector2 v2 )
		{
			return new Vector2 ( v1.X / v2.X, v1.Y / v2.Y );
		}

		public static Vector2 Divide ( Vector2 v1, float v2 )
		{
			return new Vector2 ( v1.X / v2, v1.Y / v2 );
		}

		public static Vector2 TransposeMultiply ( Matrix2x2 v1, Vector2 v2 )
		{
			return new Vector2 (
				Vector2.Dot ( v2, new Vector2 ( v1.M11, v1.M12 ) ),
				Vector2.Dot ( v2, new Vector2 ( v1.M21, v1.M22 ) )
			);
		}

		public static Vector2 Skew ( Vector2 value )
		{
			return new Vector2 ( -value.Y, value.X );
		}

		public Vector2 Skew ()
		{
			return this = Vector2.Skew ( this );
		}

		public static Vector2 Transform ( Vector2 position, Matrix4x4 matrix )
		{
			return new Vector2 (
				( position.X * matrix.M11 ) + ( position.Y * matrix.M21 ) + matrix.M41,
				( position.X * matrix.M12 ) + ( position.Y * matrix.M22 ) + matrix.M42
			);
		}

		public static float Distance ( Vector2 v1, Vector2 v2 )
		{
			return ( float ) System.Math.Sqrt ( DistanceSquared ( v1, v2 ) );
		}

		public static float DistanceSquared ( Vector2 v1, Vector2 v2 )
		{
			return ( float ) ( System.Math.Pow ( v2.X - v1.X, 2 ) + System.Math.Pow ( v2.Y - v1.Y, 2 ) );
		}

		public int Compare ( Vector2 x, Vector2 y )
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
			if ( !( obj is Vector2 ) ) return false;
			return Length == ( ( Vector2 ) obj ).Length;
		}

		public static bool operator == ( Vector2 v1, Vector2 v2 )
		{
			return v1.X == v2.X && v1.Y == v2.Y;
		}

		public static bool operator != ( Vector2 v1, Vector2 v2 )
		{
			return !( v1 == v2 );
		}

		public override int GetHashCode ()
		{
			return ( int ) ( Length );
		}

		public override string ToString ()
		{
			return String.Format ( "{{X:{0}, Y:{1}}}", X, Y );
		}

		public float [] ToArray () { return new float[] { X, Y }; }

		public float this [ int index ]
		{
			get { return ( index == 0 ) ? X : ( ( index == 1 ) ? Y : float.NaN ); }
			set
			{
				switch ( index )
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					default: throw new IndexOutOfRangeException ();
				}
			}
		}

		public bool IsCollisionTo ( Vector2 obj )
		{
			return ( obj.X == X && obj.Y == Y );
		}

		public bool IsCollisionTo ( Circle obj )
		{
			return obj.IsCollisionTo ( this );
		}

		public bool IsCollisionTo ( Rectangle obj )
		{
			return obj.IsCollisionTo ( this );
		}

		public bool IsValid
		{
			get
			{
				if ( float.IsNaN ( X ) || float.IsNaN ( Y ) ) return false;
				if ( float.IsInfinity ( X ) || float.IsInfinity ( Y ) ) return false;
				return true;
			}
		}

		public static Vector2 Max ( Vector2 v1, Vector2 v2 )
		{
			return new Vector2 ( ( float ) Math.Max ( v1.X, v2.X ), ( float ) Math.Max ( v1.Y, v2.Y ) );
		}

		public static Vector2 Min ( Vector2 v1, Vector2 v2 )
		{
			return new Vector2 ( ( float ) Math.Min ( v1.X, v2.X ), ( float ) Math.Min ( v1.Y, v2.Y ) );
		}

		public static Vector2 Clamp ( Vector2 v1, Vector2 v2, Vector2 v3 )
		{
			return Max ( v2, Min ( v1, v3 ) );
		}

		public static Vector2 Absolute ( Vector2 v )
		{
			return new Vector2 ( ( float ) Math.Abs ( v.X ), ( float ) Math.Abs ( v.Y ) );
		}
	}
}
