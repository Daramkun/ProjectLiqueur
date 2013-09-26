using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics
{
	public struct Matrix2x2 : IEquatable<Matrix2x2>
	{
		public float M11, M12, M21, M22;

		public static readonly Matrix2x2 Identity = new Matrix2x2 (
																	1, 0,
																	0, 1
																	);

		public Matrix2x2 ( float m11, float m12, float m21, float m22 )
		{
			M11 = m11; M12 = m12;
			M21 = m21; M22 = m22;
		}

		public static Matrix2x2 operator + ( Matrix2x2 v1, Matrix2x2 v2 )
		{
			v1.M11 += v2.M11;
			v1.M12 += v2.M12;
			v1.M21 += v2.M21;
			v1.M22 += v2.M22;
			return v1;
		}

		public static Matrix2x2 operator - ( Matrix2x2 v1, Matrix2x2 v2 )
		{
			v1.M11 -= v2.M11;
			v1.M12 -= v2.M12;
			v1.M21 -= v2.M21;
			v1.M22 -= v2.M22;
			return v1;
		}

		public static Matrix2x2 operator * ( Matrix2x2 v1, Matrix2x2 v2 )
		{
			v1.M11 = ( v1.M11 * v2.M11 ) + ( v1.M12 * v2.M21 );
			v1.M12 = ( v1.M11 * v2.M12 ) + ( v1.M12 * v2.M22 );
			v1.M21 = ( v1.M21 * v2.M11 ) + ( v1.M22 * v2.M21 );
			v1.M22 = ( v1.M21 * v2.M12 ) + ( v1.M22 * v2.M22 );
			return v1;
		}

		public static Matrix2x2 operator * ( Matrix2x2 v1, float v2 )
		{
			v1.M11 *= v2;
			v1.M12 *= v2;
			v1.M21 *= v2;
			v1.M22 *= v2;
			return v1;
		}

		public static Matrix2x2 operator / ( Matrix2x2 v1, Matrix2x2 v2 )
		{
			v1.M11 /= v2.M11;
			v1.M12 /= v2.M12;
			v1.M21 /= v2.M21;
			v1.M22 /= v2.M22;
			return v1;
		}

		public static Matrix2x2 operator / ( Matrix2x2 v1, float v2 )
		{
			v1.M11 /= v2;
			v1.M12 /= v2;
			v1.M21 /= v2;
			v1.M22 /= v2;
			return v1;
		}

		public static Matrix2x2 operator ! ( Matrix2x2 v1 )
		{
			return Invert ( v1 );
		}

		public static Matrix2x2 Transpose ( Matrix2x2 v1 )
		{
			Matrix2x2 mat = new Matrix2x2 ();
			mat.M11 = v1.M11;
			mat.M12 = v1.M21;
			mat.M21 = v1.M12;
			mat.M22 = v1.M22;
			return mat;
		}

		public Matrix2x2 Transpose ()
		{
			return Transpose ( this );
		}

		public Matrix2x2 TransposeMultiply ( Matrix2x2 v1, Matrix2x2 v2 )
		{
			Vector2 ex = new Vector2 ( M11, M12 );
			Vector2 ey = new Vector2 ( M21, M22 );

			Vector2 c1 = new Vector2 ( Vector2.Dot ( ex, ex ), Vector2.Dot ( ey, ex ) );
			Vector2 c2 = new Vector2 ( Vector2.Dot ( ex, ey ), Vector2.Dot ( ey, ey ) );

			return new Matrix2x2 ( c1.X, c1.Y, c2.X, c2.Y );
		}

		public static Matrix2x2 Invert ( Matrix2x2 v1 )
		{
			float d = 1 / ( v1.M11 * v1.M22 - v1.M12 * v1.M21 );
			v1.M11 = d * v1.M22;
			v1.M12 = d * v1.M12;
			v1.M21 = d * v1.M21;
			v1.M22 = d * v1.M11;
			return v1;
		}

		public static Vector2 Solve ( Matrix2x2 matrix, Vector2 vector )
		{
			float a11 = matrix.M11, a12 = matrix.M21, a21 = matrix.M12, a22 = matrix.M22;
			float det = a11 * a22 - a12 * a21;
			if ( det != 0.0f ) det = 1.0f / det;
			return new Vector2 ( det * ( a22 * vector.X - a12 * vector.Y ), det * ( a11 * vector.Y - a21 * vector.X ) );
		}

		public Vector2 Solve ( Vector2 value )
		{
			return Solve ( this, value );
		}

		public bool Equals ( Matrix2x2 other )
		{
			return ( M11 == other.M11 && M12 == other.M12 ) &&
				( M21 == other.M21 && M22 == other.M22 );
		}

		public static bool operator == ( Matrix2x2 v1, Matrix2x2 v2 )
		{
			return v1.Equals ( v2 );
		}

		public static bool operator != ( Matrix2x2 v1, Matrix2x2 v2 )
		{
			return !v1.Equals ( v2 );
		}

		public override bool Equals ( object obj )
		{
			if ( !( obj is Matrix2x2 ) ) return false;
			return Equals ( ( Matrix2x2 ) obj );
		}

		public override int GetHashCode ()
		{
			return ToString ().GetHashCode ();
		}

		public override string ToString ()
		{
			return String.Format ( "{{11:{0}, 12:{1}} {21:{2}, 22:{3}}}",
				M11, M12, M21, M22 );
		}

		public float [] ToArray ()
		{
			return new float []
			{
				M11, M12,
				M21, M22,
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
					case 2: return M21;
					case 3: return M22;
					default: throw new IndexOutOfRangeException ();
				}
			}
			set
			{
				switch ( index )
				{
					case 0: M11 = value; break;
					case 1: M12 = value; break;
					case 2: M21 = value; break;
					case 3: M22 = value; break;
					default: throw new IndexOutOfRangeException ();
				}
			}
		}

		public float this [ int x, int y ]
		{
			get { return this [ x + ( y * 2 ) ]; }
			set { this [ x + ( y * 2 ) ] = value; }
		}
	}
}
