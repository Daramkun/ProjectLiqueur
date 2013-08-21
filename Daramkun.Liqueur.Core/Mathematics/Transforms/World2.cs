using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public struct World2 : IWorld<Vector2, float>
	{
		public Vector2 Translate { get; set; }
		public Vector2 ScaleCenter { get; set; }
		public Vector2 Scale { get; set; }
		public float Rotation { get; set; }
		public Vector2 RotationCenter { get; set; }

		public static World2 Identity
		{
			get { return new World2 () { Scale = new Vector2 ( 1, 1 ) }; }
		}

		public World2 ( Vector2 translate, Vector2 scaleCenter, Vector2 scale, float rotation, Vector2 rotationCenter )
			: this ()
		{
			Translate = translate;
			ScaleCenter = scaleCenter;
			Scale = scale;
			Rotation = rotation;
			RotationCenter = rotationCenter;
		}

		public static World2 operator + ( World2 v1, World2 v2 )
		{
			return new World2 ( v1.Translate + v2.Translate, v1.ScaleCenter + v2.ScaleCenter,
				v1.Scale * v2.Scale, v1.Rotation + v2.Rotation, v1.RotationCenter + v2.RotationCenter );
		}

		public static World2 operator - ( World2 v1, World2 v2 )
		{
			return new World2 ( v1.Translate - v2.Translate, v1.ScaleCenter - v2.ScaleCenter,
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
				0, 0, 1, 0,
				0, 0, 0, 1 );
		}

		private Matrix4x4 RotateMatrix ( float rotation )
		{
			var val1 = ( float ) System.Math.Cos ( rotation );
			var val2 = ( float ) System.Math.Sin ( rotation );

			return new Matrix4x4 (
				val1, val2, 0, 0,
				-val2, val1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1
			);
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = Matrix4x4.Identity;
				matrix *= TranslationMatrix ( RotationCenter );
				matrix *= RotateMatrix ( Rotation );
				matrix *= TranslationMatrix ( -RotationCenter );
				matrix *= TranslationMatrix ( ScaleCenter );
				matrix *= ScaleMatrix ( Scale );
				matrix *= TranslationMatrix ( -ScaleCenter );
				matrix *= TranslationMatrix ( Translate );
				return matrix;
			}
		}
	}
}
