using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public struct Transform3 : ITransform<Vector3, Vector3>
	{
		public Vector3 Translate { get; set; }
		public Vector3 ScaleCenter { get; set; }
		public Vector3 Scale { get; set; }
		public Vector3 Rotation { get; set; }
		public Vector3 RotationCenter { get; set; }

		public static Transform3 Identity
		{
			get { return new Transform3 () { Scale = new Vector3 ( 1, 1, 1 ) }; }
		}

		public Transform3 ( Vector3 translate, Vector3 scaleCenter, Vector3 scale, Vector3 rotation, Vector3 rotationCenter )
			: this ()
		{
			Translate = translate;
			ScaleCenter = scaleCenter;
			Scale = scale;
			Rotation = rotation;
			RotationCenter = rotationCenter;
		}

		public static Transform3 operator + ( Transform3 v1, Transform3 v2 )
		{
			return new Transform3 ( v1.Translate + v2.Translate, v1.ScaleCenter + v2.ScaleCenter,
				v1.Scale * v2.Scale, v1.Rotation + v2.Rotation, v1.RotationCenter + v2.RotationCenter );
		}

		public static Transform3 operator - ( Transform3 v1, Transform3 v2 )
		{
			return new Transform3 ( v1.Translate - v2.Translate, v1.ScaleCenter - v2.ScaleCenter,
				v1.Scale / v2.Scale, v1.Rotation - v2.Rotation, v1.RotationCenter - v2.RotationCenter );
		}

		private Matrix4x4 TranslationMatrix ( Vector3 translate )
		{
			return new Matrix4x4 (
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				translate.X, translate.Y, 0, 1
				);
		}

		private Matrix4x4 ScaleMatrix ( Vector3 scale )
		{
			return new Matrix4x4 (
				scale.X, 0, 0, 0,
				0, scale.Y, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 1 );
		}

		private Matrix4x4 RotateX ( float value )
		{
			Matrix4x4 ret = Matrix4x4.Identity;

			var val1 = ( float ) System.Math.Cos ( value );
			var val2 = ( float ) System.Math.Sin ( value );

			ret.M22 = val1;
			ret.M23 = val2;
			ret.M32 = -val2;
			ret.M33 = val1;

			return ret;
		}

		private Matrix4x4 RotateY ( float value )
		{
			Matrix4x4 ret = Matrix4x4.Identity;

			var val1 = ( float ) System.Math.Cos ( value );
			var val2 = ( float ) System.Math.Sin ( value );

			ret.M11 = val1;
			ret.M13 = val2;
			ret.M31 = -val2;
			ret.M33 = val1;

			return ret;
		}

		private Matrix4x4 RotateZ ( float value )
		{
			Matrix4x4 ret = Matrix4x4.Identity;

			var val1 = ( float ) System.Math.Cos ( value );
			var val2 = ( float ) System.Math.Sin ( value );

			ret.M11 = val1;
			ret.M12 = val2;
			ret.M21 = -val2;
			ret.M22 = val1;

			return ret;
		}

		private Matrix4x4 RotateMatrix ( Vector3 rotation )
		{
			return RotateX ( rotation.X ) * RotateY ( rotation.Y ) * RotateZ ( rotation.Z );
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = Matrix4x4.Identity;
				matrix *= TranslationMatrix ( Translate );
				matrix *= RotateMatrix ( Rotation );
				matrix *= TranslationMatrix ( new Vector3 ( -RotationCenter.X + ScaleCenter.X, -RotationCenter.Y + ScaleCenter.Y,
					-RotationCenter.Z + ScaleCenter.Z ) );
				matrix *= ScaleMatrix ( Scale );
				matrix *= TranslationMatrix ( -ScaleCenter );
				return matrix;
			}
		}
	}
}