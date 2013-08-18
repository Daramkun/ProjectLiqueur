using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public struct View : ITransform
	{
		public Vector3 Position { get; set; }
		public Vector3 Target { get; set; }
		public Vector3 UpVector { get; set; }

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = new Matrix4x4 ();
				Vector3 vector = Vector3.Normalize ( Position - Target );
				Vector3 vector2 = Vector3.Normalize ( Vector3.Cross ( UpVector, vector ) );
				Vector3 vector3 = Vector3.Cross ( vector, vector2 );
				matrix.M11 = vector2.X;
				matrix.M12 = vector3.X;
				matrix.M13 = vector.X;
				matrix.M21 = vector2.Y;
				matrix.M22 = vector3.Y;
				matrix.M23 = vector.Y;
				matrix.M31 = vector2.Z;
				matrix.M32 = vector3.Z;
				matrix.M33 = vector.Z;
				matrix.M41 = -Vector3.Dot ( vector2, Position );
				matrix.M42 = -Vector3.Dot ( vector3, Position );
				matrix.M43 = -Vector3.Dot ( vector, Position );
				matrix.M44 = 1f;
				return matrix;
			}
		}

		public View ( Vector3 position, Vector3 target, Vector3 upVector )
			: this ()
		{
			Position = position;
			Target = target;
			UpVector = upVector;
		}
	}
}
