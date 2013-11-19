using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public class World2 : IWorld<Vector2, float>
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

		public World2 ()
		{
			Scale = new Vector2 ( 1 );
		}

		public World2 ( Vector2 translate, Vector2 scale, Vector2 scaleCenter, float rotation, Vector2 rotationCenter )
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

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = Matrix4x4.Identity;

				matrix *= CommonTransform.Translate ( new Vector3 ( -RotationCenter, 0 ) );
				matrix *= CommonTransform.RotationZ ( Rotation );
				matrix *= CommonTransform.Translate ( new Vector3 ( RotationCenter, 0 ) );

				matrix *= CommonTransform.Translate ( new Vector3 ( -ScaleCenter, 0 ) );
				matrix *= CommonTransform.Scale ( new Vector3 ( Scale, 1 ) );
				matrix *= CommonTransform.Translate ( new Vector3 ( ScaleCenter, 0 ) );

				matrix *= CommonTransform.Translate ( new Vector3 ( Translate, 0 ) );

				return matrix;
			}
		}
	}
}
