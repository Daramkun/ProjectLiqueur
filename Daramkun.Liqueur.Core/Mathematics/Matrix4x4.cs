using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics
{
	public struct Matrix4x4 : IEquatable<Matrix4x4>
	{
		public float
			M11, M12, M13, M14,
			M21, M22, M23, M24,
			M31, M32, M33, M34,
			M41, M42, M43, M44;

		public Vector4 Column1 { get { return new Vector4 ( M11, M12, M13, M14 ); } set { M11 = value.X; M12 = value.Y; M13 = value.Z; M14 = value.W; } }
		public Vector4 Column2 { get { return new Vector4 ( M21, M22, M23, M24 ); } set { M21 = value.X; M22 = value.Y; M23 = value.Z; M24 = value.W; } }
		public Vector4 Column3 { get { return new Vector4 ( M31, M32, M33, M34 ); } set { M31 = value.X; M32 = value.Y; M33 = value.Z; M34 = value.W; } }
		public Vector4 Column4 { get { return new Vector4 ( M41, M42, M43, M44 ); } set { M41 = value.X; M42 = value.Y; M43 = value.Z; M44 = value.W; } }

		public static readonly Matrix4x4 Identity = new Matrix4x4 (
														1f, 0f, 0f, 0f,
														0f, 1f, 0f, 0f,
														0f, 0f, 1f, 0f,
														0f, 0f, 0f, 1f
														);

		public Matrix4x4 (
			float m11, float m12, float m13, float m14,
			float m21, float m22, float m23, float m24,
			float m31, float m32, float m33, float m34,
			float m41, float m42, float m43, float m44
			)
		{
			M11 = m11; M12 = m12; M13 = m13; M14 = m14;
			M21 = m21; M22 = m22; M23 = m23; M24 = m24;
			M31 = m31; M32 = m32; M33 = m33; M34 = m34;
			M41 = m41; M42 = m42; M43 = m43; M44 = m44;
		}

		public Matrix4x4 ( Vector4 column1, Vector4 column2, Vector4 column3, Vector4 column4 )
			: this (
				column1.X, column1.Y, column1.Z, column1.W,
				column2.X, column2.Y, column2.Z, column2.W,
				column3.X, column3.Y, column3.Z, column3.W,
				column4.X, column4.Y, column4.Z, column4.W
			)
		{

		}

		public Matrix4x4 ( Quaternion quaternion )
		{
			float num9 = quaternion.X * quaternion.X;
			float num8 = quaternion.Y * quaternion.Y;
			float num7 = quaternion.Z * quaternion.Z;
			float num6 = quaternion.X * quaternion.Y;
			float num5 = quaternion.Z * quaternion.W;
			float num4 = quaternion.Z * quaternion.X;
			float num3 = quaternion.Y * quaternion.W;
			float num2 = quaternion.Y * quaternion.Z;
			float num = quaternion.X * quaternion.W;
			M11 = 1f - ( 2f * ( num8 + num7 ) );
			M12 = 2f * ( num6 + num5 );
			M13 = 2f * ( num4 - num3 );
			M14 = 0f;
			M21 = 2f * ( num6 - num5 );
			M22 = 1f - ( 2f * ( num7 + num9 ) );
			M23 = 2f * ( num2 + num );
			M24 = 0f;
			M31 = 2f * ( num4 + num3 );
			M32 = 2f * ( num2 - num );
			M33 = 1f - ( 2f * ( num8 + num9 ) );
			M34 = 0f;
			M41 = 0f;
			M42 = 0f;
			M43 = 0f;
			M44 = 1f;
		}

		public void SetIdentity () { this = Identity; }

		public static Matrix4x4 operator + ( Matrix4x4 matrix1, Matrix4x4 matrix2 )
		{
			matrix1.M11 += matrix2.M11;
			matrix1.M12 += matrix2.M12;
			matrix1.M13 += matrix2.M13;
			matrix1.M14 += matrix2.M14;
			matrix1.M21 += matrix2.M21;
			matrix1.M22 += matrix2.M22;
			matrix1.M23 += matrix2.M23;
			matrix1.M24 += matrix2.M24;
			matrix1.M31 += matrix2.M31;
			matrix1.M32 += matrix2.M32;
			matrix1.M33 += matrix2.M33;
			matrix1.M34 += matrix2.M34;
			matrix1.M41 += matrix2.M41;
			matrix1.M42 += matrix2.M42;
			matrix1.M43 += matrix2.M43;
			matrix1.M44 += matrix2.M44;
			return matrix1;
		}

		public static Matrix4x4 operator - ( Matrix4x4 matrix1, Matrix4x4 matrix2 )
		{
			matrix1.M11 -= matrix2.M11;
			matrix1.M12 -= matrix2.M12;
			matrix1.M13 -= matrix2.M13;
			matrix1.M14 -= matrix2.M14;
			matrix1.M21 -= matrix2.M21;
			matrix1.M22 -= matrix2.M22;
			matrix1.M23 -= matrix2.M23;
			matrix1.M24 -= matrix2.M24;
			matrix1.M31 -= matrix2.M31;
			matrix1.M32 -= matrix2.M32;
			matrix1.M33 -= matrix2.M33;
			matrix1.M34 -= matrix2.M34;
			matrix1.M41 -= matrix2.M41;
			matrix1.M42 -= matrix2.M42;
			matrix1.M43 -= matrix2.M43;
			matrix1.M44 -= matrix2.M44;
			return matrix1;
		}

		public static Matrix4x4 operator * ( Matrix4x4 matrix1, Matrix4x4 matrix2 )
		{
			var m11 = ( ( ( matrix1.M11 * matrix2.M11 ) + ( matrix1.M12 * matrix2.M21 ) ) +
				( matrix1.M13 * matrix2.M31 ) ) + ( matrix1.M14 * matrix2.M41 );
			var m12 = ( ( ( matrix1.M11 * matrix2.M12 ) + ( matrix1.M12 * matrix2.M22 ) ) +
				( matrix1.M13 * matrix2.M32 ) ) + ( matrix1.M14 * matrix2.M42 );
			var m13 = ( ( ( matrix1.M11 * matrix2.M13 ) + ( matrix1.M12 * matrix2.M23 ) ) +
				( matrix1.M13 * matrix2.M33 ) ) + ( matrix1.M14 * matrix2.M43 );
			var m14 = ( ( ( matrix1.M11 * matrix2.M14 ) + ( matrix1.M12 * matrix2.M24 ) ) +
				( matrix1.M13 * matrix2.M34 ) ) + ( matrix1.M14 * matrix2.M44 );
			var m21 = ( ( ( matrix1.M21 * matrix2.M11 ) + ( matrix1.M22 * matrix2.M21 ) ) +
				( matrix1.M23 * matrix2.M31 ) ) + ( matrix1.M24 * matrix2.M41 );
			var m22 = ( ( ( matrix1.M21 * matrix2.M12 ) + ( matrix1.M22 * matrix2.M22 ) ) +
				( matrix1.M23 * matrix2.M32 ) ) + ( matrix1.M24 * matrix2.M42 );
			var m23 = ( ( ( matrix1.M21 * matrix2.M13 ) + ( matrix1.M22 * matrix2.M23 ) ) +
				( matrix1.M23 * matrix2.M33 ) ) + ( matrix1.M24 * matrix2.M43 );
			var m24 = ( ( ( matrix1.M21 * matrix2.M14 ) + ( matrix1.M22 * matrix2.M24 ) ) +
				( matrix1.M23 * matrix2.M34 ) ) + ( matrix1.M24 * matrix2.M44 );
			var m31 = ( ( ( matrix1.M31 * matrix2.M11 ) + ( matrix1.M32 * matrix2.M21 ) ) +
				( matrix1.M33 * matrix2.M31 ) ) + ( matrix1.M34 * matrix2.M41 );
			var m32 = ( ( ( matrix1.M31 * matrix2.M12 ) + ( matrix1.M32 * matrix2.M22 ) ) +
				( matrix1.M33 * matrix2.M32 ) ) + ( matrix1.M34 * matrix2.M42 );
			var m33 = ( ( ( matrix1.M31 * matrix2.M13 ) + ( matrix1.M32 * matrix2.M23 ) ) +
				( matrix1.M33 * matrix2.M33 ) ) + ( matrix1.M34 * matrix2.M43 );
			var m34 = ( ( ( matrix1.M31 * matrix2.M14 ) + ( matrix1.M32 * matrix2.M24 ) ) +
				( matrix1.M33 * matrix2.M34 ) ) + ( matrix1.M34 * matrix2.M44 );
			var m41 = ( ( ( matrix1.M41 * matrix2.M11 ) + ( matrix1.M42 * matrix2.M21 ) ) +
				( matrix1.M43 * matrix2.M31 ) ) + ( matrix1.M44 * matrix2.M41 );
			var m42 = ( ( ( matrix1.M41 * matrix2.M12 ) + ( matrix1.M42 * matrix2.M22 ) ) +
				( matrix1.M43 * matrix2.M32 ) ) + ( matrix1.M44 * matrix2.M42 );
			var m43 = ( ( ( matrix1.M41 * matrix2.M13 ) + ( matrix1.M42 * matrix2.M23 ) ) +
				( matrix1.M43 * matrix2.M33 ) ) + ( matrix1.M44 * matrix2.M43 );
			var m44 = ( ( ( matrix1.M41 * matrix2.M14 ) + ( matrix1.M42 * matrix2.M24 ) ) +
				( matrix1.M43 * matrix2.M34 ) ) + ( matrix1.M44 * matrix2.M44 );
			matrix1.M11 = m11; matrix1.M12 = m12; matrix1.M13 = m13; matrix1.M14 = m14;
			matrix1.M21 = m21; matrix1.M22 = m22; matrix1.M23 = m23; matrix1.M24 = m24;
			matrix1.M31 = m31; matrix1.M32 = m32; matrix1.M33 = m33; matrix1.M34 = m34;
			matrix1.M41 = m41; matrix1.M42 = m42; matrix1.M43 = m43; matrix1.M44 = m44;
			return matrix1;
		}

		public static Matrix4x4 operator * ( Matrix4x4 matrix1, float factor )
		{
			matrix1.M11 *= factor; matrix1.M12 *= factor; matrix1.M13 *= factor; matrix1.M14 *= factor;
			matrix1.M21 *= factor; matrix1.M22 *= factor; matrix1.M23 *= factor; matrix1.M24 *= factor;
			matrix1.M31 *= factor; matrix1.M32 *= factor; matrix1.M33 *= factor; matrix1.M34 *= factor;
			matrix1.M41 *= factor; matrix1.M42 *= factor; matrix1.M43 *= factor; matrix1.M44 *= factor;
			return matrix1;
		}

		public static Matrix4x4 operator / ( Matrix4x4 matrix1, Matrix4x4 matrix2 )
		{
			matrix1.M11 = matrix1.M11 / matrix2.M11; matrix1.M12 = matrix1.M12 / matrix2.M12;
			matrix1.M13 = matrix1.M13 / matrix2.M13; matrix1.M14 = matrix1.M14 / matrix2.M14;
			matrix1.M21 = matrix1.M21 / matrix2.M21; matrix1.M22 = matrix1.M22 / matrix2.M22;
			matrix1.M23 = matrix1.M23 / matrix2.M23; matrix1.M24 = matrix1.M24 / matrix2.M24;
			matrix1.M31 = matrix1.M31 / matrix2.M31; matrix1.M32 = matrix1.M32 / matrix2.M32;
			matrix1.M33 = matrix1.M33 / matrix2.M33; matrix1.M34 = matrix1.M34 / matrix2.M34;
			matrix1.M41 = matrix1.M41 / matrix2.M41; matrix1.M42 = matrix1.M42 / matrix2.M42;
			matrix1.M43 = matrix1.M43 / matrix2.M43; matrix1.M44 = matrix1.M44 / matrix2.M44;
			return matrix1;
		}

		public static Matrix4x4 operator / ( Matrix4x4 matrix, float divider )
		{
			float num = 1f / divider;
			matrix.M11 = matrix.M11 * num; matrix.M12 = matrix.M12 * num;
			matrix.M13 = matrix.M13 * num; matrix.M14 = matrix.M14 * num;
			matrix.M21 = matrix.M21 * num; matrix.M22 = matrix.M22 * num;
			matrix.M23 = matrix.M23 * num; matrix.M24 = matrix.M24 * num;
			matrix.M31 = matrix.M31 * num; matrix.M32 = matrix.M32 * num;
			matrix.M33 = matrix.M33 * num; matrix.M34 = matrix.M34 * num;
			matrix.M41 = matrix.M41 * num; matrix.M42 = matrix.M42 * num;
			matrix.M43 = matrix.M43 * num; matrix.M44 = matrix.M44 * num;
			return matrix;
		}

		public static Matrix4x4 operator ! ( Matrix4x4 matrix )
		{
			return Invert ( matrix );
		}

		public static Matrix4x4 Transpose ( Matrix4x4 matrix )
		{
			Matrix4x4 ret;

			ret.M11 = matrix.M11; ret.M12 = matrix.M21; ret.M13 = matrix.M31; ret.M14 = matrix.M41;
			ret.M21 = matrix.M12; ret.M22 = matrix.M22; ret.M23 = matrix.M32; ret.M24 = matrix.M42;
			ret.M31 = matrix.M13; ret.M32 = matrix.M23; ret.M33 = matrix.M33; ret.M34 = matrix.M43;
			ret.M41 = matrix.M14; ret.M42 = matrix.M24; ret.M43 = matrix.M34; ret.M44 = matrix.M44;

			return ret;
		}

		public Matrix4x4 Transpose ()
		{
			return Transpose ( this );
		}

		public static bool operator == ( Matrix4x4 matrix1, Matrix4x4 matrix2 )
		{
			return
				( ( ( (
				( ( matrix1.M11 == matrix2.M11 ) && ( matrix1.M22 == matrix2.M22 ) ) &&
				( ( matrix1.M33 == matrix2.M33 ) && ( matrix1.M44 == matrix2.M44 ) )
				) && (
				( ( matrix1.M12 == matrix2.M12 ) && ( matrix1.M13 == matrix2.M13 ) ) &&
				( ( matrix1.M14 == matrix2.M14 ) && ( matrix1.M21 == matrix2.M21 ) )
				) ) && ( (
				( ( matrix1.M23 == matrix2.M23 ) && ( matrix1.M24 == matrix2.M24 ) ) &&
				( ( matrix1.M31 == matrix2.M31 ) && ( matrix1.M32 == matrix2.M32 ) )
				) && (
				( ( matrix1.M34 == matrix2.M34 ) && ( matrix1.M41 == matrix2.M41 ) ) &&
				( matrix1.M42 == matrix2.M42 ) ) ) ) && ( matrix1.M43 == matrix2.M43 ) );
		}

		public static bool operator != ( Matrix4x4 matrix1, Matrix4x4 matrix2 )
		{
			return !( matrix1 == matrix2 );
		}

		public bool Equals ( Matrix4x4 other )
		{
			return this == other;
		}

		public override bool Equals ( object obj )
		{
			if ( !( obj is Matrix4x4 ) ) return false;
			return Equals ( ( Matrix4x4 ) obj );
		}

		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format (
				"{{11:{0}, 12:{1}, 13:{2}, 14:{3}} {21:{4}, 22:{5}, 23:{6}, 24:{7}} " +
				"{31:{8}, 32:{9}, 33:{10}, 34:{11}} {41:{12}, 42:{13}, 43:{14}, 44:{15}}}",
				M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44
				);
		}

		public float [] ToArray ()
		{
			return new float []
			{
				M11, M12, M13, M14,
				M21, M22, M23, M24,
				M31, M32, M33, M34,
				M41, M42, M43, M44
			};
		}

		public float this [ int index ]
		{
			get
			{
				switch ( index )
				{
					case 0: return M11;
					case 1: return M12;
					case 2: return M13;
					case 3: return M14;
					case 4: return M21;
					case 5: return M22;
					case 6: return M23;
					case 7: return M24;
					case 8: return M31;
					case 9: return M32;
					case 10: return M33;
					case 11: return M34;
					case 12: return M41;
					case 13: return M42;
					case 14: return M43;
					case 15: return M44;
					default: throw new IndexOutOfRangeException ();
				}
			}
			set
			{
				switch ( index )
				{
					case 0: M11 = value; break;
					case 1: M12 = value; break;
					case 2: M13 = value; break;
					case 3: M14 = value; break;
					case 4: M21 = value; break;
					case 5: M22 = value; break;
					case 6: M23 = value; break;
					case 7: M24 = value; break;
					case 8: M31 = value; break;
					case 9: M32 = value; break;
					case 10: M33 = value; break;
					case 11: M34 = value; break;
					case 12: M41 = value; break;
					case 13: M42 = value; break;
					case 14: M43 = value; break;
					case 15: M44 = value; break;
					default: throw new IndexOutOfRangeException ();
				}
			}
		}

		public float this [ int x, int y ]
		{
			get { return this [ x + ( y * 4 ) ]; }
			set { this [ x + ( y * 4 ) ] = value; }
		}

		public static float Determinant ( Matrix4x4 matrix )
		{
			float num22 = matrix.M11, num21 = matrix.M12, num20 = matrix.M13, num19 = matrix.M14;
			float num12 = matrix.M21, num11 = matrix.M22, num10 = matrix.M23, num9 = matrix.M24;
			float num8 = matrix.M31, num7 = matrix.M32, num6 = matrix.M33, num5 = matrix.M34;
			float num4 = matrix.M41, num3 = matrix.M42, num2 = matrix.M43, num = matrix.M44;
			float num18 = ( num6 * num ) - ( num5 * num2 ), num17 = ( num7 * num ) - ( num5 * num3 );
			float num16 = ( num7 * num2 ) - ( num6 * num3 ), num15 = ( num8 * num ) - ( num5 * num4 );
			float num14 = ( num8 * num2 ) - ( num6 * num4 ), num13 = ( num8 * num3 ) - ( num7 * num4 );
			return (
				( ( ( num22 * ( ( ( num11 * num18 ) - ( num10 * num17 ) ) + ( num9 * num16 ) ) ) -
				( num21 * ( ( ( num12 * num18 ) - ( num10 * num15 ) ) + ( num9 * num14 ) ) ) ) +
				( num20 * ( ( ( num12 * num17 ) - ( num11 * num15 ) ) + ( num9 * num13 ) ) ) ) -
				( num19 * ( ( ( num12 * num16 ) - ( num11 * num14 ) ) + ( num10 * num13 ) ) )
			);
		}

		public float Determinant () { return Determinant ( this ); }

		public static Matrix4x4 Invert ( Matrix4x4 matrix )
		{
			float det1 = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
			float det2 = matrix.M11 * matrix.M23 - matrix.M13 * matrix.M21;
			float det3 = matrix.M11 * matrix.M24 - matrix.M14 * matrix.M21;
			float det4 = matrix.M12 * matrix.M23 - matrix.M13 * matrix.M22;
			float det5 = matrix.M12 * matrix.M24 - matrix.M14 * matrix.M22;
			float det6 = matrix.M13 * matrix.M24 - matrix.M14 * matrix.M23;
			float det7 = matrix.M31 * matrix.M42 - matrix.M32 * matrix.M41;
			float det8 = matrix.M31 * matrix.M43 - matrix.M33 * matrix.M41;
			float det9 = matrix.M31 * matrix.M44 - matrix.M34 * matrix.M41;
			float det10 = matrix.M32 * matrix.M43 - matrix.M33 * matrix.M42;
			float det11 = matrix.M32 * matrix.M44 - matrix.M34 * matrix.M42;
			float det12 = matrix.M33 * matrix.M44 - matrix.M34 * matrix.M43;

			float detMatrix = ( float ) ( det1 * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7 );
			float invDetMatrix = 1f / detMatrix;

			Matrix4x4 ret;
			ret.M11 = ( matrix.M22 * det12 - matrix.M23 * det11 + matrix.M24 * det10 ) * invDetMatrix;
			ret.M12 = ( -matrix.M12 * det12 + matrix.M13 * det11 - matrix.M14 * det10 ) * invDetMatrix;
			ret.M13 = ( matrix.M42 * det6 - matrix.M43 * det5 + matrix.M44 * det4 ) * invDetMatrix;
			ret.M14 = ( -matrix.M32 * det6 + matrix.M33 * det5 - matrix.M34 * det4 ) * invDetMatrix;
			ret.M21 = ( -matrix.M21 * det12 + matrix.M23 * det9 - matrix.M24 * det8 ) * invDetMatrix;
			ret.M22 = ( matrix.M11 * det12 - matrix.M13 * det9 + matrix.M14 * det8 ) * invDetMatrix;
			ret.M23 = ( -matrix.M41 * det6 + matrix.M43 * det3 - matrix.M44 * det2 ) * invDetMatrix;
			ret.M24 = ( matrix.M31 * det6 - matrix.M33 * det3 + matrix.M34 * det2 ) * invDetMatrix;
			ret.M31 = ( matrix.M21 * det11 - matrix.M22 * det9 + matrix.M24 * det7 ) * invDetMatrix;
			ret.M32 = ( -matrix.M11 * det11 + matrix.M12 * det9 - matrix.M14 * det7 ) * invDetMatrix;
			ret.M33 = ( matrix.M41 * det5 - matrix.M42 * det3 + matrix.M44 * det1 ) * invDetMatrix;
			ret.M34 = ( -matrix.M31 * det5 + matrix.M32 * det3 - matrix.M34 * det1 ) * invDetMatrix;
			ret.M41 = ( -matrix.M21 * det10 + matrix.M22 * det8 - matrix.M23 * det7 ) * invDetMatrix;
			ret.M42 = ( matrix.M11 * det10 - matrix.M12 * det8 + matrix.M13 * det7 ) * invDetMatrix;
			ret.M43 = ( -matrix.M41 * det4 + matrix.M42 * det2 - matrix.M43 * det1 ) * invDetMatrix;
			ret.M44 = ( matrix.M31 * det4 - matrix.M32 * det2 + matrix.M33 * det1 ) * invDetMatrix;

			return ret;
		}

		public Matrix4x4 Invert ()
		{
			return Invert ( this );
		}
	}
}
