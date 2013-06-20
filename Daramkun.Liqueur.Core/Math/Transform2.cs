using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public struct Transform2 : ITransform<Vector2, float>
	{
		public Vector2 Translate { get; set; }
		public Vector2 ScaleCenter { get; set; }
		public Vector2 Scale { get; set; }
		public float Rotation { get; set; }
		public Vector2 RotationCenter { get; set; }

		public static Transform2 Identity
		{
			get { return new Transform2 () { Scale = new Vector2 ( 1, 1 ) }; }
		}

		public Transform2 ( Vector2 translate, Vector2 scaleCenter, Vector2 scale, float rotation, Vector2 rotationCenter )
			: this ()
		{
			Translate = translate;
			ScaleCenter = scaleCenter;
			Scale = scale;
			Rotation = rotation;
			RotationCenter = rotationCenter;
		}

		public static Transform2 operator + ( Transform2 v1, Transform2 v2 )
		{
			return new Transform2 ( v1.Translate + v2.Translate, v1.ScaleCenter + v2.ScaleCenter,
				v1.Scale * v2.Scale, v1.Rotation + v2.Rotation, v1.RotationCenter + v2.RotationCenter );
		}

		public static Transform2 operator - ( Transform2 v1, Transform2 v2 )
		{
			return new Transform2 ( v1.Translate - v2.Translate, v1.ScaleCenter - v2.ScaleCenter,
				v1.Scale / v2.Scale, v1.Rotation - v2.Rotation, v1.RotationCenter - v2.RotationCenter );
		}

		private Matrix4x4 TranslationMatrix ( Vector2 translate )
		{
			return new Matrix4x4 (
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				translate.X, translate.Y, 0, 1
				);
		}

		private Matrix4x4 ScaleMatrix ( Vector2 scale )
		{
			return new Matrix4x4 (
				scale.X, 0, 0, 0,
				0, scale.Y, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 1 );
		}

		private Matrix4x4 RotateMatrix ( float rotation )
		{
			Matrix4x4 ret = Matrix4x4.Identity;

			var val1 = ( float ) System.Math.Cos ( rotation );
			var val2 = ( float ) System.Math.Sin ( rotation );

			ret.M11 = val1;
			ret.M12 = val2;
			ret.M21 = -val2;
			ret.M22 = val1;

			return ret;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = Matrix4x4.Identity;
				matrix *= TranslationMatrix ( Translate );
				matrix *= RotateMatrix ( Rotation );
				matrix *= TranslationMatrix ( new Vector2 ( -RotationCenter.X + ScaleCenter.X, -RotationCenter.Y + ScaleCenter.Y ) );
				matrix *= ScaleMatrix ( Scale );
				matrix *= TranslationMatrix ( -ScaleCenter );
				return matrix;
			}
		}
	}
}
