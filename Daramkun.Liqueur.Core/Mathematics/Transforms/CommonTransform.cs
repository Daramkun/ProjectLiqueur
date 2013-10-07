using System;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public static class CommonTransform
	{


		public static Matrix4x4 CreateMatrixFromAxisAngle ( Vector3 axis, float angle )
		{
			Matrix4x4 matrix;
			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;
			float num2 = ( float ) Math.Sin ( ( double ) angle );
			float num = ( float ) Math.Cos ( ( double ) angle );
			float num11 = x * x;
			float num10 = y * y;
			float num9 = z * z;
			float num8 = x * y;
			float num7 = x * z;
			float num6 = y * z;
			matrix.M11 = num11 + ( num * ( 1f - num11 ) );
			matrix.M12 = ( num8 - ( num * num8 ) ) + ( num2 * z );
			matrix.M13 = ( num7 - ( num * num7 ) ) - ( num2 * y );
			matrix.M14 = 0f;
			matrix.M21 = ( num8 - ( num * num8 ) ) - ( num2 * z );
			matrix.M22 = num10 + ( num * ( 1f - num10 ) );
			matrix.M23 = ( num6 - ( num * num6 ) ) + ( num2 * x );
			matrix.M24 = 0f;
			matrix.M31 = ( num7 - ( num * num7 ) ) + ( num2 * y );
			matrix.M32 = ( num6 - ( num * num6 ) ) - ( num2 * x );
			matrix.M33 = num9 + ( num * ( 1f - num9 ) );
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}
	}
}

