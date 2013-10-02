﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics
{
	public struct Quaternion : IComparer<Quaternion>
	{
		public float W, X, Y, Z;

		public static Quaternion Identity = new Quaternion ( 0, 0, 0, 1 );

		public Quaternion ( float x, float y, float z, float w )
		{
			X = x; Y = y; Z = z; W = w;
		}

		public Quaternion ( Vector3 vectorPart, float scalarPart )
			: this ( vectorPart.X, vectorPart.Y, vectorPart.Z, scalarPart )
		{ }

		public float Length
		{
			get
			{
				return ( float ) Math.Sqrt ( LengthSquared );
			}
		}

		public float LengthSquared
		{
			get
			{
				return ( ( ( ( this.X * this.X ) + ( this.Y * this.Y ) ) + ( this.Z * this.Z ) ) + ( this.W * this.W ) );
			}
		}


		public static Quaternion operator + ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return Add ( quaternion1, quaternion2 );
		}

		public static Quaternion Add ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X + quaternion2.X;
			quaternion.Y = quaternion1.Y + quaternion2.Y;
			quaternion.Z = quaternion1.Z + quaternion2.Z;
			quaternion.W = quaternion1.W + quaternion2.W;
			return quaternion;
		}

		public static Quaternion operator - ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return Subtract ( quaternion1, quaternion2 );
		}

		public static Quaternion operator - ( Quaternion q )
		{
			return new Quaternion ( -q.X, -q.Y, -q.Z, -q.W );
		}

		public static Quaternion Subtract ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X - quaternion2.X;
			quaternion.Y = quaternion1.Y - quaternion2.Y;
			quaternion.Z = quaternion1.Z - quaternion2.Z;
			quaternion.W = quaternion1.W - quaternion2.W;
			return quaternion;
		}

		public static Quaternion Negate ( Quaternion q )
		{
			return new Quaternion ( -q.X, -q.Y, -q.Z, -q.W );
		}

		public static Quaternion operator * ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return Multiply ( quaternion1, quaternion2 );
		}

		public static Quaternion operator * ( Quaternion quaternion1, float scaleFactor )
		{
			return Multiply ( quaternion1, scaleFactor );
		}

		public static Quaternion Multiply ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num4 = quaternion2.X;
			float num3 = quaternion2.Y;
			float num2 = quaternion2.Z;
			float num = quaternion2.W;
			float num12 = ( y * num2 ) - ( z * num3 );
			float num11 = ( z * num4 ) - ( x * num2 );
			float num10 = ( x * num3 ) - ( y * num4 );
			float num9 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			quaternion.X = ( ( x * num ) + ( num4 * w ) ) + num12;
			quaternion.Y = ( ( y * num ) + ( num3 * w ) ) + num11;
			quaternion.Z = ( ( z * num ) + ( num2 * w ) ) + num10;
			quaternion.W = ( w * num ) - num9;
			return quaternion;
		}

		public static Quaternion Multiply ( Quaternion quaternion1, float scaleFactor )
		{
			Quaternion quaternion;
			quaternion.X = quaternion1.X * scaleFactor;
			quaternion.Y = quaternion1.Y * scaleFactor;
			quaternion.Z = quaternion1.Z * scaleFactor;
			quaternion.W = quaternion1.W * scaleFactor;
			return quaternion;
		}


		public static Quaternion operator / ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return Divide ( quaternion1, quaternion2 );
		}

		public static Quaternion Divide ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			Quaternion quaternion;
			float x = quaternion1.X;
			float y = quaternion1.Y;
			float z = quaternion1.Z;
			float w = quaternion1.W;
			float num14 = ( ( ( quaternion2.X * quaternion2.X ) + ( quaternion2.Y * quaternion2.Y ) ) +
				( quaternion2.Z * quaternion2.Z ) ) + ( quaternion2.W * quaternion2.W );
			float num5 = 1f / num14;
			float num4 = -quaternion2.X * num5;
			float num3 = -quaternion2.Y * num5;
			float num2 = -quaternion2.Z * num5;
			float num = quaternion2.W * num5;
			float num13 = ( y * num2 ) - ( z * num3 );
			float num12 = ( z * num4 ) - ( x * num2 );
			float num11 = ( x * num3 ) - ( y * num4 );
			float num10 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			quaternion.X = ( ( x * num ) + ( num4 * w ) ) + num13;
			quaternion.Y = ( ( y * num ) + ( num3 * w ) ) + num12;
			quaternion.Z = ( ( z * num ) + ( num2 * w ) ) + num11;
			quaternion.W = ( w * num ) - num10;
			return quaternion;
		}

		public static float Dot ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return ( ( ( ( quaternion1.X * quaternion2.X ) + ( quaternion1.Y * quaternion2.Y ) ) +
				( quaternion1.Z * quaternion2.Z ) ) + ( quaternion1.W * quaternion2.W ) );
		}

		public void Normalize ()
		{
			this = Normalize ( this );
		}

		public static Quaternion Normalize ( Quaternion quaternion )
		{
			Quaternion quaternion2;
			float num2 = ( ( ( quaternion.X * quaternion.X ) + ( quaternion.Y * quaternion.Y ) ) +
				( quaternion.Z * quaternion.Z ) ) + ( quaternion.W * quaternion.W );
			float num = 1f / ( ( float ) Math.Sqrt ( ( double ) num2 ) );
			quaternion2.X = quaternion.X * num;
			quaternion2.Y = quaternion.Y * num;
			quaternion2.Z = quaternion.Z * num;
			quaternion2.W = quaternion.W * num;
			return quaternion2;
		}

		public static Quaternion Concatenate ( Quaternion value1, Quaternion value2 )
		{
			Quaternion quaternion;
			float x = value2.X;
			float y = value2.Y;
			float z = value2.Z;
			float w = value2.W;
			float num4 = value1.X;
			float num3 = value1.Y;
			float num2 = value1.Z;
			float num = value1.W;
			float num12 = ( y * num2 ) - ( z * num3 );
			float num11 = ( z * num4 ) - ( x * num2 );
			float num10 = ( x * num3 ) - ( y * num4 );
			float num9 = ( ( x * num4 ) + ( y * num3 ) ) + ( z * num2 );
			quaternion.X = ( ( x * num ) + ( num4 * w ) ) + num12;
			quaternion.Y = ( ( y * num ) + ( num3 * w ) ) + num11;
			quaternion.Z = ( ( z * num ) + ( num2 * w ) ) + num10;
			quaternion.W = ( w * num ) - num9;
			return quaternion;
		}

		public void Conjugate ()
		{
			this = Conjugate ( this );
		}

		public static Quaternion Conjugate ( Quaternion value )
		{
			Quaternion quaternion;
			quaternion.X = -value.X;
			quaternion.Y = -value.Y;
			quaternion.Z = -value.Z;
			quaternion.W = value.W;
			return quaternion;
		}

		public static bool operator == ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return ( ( quaternion1.X == quaternion2.X ) && ( quaternion1.Y == quaternion2.Y ) ) &&
				( ( quaternion1.Z == quaternion2.Z ) && ( quaternion1.W == quaternion2.W ) );
		}

		public static bool operator != ( Quaternion quaternion1, Quaternion quaternion2 )
		{
			return !( quaternion1 == quaternion2 );
		}

		public static Matrix4x4 ToMatrix ( Quaternion quaternion )
		{
			Matrix4x4 matrix = new Matrix4x4 ();

			float x2 = quaternion.X * quaternion.X;
			float y2 = quaternion.Y * quaternion.Y;
			float z2 = quaternion.Z * quaternion.Z;
			float xy = quaternion.X * quaternion.Y;
			float xz = quaternion.X * quaternion.Z;
			float yz = quaternion.Y * quaternion.Z;
			float wx = quaternion.W * quaternion.X;
			float wy = quaternion.W * quaternion.Y;
			float wz = quaternion.W * quaternion.Z;


			// This calculation would be a lot more complicated for non-unit length quaternions
			// Note: The constructor of Matrix4 expects the Matrix in column-major format like expected by
			//   OpenGL
			matrix.M11 = 1.0f - 2.0f * ( y2 + z2 );
			matrix.M12 = 2.0f * ( xy - wz );
			matrix.M13 = 2.0f * ( xz + wy );
			matrix.M14 = 0.0f;


			matrix.M21 = 2.0f * ( xy + wz );
			matrix.M22 = 1.0f - 2.0f * ( x2 + z2 );
			matrix.M23 = 2.0f * ( yz - wx );
			matrix.M24 = 0.0f;


			matrix.M31 = 2.0f * ( xz - wy );
			matrix.M32 = 2.0f * ( yz + wx );
			matrix.M33 = 1.0f - 2.0f * ( x2 + y2 );
			matrix.M34 = 0.0f;


			matrix.M41 = 2.0f * ( xz - wy );
			matrix.M42 = 2.0f * ( yz + wx );
			matrix.M43 = 1.0f - 2.0f * ( x2 + y2 );
			matrix.M44 = 0.0f;

			return matrix;
		}

		public override bool Equals ( object obj )
		{
			if ( !( obj is Quaternion ) ) return false;
			return Length == ( ( Quaternion ) obj ).Length;
		}

		public override int GetHashCode ()
		{
			return ( int ) ( Length );
		}

		public int Compare ( Quaternion x, Quaternion y )
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

		public float [] ToArray () { return new float [] { X, Y, Z, W }; }

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
