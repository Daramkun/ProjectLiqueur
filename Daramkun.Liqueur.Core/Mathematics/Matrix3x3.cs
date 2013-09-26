using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics
{
	public struct Matrix3x3 : IEquatable<Matrix3x3>
	{
		public float M11, M12, M13, M21, M22, M23, M31, M32, M33;

		public static readonly Matrix3x3 Identity = new Matrix3x3 (
																	1, 0, 0,
																	0, 1, 0,
																	0, 0, 1
																	);

		public Matrix3x3 ( float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33 )
		{
			M11 = m11; M12 = m12; M13 = m13;
			M21 = m21; M22 = m22; M23 = m23;
			M31 = m31; M32 = m32; M33 = m33;
		}

		public static Matrix3x3 operator + ( Matrix3x3 v1, Matrix3x3 v2 )
		{
			v1.M11 += v2.M11; v1.M12 += v2.M12; v1.M13 += v2.M13;
			v1.M21 += v2.M21; v1.M22 += v2.M22; v1.M23 += v2.M23;
			v1.M31 += v2.M31; v1.M32 += v2.M32; v1.M33 += v2.M33;
			return v1;
		}

		public static Matrix3x3 operator - ( Matrix3x3 v1, Matrix3x3 v2 )
		{
			v1.M11 -= v2.M11; v1.M12 -= v2.M12; v1.M13 -= v2.M13;
			v1.M21 -= v2.M21; v1.M22 -= v2.M22; v1.M23 -= v2.M23;
			v1.M31 -= v2.M31; v1.M32 -= v2.M32; v1.M33 -= v2.M33;
			return v1;
		}

		public static Matrix3x3 operator * ( Matrix3x3 v1, Matrix3x3 v2 )
		{
			v1.M11 = ( v1.M11 * v2.M11 ) + ( v1.M12 * v2.M21 ) + ( v1.M13 * v2.M31 );
			v1.M12 = ( v1.M11 * v2.M12 ) + ( v1.M12 * v2.M22 ) + ( v1.M13 * v2.M32 );
			v1.M13 = ( v1.M11 * v2.M13 ) + ( v1.M12 * v2.M23 ) + ( v1.M13 * v2.M33 );
			v1.M21 = ( v1.M21 * v2.M11 ) + ( v1.M22 * v2.M21 ) + ( v1.M23 * v2.M31 );
			v1.M22 = ( v1.M21 * v2.M12 ) + ( v1.M22 * v2.M22 ) + ( v1.M23 * v2.M32 );
			v1.M23 = ( v1.M21 * v2.M13 ) + ( v1.M22 * v2.M23 ) + ( v1.M23 * v2.M33 );
			v1.M31 = ( v1.M31 * v2.M11 ) + ( v1.M32 * v2.M21 ) + ( v1.M33 * v2.M31 );
			v1.M32 = ( v1.M31 * v2.M12 ) + ( v1.M32 * v2.M22 ) + ( v1.M33 * v2.M32 );
			v1.M33 = ( v1.M31 * v2.M13 ) + ( v1.M32 * v2.M23 ) + ( v1.M33 * v2.M33 );
			return v1;
		}

		public static Matrix3x3 operator * ( Matrix3x3 v1, float v2 )
		{
			v1.M11 *= v2; v1.M12 *= v2; v1.M13 *= v2;
			v1.M21 *= v2; v1.M22 *= v2; v1.M23 *= v2;
			v1.M31 *= v2; v1.M32 *= v2; v1.M33 *= v2;
			return v1;
		}

		public static Matrix3x3 operator / ( Matrix3x3 v1, Matrix3x3 v2 )
		{
			v1.M11 /= v2.M11; v1.M12 /= v2.M12; v1.M13 /= v2.M13;
			v1.M21 /= v2.M21; v1.M22 /= v2.M22; v1.M23 /= v2.M23;
			v1.M31 /= v2.M31; v1.M32 /= v2.M32; v1.M33 /= v2.M33;
			return v1;
		}

		public static Matrix3x3 operator / ( Matrix3x3 v1, float v2 )
		{
			v1.M11 /= v2; v1.M12 /= v2; v1.M13 /= v2;
			v1.M21 /= v2; v1.M22 /= v2; v1.M23 /= v2;
			v1.M31 /= v2; v1.M32 /= v2; v1.M33 /= v2;
			return v1;
		}

		public static Matrix3x3 operator ! ( Matrix3x3 v1 )
		{
			return Invert ( v1 );
		}

		public static Matrix3x3 Transpose ( Matrix3x3 v1 )
		{
			Matrix3x3 mat = new Matrix3x3 ();
			mat.M11 = v1.M11; mat.M12 = v1.M21; mat.M13 = v1.M31;
			mat.M21 = v1.M12; mat.M22 = v1.M22; mat.M23 = v1.M32;
			mat.M31 = v1.M13; mat.M32 = v1.M23; mat.M33 = v1.M33;
			return mat;
		}

		public Matrix3x3 Transpose ()
		{
			return Transpose ( this );
		}

		public static Matrix3x3 Invert2x2 ( Matrix3x3 v1 )
		{
			float a = v1.M11, b = v1.M21, c = v1.M12, d = v1.M22;
			float det = a * d - b * c;
			if ( det != 0.0f ) det = 1.0f / det;
			Matrix3x3 matrix = new Matrix3x3 ();
			matrix.M11 = det * d; matrix.M21 = -det * b; matrix.M13 = 0;
			matrix.M12 = -det * c; matrix.M22 = det * a; matrix.M23 = 0;
			matrix.M31 = 0; matrix.M32 = 0; matrix.M33 = 0;
			return matrix;
		}

		public Matrix3x3 Invert2x2 () { return Matrix3x3.Invert2x2 ( this ); }

		public static Matrix3x3 Invert ( Matrix3x3 v1 )
		{
			float det = Vector3.Dot ( new Vector3 ( v1.M11, v1.M12, v1.M13 ), new Vector3 ( v1.M31, v1.M32, v1.M33 ) );
			if ( det != 0 ) det = 1.0f / det;

			float a11 = v1.M11, a12 = v1.M21, a13 = v1.M31;
			float a22 = v1.M22, a23 = v1.M32;
			float a33 = v1.M33;

			Matrix3x3 matrix = new Matrix3x3 ();
			matrix.M11 = det * ( a22 * a33 - a23 * a23 );
			matrix.M12 = det * ( a13 * a23 - a12 * a33 );
			matrix.M13 = det * ( a12 * a23 - a13 * a22 );
			matrix.M21 = v1.M12;
			matrix.M22 = det * ( a11 * a33 - a13 * a13 );
			matrix.M23 = det * ( a13 * a12 - a11 * a23 );
			matrix.M31 = v1.M13;
			matrix.M32 = v1.M23;
			matrix.M33 = det * ( a11 * a22 - a12 * a12 );
			
			return matrix;
		}

		public Matrix3x3 Invert () { return Matrix3x3.Invert ( this ); }

		public static Matrix3x3 SymmetricInvert ( Matrix3x3 v1 )
		{
			float det = Vector3.Dot ( new Vector3 ( v1.M11, v1.M12, v1.M13 ), 
				Vector3.Cross ( new Vector3 ( v1.M21, v1.M22, v1.M23 ), new Vector3 ( v1.M31, v1.M32, v1.M33 ) ) );
			if ( det != 0 ) det = 1.0f / det;

			float a11 = v1.M11, a12 = v1.M21, a13 = v1.M31;
			float a22 = v1.M22, a23 = v1.M32;
			float a33 = v1.M33;

			Matrix3x3 matrix = new Matrix3x3 ();
			matrix.M11 = det * ( a22 * a33 - a23 * a23 );
			matrix.M12 = det * ( a13 * a23 - a12 * a33 );
			matrix.M13 = det * ( a12 * a23 - a13 * a22 );

			matrix.M21 = matrix.M12;
			matrix.M22 = det * ( a11 * a33 - a13 * a13 );
			matrix.M23 = det * ( a13 * a12 - a11 * a23 );

			matrix.M31 = matrix.M13;
			matrix.M32 = matrix.M23;
			matrix.M33 = det * ( a11 * a22 - a12 * a12 );
			return matrix;
		}

		public Matrix3x3 SymmetricInvert () { return Matrix3x3.SymmetricInvert ( this ); }

		public static Vector3 Solve ( Matrix3x3 matrix, Vector3 vector )
		{
			float det = Vector3.Dot ( new Vector3 ( matrix.M11, matrix.M12, matrix.M13 ),
				Vector3.Cross ( new Vector3 ( matrix.M21, matrix.M22, matrix.M23 ), new Vector3 ( matrix.M31, matrix.M32, matrix.M33 ) ) );
			if ( det != 0 ) det = 1.0f / det;
			return new Vector3 (
				det * Vector3.Dot ( vector, Vector3.Cross ( new Vector3 ( matrix.M21, matrix.M22, matrix.M23 ), new Vector3 ( matrix.M31, matrix.M32, matrix.M33 ) ) ),
				det * Vector3.Dot ( new Vector3 ( matrix.M11, matrix.M12, matrix.M13 ), Vector3.Cross ( vector, new Vector3 ( matrix.M31, matrix.M32, matrix.M33 ) ) ),
				det * Vector3.Dot ( new Vector3 ( matrix.M11, matrix.M12, matrix.M13 ), Vector3.Cross ( new Vector3 ( matrix.M21, matrix.M22, matrix.M23 ), vector ) )
				);
		}

		public static Vector2 Solve ( Matrix3x3 matrix, Vector2 vector )
		{
			float a11 = matrix.M11, a12 = matrix.M21, a21 = matrix.M12, a22 = matrix.M22;
			float det = a11 * a22 - a12 * a21;
			if ( det != 0.0f ) det = 1.0f / det;
			return new Vector2 ( det * ( a22 * vector.X - a12 * vector.Y ), det * ( a11 * vector.Y - a21 * vector.X ) );
		}

		public Vector3 Solve ( Vector3 value ) { return Matrix3x3.Solve ( this, value ); }
		public Vector2 Solve ( Vector2 value ) { return Matrix3x3.Solve ( this, value ); }

		public bool Equals ( Matrix3x3 other )
		{
			return ( ( M11 == other.M11 && M12 == other.M12 ) && ( M13 == other.M13 ) ) &&
				( ( M21 == other.M21 && M22 == other.M22 ) && ( M23 == other.M23 ) ) &&
				( ( M31 == other.M31 && M32 == other.M32 ) && ( M33 == other.M33 ) );
		}

		public static bool operator == ( Matrix3x3 v1, Matrix3x3 v2 )
		{
			return v1.Equals ( v2 );
		}

		public static bool operator != ( Matrix3x3 v1, Matrix3x3 v2 )
		{
			return !v1.Equals ( v2 );
		}

		public override bool Equals ( object obj )
		{
			if ( !( obj is Matrix3x3 ) ) return false;
			return Equals ( ( Matrix3x3 ) obj );
		}

		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format ( "{{11:{0}, 12:{1}, 13:{2}} {21:{3}, 22:{4}, 23:{5}} {31:{6}, 32:{7}, 33:{8}}}",
				M11, M12, M13, M21, M22, M23, M31, M32, M33 );
		}

		public float [] ToArray ()
		{
			return new float []
			{
				M11, M12, M13,
				M21, M22, M23,
				M31, M32, M33,
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
					case 3: return M21;
					case 4: return M22;
					case 5: return M23;
					case 6: return M31;
					case 7: return M32;
					case 8: return M33;
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
					case 3: M21 = value; break;
					case 4: M22 = value; break;
					case 5: M23 = value; break;
					case 6: M31 = value; break;
					case 7: M32 = value; break;
					case 8: M33 = value; break;
					default: throw new IndexOutOfRangeException ();
				}
			}
		}

		public float this [ int x, int y ]
		{
			get { return this [ x + ( y * 3 ) ]; }
			set { this [ x + ( y * 3 ) ] = value; }
		}
	}
}
