using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public class PerspectiveFieldOfViewProjection : IPerspectiveProjection
	{
		public float FieldOfView { get; set; }
		public float AspectRatio { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }

		public PerspectiveFieldOfViewProjection ( float fieldOfView, float aspectRatio, float near, float far )
			//: this ()
		{
			FieldOfView = fieldOfView;
			AspectRatio = aspectRatio;
			Near = near;
			Far = far;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = new Matrix4x4 ();
				float num = 1 / ( float ) System.Math.Tan ( FieldOfView * 0.5f );
				matrix.M11 = num / AspectRatio;
				matrix.M22 = num;
				matrix.M33 = Far / ( Near - Far );
				matrix.M34 = -1;
				matrix.M43 = ( Near * Far ) / ( Near - Far );
				return matrix;
			}
		}
	}
}
