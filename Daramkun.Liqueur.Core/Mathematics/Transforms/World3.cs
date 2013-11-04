using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public class World3 : IWorld<Vector3, Vector3>
	{
		public Vector3 Translate { get; set; }
		public Vector3 ScaleCenter { get; set; }
		public Vector3 Scale { get; set; }
		public Vector3 Rotation { get; set; }
		public Vector3 RotationCenter { get; set; }

		public static World3 Identity
		{
			get { return new World3 () { Scale = new Vector3 ( 1, 1, 1 ) }; }
		}

		public World3 ()
		{
			Scale = new Vector3 ( 1 );
		}

		public World3 ( Vector3 translate, Vector3 scaleCenter, Vector3 scale, Vector3 rotation, Vector3 rotationCenter )
			: this ()
		{
			Translate = translate;
			ScaleCenter = scaleCenter;
			Scale = scale;
			Rotation = rotation;
			RotationCenter = rotationCenter;
		}

		public static World3 operator + ( World3 v1, World3 v2 )
		{
			return new World3 ( v1.Translate + v2.Translate, v1.ScaleCenter + v2.ScaleCenter,
				v1.Scale * v2.Scale, v1.Rotation + v2.Rotation, v1.RotationCenter + v2.RotationCenter );
		}

		public static World3 operator - ( World3 v1, World3 v2 )
		{
			return new World3 ( v1.Translate - v2.Translate, v1.ScaleCenter - v2.ScaleCenter,
				v1.Scale / v2.Scale, v1.Rotation - v2.Rotation, v1.RotationCenter - v2.RotationCenter );
		}

		private Matrix4x4 TranslationMatrix ( Vector3 translate )
		{
			return new Matrix4x4 (
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				translate.X, translate.Y, translate.Z, 1
			);
		}

		private Matrix4x4 ScaleMatrix ( Vector3 scale )
		{
			return new Matrix4x4 (
				scale.X, 0, 0, 0,
				0, scale.Y, 0, 0,
				0, 0, scale.Z, 0,
				0, 0, 0, 1
			);
		}

		private Matrix4x4 RotateX ( float value )
		{
			var val1 = ( float ) System.Math.Cos ( value );
			var val2 = ( float ) System.Math.Sin ( value );

			return new Matrix4x4 (
				1, 0, 0, 0,
				0, val1, val2, 0,
				0, -val2, val1, 0,
				0, 0, 0, 1
			);
		}

		private Matrix4x4 RotateY ( float value )
		{
			var val1 = ( float ) System.Math.Cos ( value );
			var val2 = ( float ) System.Math.Sin ( value );

			return new Matrix4x4 (
				val1, 0, -val2, 0,
				0, 1, 0, 0,
				val2, 0, val1, 0,
				0, 0, 0, 1
			);
		}

		private Matrix4x4 RotateZ ( float value )
		{
			var val1 = ( float ) System.Math.Cos ( value );
			var val2 = ( float ) System.Math.Sin ( value );

			return new Matrix4x4 (
				val1, val2, 0, 0,
				-val2, val1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1
			);
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

				matrix *= TranslationMatrix ( -RotationCenter );
				matrix *= RotateMatrix ( Rotation );
				matrix *= TranslationMatrix ( RotationCenter );

				matrix *= TranslationMatrix ( -ScaleCenter );
				matrix *= ScaleMatrix ( Scale );
				matrix *= TranslationMatrix ( ScaleCenter );

				matrix *= TranslationMatrix ( Translate );

				return matrix;
			}
		}
	}
}
