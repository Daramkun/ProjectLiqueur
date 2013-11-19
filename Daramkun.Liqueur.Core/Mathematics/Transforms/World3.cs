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

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = Matrix4x4.Identity;

				matrix *= CommonTransform.Translate ( -RotationCenter );
				matrix *= CommonTransform.RotationXYZ ( Rotation );
				matrix *= CommonTransform.Translate ( RotationCenter );

				matrix *= CommonTransform.Translate ( -ScaleCenter );
				matrix *= CommonTransform.Scale ( Scale );
				matrix *= CommonTransform.Translate ( ScaleCenter );

				matrix *= CommonTransform.Translate ( Translate );

				return matrix;
			}
		}
	}
}
