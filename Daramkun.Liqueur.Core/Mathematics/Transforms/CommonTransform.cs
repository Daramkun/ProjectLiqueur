using System;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public static class CommonTransform
	{
		public static float Determinant(Matrix4x4 matrix)
		{
			float num22 = matrix.M11, num21 = matrix.M12, num20 = matrix.M13, num19 = matrix.M14;
			float num12 = matrix.M21, num11 = matrix.M22, num10 = matrix.M23, num9 = matrix.M24;
			float num8 = matrix.M31, num7 = matrix.M32, num6 = matrix.M33, num5 = matrix.M34;
			float num4 = matrix.M41, num3 = matrix.M42, num2 = matrix.M43, num = matrix.M44;
			float num18 = (num6 * num) - (num5 * num2), num17 = (num7 * num) - (num5 * num3);
			float num16 = (num7 * num2) - (num6 * num3), num15 = (num8 * num) - (num5 * num4);
			float num14 = (num8 * num2) - (num6 * num4), num13 = (num8 * num3) - (num7 * num4);
			return (
				(((num22 * (((num11 * num18) - (num10 * num17)) + (num9 * num16))) - 
			    (num21 * (((num12 * num18) - (num10 * num15)) + (num9 * num14)))) + 
			    (num20 * (((num12 * num17) - (num11 * num15)) + (num9 * num13)))) - 
			    (num19 * (((num12 * num16) - (num11 * num14)) + (num10 * num13)))
			);
		}

		public static Matrix4x4 Invert(Matrix4x4 matrix)
		{

		}
	}
}

